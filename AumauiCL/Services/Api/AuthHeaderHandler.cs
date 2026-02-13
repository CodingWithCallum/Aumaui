using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AumauiCL.Interfaces;
using AumauiCL.Models.Api;

namespace AumauiCL.Services.Api
{
    /// <summary>
    /// HTTP message handler that automatically attaches the SHEQsys JWT Bearer token
    /// to every outgoing API request. If the server returns 401, it attempts a single
    /// token refresh and retries the original request.
    /// </summary>
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ISecureStorageService _secureStorage;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AuthHeaderHandler(ISecureStorageService secureStorage)
        {
            _secureStorage = secureStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Attach current token
            var token = await _secureStorage.GetAsync("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // On 401, attempt one token refresh cycle
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshDirectAsync();
                if (refreshed)
                {
                    // Retry with the new token
                    var newToken = await _secureStorage.GetAsync("access_token");
                    var retry = await CloneRequestAsync(request);
                    retry.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", newToken);
                    response = await base.SendAsync(retry, cancellationToken);
                }
            }

            return response;
        }

        /// <summary>
        /// Calls the refresh endpoint directly (bypassing the DI HttpClient pipeline)
        /// to avoid the circular dependency AuthService → ApiService → HttpClient → this handler.
        /// </summary>
        private async Task<bool> TryRefreshDirectAsync()
        {
            try
            {
                var refreshToken = await _secureStorage.GetAsync("refresh_token");
                var companyCode = await _secureStorage.GetAsync("company_code");

                if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(companyCode))
                    return false;

                var refreshRequest = new APIRequest
                {
                    Code = companyCode,
                    Header = refreshToken
                };

                using var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(ApiConfig.BaseUrl)
                };

                var json = JsonSerializer.Serialize(refreshRequest, JsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(
                    "/api/Authentication/SHEQsysRefreshCurrentToken", content);

                if (!response.IsSuccessStatusCode)
                    return false;

                var body = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<APIResponse<LoginResponse>>(body, JsonOptions);

                if (apiResponse?.ResponseData == null || !apiResponse.ResponseData.Success)
                    return false;

                // Swap tokens in secure storage
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.AccessToken))
                    await _secureStorage.SetAsync("access_token", apiResponse.ResponseData.AccessToken);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.RefreshToken))
                    await _secureStorage.SetAsync("refresh_token", apiResponse.ResponseData.RefreshToken);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AuthHeaderHandler token refresh failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Clones an HttpRequestMessage so it can be resent (original is disposed after first send).
        /// </summary>
        private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
        {
            var clone = new HttpRequestMessage(original.Method, original.RequestUri);

            if (original.Content != null)
            {
                var contentBytes = await original.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(contentBytes);

                foreach (var header in original.Content.Headers)
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var header in original.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }
    }
}
