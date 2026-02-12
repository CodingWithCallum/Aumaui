using System.Diagnostics;
using AumauiCL.Interfaces;
using AumauiCL.Models.User;
using AumauiCL.Services.Data;
using AumauiCL.Services.Sync;
using Microsoft.Identity.Client;

namespace AumauiCL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly DatabaseService _databaseService;
        private readonly SyncService _syncService;

        // MSAL Configuration
        private const string ClientId = "YOUR_CLIENT_ID_HERE"; // Placeholder
        private const string RedirectUri = "msauth://com.companyname.aumaui"; // Placeholder
        private string[] Scopes = new string[] { "User.Read" };

        public AuthService(DatabaseService databaseService, SyncService syncService)
        {
            _databaseService = databaseService;
            _syncService = syncService;
        }

        public async Task<UserModel?> GetCurrentUserAsync()
        {
            // Simple check: get the first user in the DB. 
            var users = await _databaseService.GetItemsAsync<UserModel>();
            return users.FirstOrDefault();
        }

        public async Task<string> ResolveTenantAsync(string companyCode)
        {
            // MOCK API Call
            await Task.Delay(500); // Simulate network

            return companyCode.ToLower() switch
            {
                "microsoft" => "common",
                "contoso" => "contoso-tenant-guid",
                "demo" => "demo-tenant-guid",
                _ => throw new Exception("Invalid Company Code")
            };
        }

        public async Task<UserModel> LoginWithMicrosoftAsync(string companyCode)
        {
            try
            {
                var tenantId = await ResolveTenantAsync(companyCode);
                var authority = $"https://login.microsoftonline.com/{tenantId}";

                var pca = PublicClientApplicationBuilder
                    .Create(ClientId)
                    .WithAuthority(authority)
                    .WithRedirectUri(RedirectUri)
#if ANDROID
                    .WithParentActivityOrWindow(() => Platform.CurrentActivity)
#endif
                    .Build();

                var result = await pca.AcquireTokenInteractive(Scopes)
                                      .ExecuteAsync();

                // Create User Model
                var user = new UserModel
                {
                    Email = result.Account.Username,
                    Name = result.Account.Username, // MSAL might give claims, simplified here
                    MicrosoftId = result.UniqueId, // Store Unique ID
                    CompanyCode = companyCode,
                    Company = "Derived from Tenant", // Placeholder
                    // Initialize other required fields to defaults
                    ExternalId = Guid.NewGuid().ToString(),
                    UserName = result.Account.Username
                };

                // Save to local DB
                await _databaseService.SaveItemAsync(user);

                // TRIGGER SYNC - PULL USER PROFILE
                await _syncService.SyncModuleAsync<UserModel>("users");

                return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MSAL Login Failed: {ex.Message}");
                throw;
            }
        }

        public async Task<UserModel> LoginWithStandardAsync(string companyCode, string email, string password)
        {
            // MOCK Standard Login
            await Task.Delay(500);

            if (password == "password")
            {
                var user = new UserModel
                {
                    Email = email,
                    Name = email.Split('@')[0],
                    CompanyCode = companyCode,
                    Company = "Standard Co.",
                    // Required fields
                    MicrosoftId = "N/A",
                    ExternalId = Guid.NewGuid().ToString(),
                    UserName = email
                };

                await _databaseService.SaveItemAsync(user);

                // TRIGGER SYNC - PULL USER PROFILE
                await _syncService.SyncModuleAsync<UserModel>("users");

                return user;
            }

            throw new UnauthorizedAccessException("Invalid credentials");
        }

        public async Task LogoutAsync()
        {
            // For now, just delete the local user record to "logout"
            var users = await _databaseService.GetItemsAsync<UserModel>();
            foreach (var user in users)
            {
                await _databaseService.DeleteItemAsync(user);
            }
        }
    }
}
