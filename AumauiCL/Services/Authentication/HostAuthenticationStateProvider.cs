using System.Security.Claims;
using AumauiCL.Interfaces;
using AumauiCL.Models.User;
using Microsoft.AspNetCore.Components.Authorization;

namespace AumauiCL.Services.Authentication;

/// <summary>
/// Custom Blazor authentication state provider.
/// On cold start, rehydrates the ClaimsPrincipal from SecureStorage + SQLite
/// so the user stays logged in across app restarts.
/// </summary>
public class HostAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ISecureStorageService _secureStorage;
    private readonly IDatabaseService _databaseService;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
    private bool _initialized;

    public HostAuthenticationStateProvider(
        ISecureStorageService secureStorage,
        IDatabaseService databaseService)
    {
        _secureStorage = secureStorage;
        _databaseService = databaseService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!_initialized)
        {
            _initialized = true;
            var token = await _secureStorage.GetAsync("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                var users = await _databaseService.GetItemsAsync<UserModel>();
                var user = users.FirstOrDefault();
                if (user != null)
                {
                    var identity = new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.Name, user.Email) }, "CustomAuth");
                    _currentUser = new ClaimsPrincipal(identity);
                }
            }
        }
        return new AuthenticationState(_currentUser);
    }

    public void MarkUserAsAuthenticated(string email)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) }, "CustomAuth");
        _currentUser = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void MarkUserAsLoggedOut()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
