using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MyMauiApp.Services.Auth;

// Primary Constructor (C# 14 style)
// This class provides a custom authentication state provider for Blazor applications.

public class HostAuthenticationStateProvider() : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(_currentUser));

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