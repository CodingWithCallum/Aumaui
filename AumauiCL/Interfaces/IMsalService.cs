using AumauiCL.Models.Api;

namespace AumauiCL.Interfaces
{
    /// <summary>
    /// Abstracts MSAL (Microsoft Authentication Library) operations.
    /// Handles interactive login, silent token acquisition, sign-out, and cached account checks.
    /// </summary>
    public interface IMsalService
    {
        /// <summary>
        /// Launches the interactive Microsoft login flow (browser popup).
        /// </summary>
        Task<MsalAuthResult> SignInInteractiveAsync();

        /// <summary>
        /// Attempts to silently acquire a token using a cached account.
        /// Returns null if no cached account exists or silent auth fails.
        /// </summary>
        Task<MsalAuthResult?> TrySignInSilentAsync();

        /// <summary>
        /// Signs out all cached MSAL accounts.
        /// </summary>
        Task SignOutAsync();

        /// <summary>
        /// True if there is at least one cached MSAL account from a previous login.
        /// Call <see cref="TrySignInSilentAsync"/> to attempt silent re-auth.
        /// </summary>
        bool HasCachedAccount { get; }
    }
}
