using System.Diagnostics;
using AumauiCL.Interfaces;
using AumauiCL.Models.Api;
using AumauiCL.Models.User;
using AumauiCL.Services.Authentication;

namespace AumauiCL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ISyncService _syncService;
        private readonly IApiService _apiService;
        private readonly ISecureStorageService _secureStorage;
        private readonly HostAuthenticationStateProvider _authStateProvider;
        private readonly IMsalService _msalService;

        // Storage keys
        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";
        private const string CompanyCodeKey = "company_code";

        public AuthService(
            IDatabaseService databaseService,
            ISyncService syncService,
            IApiService apiService,
            ISecureStorageService secureStorage,
            HostAuthenticationStateProvider authStateProvider,
            IMsalService msalService)
        {
            _databaseService = databaseService;
            _syncService = syncService;
            _apiService = apiService;
            _secureStorage = secureStorage;
            _authStateProvider = authStateProvider;
            _msalService = msalService;
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
                // MSAL interactive login — handled by the singleton (token caching, silent re-auth)
                var msalResult = await _msalService.SignInInteractiveAsync();

                // Call backend with full BodyData as expected by the API
                var apiRequest = new APIRequest<MicrosoftLoginRequest>
                {
                    Code = companyCode,
                    BodyData = new MicrosoftLoginRequest
                    {
                        MicrosoftUserId = msalResult.MicrosoftUserId,
                        Email = msalResult.Email,
                        DisplayName = msalResult.DisplayName,
                        AccessToken = msalResult.AccessToken
                    }
                };

                var apiResponse = await _apiService.MicrosoftLoginAsync(apiRequest);

                if (!apiResponse.IsSuccessful || apiResponse.ResponseData == null || !apiResponse.ResponseData.Success)
                {
                    var msg = apiResponse.ResponseData?.Message ?? apiResponse.ResponseMessage ?? "Microsoft login failed";
                    throw new Exception(msg);
                }

                // Populate UserModel from API response data
                var user = new UserModel
                {
                    Email = apiResponse.ResponseData.User?.Email ?? msalResult.Email,
                    Name = apiResponse.ResponseData.User?.Name ?? msalResult.DisplayName,
                    MicrosoftId = msalResult.MicrosoftUserId,
                    CompanyCode = companyCode,
                    Company = apiResponse.ResponseData.User?.Company ?? string.Empty,
                    ExternalId = apiResponse.ResponseData.User?.UserID ?? Guid.NewGuid().ToString(),
                    UserName = msalResult.Email
                };

                // Store tenant context + tokens
                await _secureStorage.SetAsync(CompanyCodeKey, companyCode);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.AccessToken))
                    await _secureStorage.SetAsync(AccessTokenKey, apiResponse.ResponseData.AccessToken);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.RefreshToken))
                    await _secureStorage.SetAsync(RefreshTokenKey, apiResponse.ResponseData.RefreshToken);

                await _databaseService.SaveItemAsync(user);
                await _syncService.SyncModuleAsync<UserModel>("users");

                // Mark user as authenticated in Blazor auth state
                _authStateProvider.MarkUserAsAuthenticated(user.Email);

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

                var apiResponse = await _apiService.SHEQsysLoginAsync(loginRequest);

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

                // Populate UserModel from API response data
                var user = new UserModel
                {
                    Email = apiResponse.ResponseData.User?.Email ?? email,
                    Name = apiResponse.ResponseData.User?.Name ?? email.Split('@')[0],
                    CompanyCode = companyCode,
                    Company = apiResponse.ResponseData.User?.Company ?? string.Empty,
                    MicrosoftId = "N/A",
                    ExternalId = apiResponse.ResponseData.User?.UserID ?? Guid.NewGuid().ToString(),
                    UserName = apiResponse.ResponseData.User?.Email ?? email
                };

                // Store tenant context + tokens
                await _secureStorage.SetAsync(CompanyCodeKey, companyCode);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.AccessToken))
                    await _secureStorage.SetAsync(AccessTokenKey, apiResponse.ResponseData.AccessToken);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.RefreshToken))
                    await _secureStorage.SetAsync(RefreshTokenKey, apiResponse.ResponseData.RefreshToken);

                await _databaseService.SaveItemAsync(user);
                await _syncService.SyncModuleAsync<UserModel>("users");

                // Mark user as authenticated in Blazor auth state
                _authStateProvider.MarkUserAsAuthenticated(user.Email);

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

            // Clear MSAL cached accounts
            await _msalService.SignOutAsync();

            // Clear all stored context
            _secureStorage.Remove(AccessTokenKey);
            _secureStorage.Remove(RefreshTokenKey);
            _secureStorage.Remove(CompanyCodeKey);

            // Mark user as logged out in Blazor auth state
            _authStateProvider.MarkUserAsLoggedOut();
        }

        public async Task<bool> TryRefreshTokenAsync()
        {
            try
            {
                var refreshToken = await _secureStorage.GetAsync(RefreshTokenKey);
                var companyCode = await _secureStorage.GetAsync(CompanyCodeKey);

                if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(companyCode))
                    return false;

                var request = new APIRequest
                {
                    Code = companyCode,
                    Header = refreshToken
                };

                var apiResponse = await _apiService.RefreshTokenAsync(request);

                if (!apiResponse.IsSuccessful || apiResponse.ResponseData == null || !apiResponse.ResponseData.Success)
                    return false;

                // Swap tokens in secure storage
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.AccessToken))
                    await _secureStorage.SetAsync(AccessTokenKey, apiResponse.ResponseData.AccessToken);
                if (!string.IsNullOrEmpty(apiResponse.ResponseData.RefreshToken))
                    await _secureStorage.SetAsync(RefreshTokenKey, apiResponse.ResponseData.RefreshToken);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Token refresh failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _secureStorage.GetAsync(AccessTokenKey);
            return !string.IsNullOrEmpty(token);
        }
    }
}
