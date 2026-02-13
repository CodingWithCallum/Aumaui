using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AumauiCL.Interfaces;
using AumauiCL.Models.Api;

namespace AumauiCL.Services.Api;

public class ApiService : IApiService
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient httpClient)
    {
        _http = httpClient;
        // BaseAddress is now set via DI in MauiProgram.cs
    }

    // ─── Generic HTTP Helper ───────────────────────────────────────────

    public async Task<TRes> PostAsync<TReq, TRes>(string endpoint, TReq body)
    {
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(body, JsonOptions),
            Encoding.UTF8,
            "application/json"
        );

        // Token is now attached automatically by AuthHeaderHandler in the HTTP pipeline

        var response = await _http.PostAsync(endpoint, jsonContent);
        var content = await response.Content.ReadAsStringAsync();

        // Try to deserialize regardless of status code
        // so API-level errors in the response body are surfaced properly
        if (!response.IsSuccessStatusCode)
        {
            // Attempt to deserialize as TRes (e.g. APIResponse) for error details
            try
            {
                var errorResult = JsonSerializer.Deserialize<TRes>(content, JsonOptions);
                if (errorResult != null) return errorResult;
            }
            catch { /* Fall through to generic error */ }

            throw new HttpRequestException(
                $"API request to {endpoint} failed with status {response.StatusCode}: {content}");
        }

        return JsonSerializer.Deserialize<TRes>(content, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize response from {endpoint}");
    }

    // ─── Authentication Endpoints ──────────────────────────────────────

    public async Task<APIResponse<LoginResponse>> SHEQsysLoginAsync(APIRequest<LoginRequest> request)
    {
        return await PostAsync<APIRequest<LoginRequest>, APIResponse<LoginResponse>>(
            "/Authentication/SHEQsysLogin", request);
    }

    public async Task<APIResponse<LoginResponse>> MicrosoftLoginAsync(APIRequest<MicrosoftLoginRequest> request)
    {
        return await PostAsync<APIRequest<MicrosoftLoginRequest>, APIResponse<LoginResponse>>(
            "/Authentication/MicrosoftLogin", request);
    }

    public async Task<APIResponse<LoginResponse>> RefreshTokenAsync(APIRequest request)
    {
        return await PostAsync<APIRequest, APIResponse<LoginResponse>>(
            "/Authentication/SHEQsysRefreshCurrentToken", request);
    }

    // ─── Generic Data Operations ───────────────────────────────────────

    public async Task<List<T>> GetItemsAsync<T>(string endpoint)
    {
        // Token is now attached automatically by AuthHeaderHandler in the HTTP pipeline

        var response = await _http.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<T>>(content, JsonOptions)
            ?? new List<T>();
    }

    public async Task<T> PushItemAsync<T>(string endpoint, T item)
    {
        return await PostAsync<T, T>(endpoint, item);
    }
}
