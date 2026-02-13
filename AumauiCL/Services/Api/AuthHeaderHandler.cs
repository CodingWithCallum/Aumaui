using System.Net.Http.Headers;
using AumauiCL.Interfaces;

namespace AumauiCL.Services.Api
{
    /// <summary>
    /// HTTP message handler that automatically attaches the SHEQsys JWT Bearer token
    /// to every outgoing API request. Registered in the HttpClient pipeline via DI
    /// so individual service methods don't need to handle token attachment themselves.
    /// </summary>
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ISecureStorageService _secureStorage;

        public AuthHeaderHandler(ISecureStorageService secureStorage)
        {
            _secureStorage = secureStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _secureStorage.GetAsync("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
