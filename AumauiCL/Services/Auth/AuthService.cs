using System.Diagnostics;
using AumauiCL.Interfaces;
using Microsoft.Identity.Client;
using MyMauiApp.Models;
using MyMauiApp.Services.Data;

namespace AumauiCL.Services.Auth;

public class AuthService : IAuthService
{
    private readonly DatabaseService _databaseService;

    // MSAL Configuration
    // NOTE: In a real app, these should be in configuration/constants
    private const string ClientId = "YOUR_CLIENT_ID_HERE"; // Placeholder
    private const string RedirectUri = "msauth://com.companyname.aumaui"; // Placeholder
    private string[] Scopes = new string[] { "User.Read" };

    public AuthService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<UserRecord?> GetCurrentUserAsync()
    {
        // Simple check: get the first user in the DB. 
        // In a multi-user local scenario, you'd handle this differently.
        var users = await _databaseService.GetItemsAsync<UserRecord>();
        return users.FirstOrDefault();
    }

    public async Task<string> ResolveTenantAsync(string companyCode)
    {
        // MOCK API Call
        // In reality, this would hit https://api.aumaui.com/resolve-tenant?code={companyCode}
        await Task.Delay(500); // Simulate network

        return companyCode.ToLower() switch
        {
            "microsoft" => "common", // or specific tenant GUID
            "contoso" => "contoso-tenant-guid",
            "demo" => "demo-tenant-guid",
            _ => throw new Exception("Invalid Company Code")
        };
    }

    public async Task<UserRecord> LoginWithMicrosoftAsync(string companyCode)
    {
        try
        {
            var tenantId = await ResolveTenantAsync(companyCode);

            // Dynamic Authority based on Tenant
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

            // Create User Record
            var user = new UserRecord
            {
                Email = result.Account.Username,
                DisplayName = result.Account.Username, // MSAL might give claims, simplified here
                TenantId = tenantId,
                CompanyCode = companyCode,
                AccessToken = result.AccessToken,
                IdToken = result.IdToken,
                ExpiresAt = result.ExpiresOn,
                LastLogin = DateTime.UtcNow
            };

            await _databaseService.UpsertItemAsync(user);
            return user;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"MSAL Login Failed: {ex.Message}");
            throw;
        }
    }

    public async Task<UserRecord> LoginWithStandardAsync(string companyCode, string email, string password)
    {
        // MOCK Standard Login
        await Task.Delay(500);

        if (password == "password") // Secure, I know.
        {
            var user = new UserRecord
            {
                Email = email,
                DisplayName = email.Split('@')[0],
                CompanyCode = companyCode,
                LastLogin = DateTime.UtcNow,
                TenantId = "standard-auth",
                AccessToken = "mock-access-token"
            };

            await _databaseService.UpsertItemAsync(user);
            return user;
        }

        throw new UnauthorizedAccessException("Invalid credentials");
    }

    public async Task LogoutAsync()
    {
        await _databaseService.PurgeSensitiveDataAsync();
    }
}
