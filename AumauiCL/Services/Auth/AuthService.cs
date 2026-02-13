using System.Diagnostics;
using AumauiCL.Interfaces;
using AumauiCL.Models.Api;
using AumauiCL.Models.User;
using Microsoft.Identity.Client;

namespace AumauiCL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ISyncService _syncService;
        private readonly IApiService _apiService;
        private readonly ISecureStorageService _secureStorage;

        // MSAL Configuration
        private const string ClientId = "YOUR_CLIENT_ID_HERE"; // Placeholder
        private const string RedirectUri = "msauth://com.companyname.aumaui"; // Placeholder
        private string[] Scopes = new string[] { "User.Read" };

        // Storage keys
        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string CompanyCodeKey = "company_code";

        public AuthService(IDatabaseService databaseService, ISyncService syncService, IApiService apiService, ISecureStorageService secureStorage)
        {
            _databaseService = databaseService;
            _syncService = syncService;
            _apiService = apiService;
            _secureStorage = secureStorage;
        }

        public async Task<UserModel?> GetCurrentUserAsync()
        {
            var users = await _databaseService.GetItemsAsync<UserModel>();
            return users.FirstOrDefault();
        }

        public async Task<UserModel> LoginWithMicrosoftAsync(string companyCode)
        {
            try
            {
                // Use "common" authority for multi-tenant — backend validates tenant membership via Code
                var authority = "https://login.microsoftonline.com/common";

                var pca = PublicClientApplicationBuilder
                    .Create(ClientId)
                    .WithAuthority(authority)
                    .WithRedirectUri(RedirectUri)
#if ANDROID
                    .WithParentActivityOrWindow(() => Platform.CurrentActivity)
#endif
                    .Build();

                var msalResult = await pca.AcquireTokenInteractive(Scopes)
                                          .ExecuteAsync();

                // Call backend — tenant resolution happens server-side via Code
                var apiRequest = new APIRequest
                {
                    Code = companyCode,
                    ProviderKey = msalResult.UniqueId
                };

                var apiResponse = await _apiService.MicrosoftLoginAsync(apiRequest);

                if (!apiResponse.IsSuccessful || apiResponse.ResponseData == null || !apiResponse.ResponseData.Success)
                {
                    var msg = apiResponse.ResponseData?.Message ?? apiResponse.ResponseMessage ?? "Microsoft login failed";
                    throw new Exception(msg);
                }

                var user = new UserModel
                {
                    Email = msalResult.Account.Username,
                    Name = msalResult.Account.Username,
                    MicrosoftId = msalResult.UniqueId,
                    CompanyCode = companyCode,
                    Company = "Derived from API",
                    ExternalId = apiResponse.ResponseData.AccessToken ?? Guid.NewGuid().ToString(),
                    UserName = msalResult.Account.Username
                };

                // Store tenant context + tokens
                await _secureStorage.SetAsync(CompanyCodeKey, companyCode);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.AccessToken))
                    await _secureStorage.SetAsync(AccessTokenKey, apiResponse.ResponseData.AccessToken);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.RefreshToken))
                    await _secureStorage.SetAsync(RefreshTokenKey, apiResponse.ResponseData.RefreshToken);

                await _databaseService.SaveItemAsync(user);
                await _syncService.SyncModuleAsync<UserModel>("users");

                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Microsoft Login Failed: {ex.Message}");
                throw;
            }
        }

        public async Task<UserModel> LoginWithStandardAsync(string companyCode, string email, string password)
        {
            try
            {
                // Backend resolves tenant via Code field
                var loginRequest = new APIRequest<LoginRequest>
                {
                    Code = companyCode,
                    BodyData = new LoginRequest
                    {
                        Email = email,
                        Password = password
                    }
                };

                var apiResponse = await _apiService.XsysLoginAsync(loginRequest);

                if (!apiResponse.IsSuccessful || apiResponse.ResponseData == null || !apiResponse.ResponseData.Success)
                {
                    var msg = apiResponse.ResponseData?.Message ?? apiResponse.ResponseMessage ?? "Login failed";

                    // Surface validation errors if present
                    if (apiResponse.ResponseValidation?.Count > 0)
                    {
                        var validationMessages = string.Join("; ",
                            apiResponse.ResponseValidation.Select(v => v.ValidationMessage));
                        msg = $"{msg}: {validationMessages}";
                    }

                    throw new Exception(msg);
                }

                var user = new UserModel
                {
                    Email = email,
                    Name = email.Split('@')[0],
                    CompanyCode = companyCode,
                    Company = "From API",
                    MicrosoftId = "N/A",
                    ExternalId = apiResponse.ResponseData.AccessToken ?? Guid.NewGuid().ToString(),
                    UserName = email
                };

                // Store tenant context + tokens
                await _secureStorage.SetAsync(CompanyCodeKey, companyCode);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.AccessToken))
                    await _secureStorage.SetAsync(AccessTokenKey, apiResponse.ResponseData.AccessToken);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.RefreshToken))
                    await _secureStorage.SetAsync(RefreshTokenKey, apiResponse.ResponseData.RefreshToken);

                await _databaseService.SaveItemAsync(user);
                await _syncService.SyncModuleAsync<UserModel>("users");

                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Standard Login Failed: {ex.Message}");
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            var users = await _databaseService.GetItemsAsync<UserModel>();
            foreach (var user in users)
            {
                await _databaseService.DeleteItemAsync(user);
            }

            // Clear all stored context
            _secureStorage.Remove(AccessTokenKey);
            _secureStorage.Remove(RefreshTokenKey);
            _secureStorage.Remove(CompanyCodeKey);
        }
    }
}
