using System.Diagnostics;
using AumauiCL.Interfaces;
using AumauiCL.Models.Api;
using Microsoft.Identity.Client;

namespace AumauiCL.Services.Auth
{
    /// <summary>
    /// Singleton wrapper around MSAL's IPublicClientApplication.
    /// Provides interactive login, silent token acquisition, and sign-out.
    /// 
    /// The PCA instance is built once at construction and reused for the app's lifetime,
    /// which enables MSAL's built-in token cache to work across login attempts.
    /// </summary>
    public class MsalService : IMsalService
    {
        private readonly IPublicClientApplication _pca;
        private IAccount? _cachedAccount;

        // ─── MSAL Configuration ──────────────────────────────────────────
        // TODO: Replace these placeholders with your Azure AD app registration values.
        //
        // ClientId:    The Application (client) ID from Azure AD → App registrations.
        //              Example: "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
        //
        // RedirectUri: Must match what's configured in Azure AD → Authentication → Redirect URIs.
        //              Android example: "msauth://com.companyname.aumaui/HASH_FROM_KEYTOOL"
        //              iOS example:     "msauth.com.companyname.aumaui://auth"
        //              Windows example: "https://login.microsoftonline.com/common/oauth2/nativeclient"
        //
        // Scopes:      The permissions your app requests. "User.Read" gives basic profile access.
        //              Add more as needed, e.g. "email", "profile", "offline_access"
        //
        // TenantId:    Using "common" allows any Microsoft account (multi-tenant).
        //              If you want to restrict to a specific org, use the tenant GUID.
        //              Example: "f1e2d3c4-b5a6-7890-cdef-1234567890ab"
        // ─────────────────────────────────────────────────────────────────

        private const string ClientId = "YOUR_CLIENT_ID_HERE";
        private const string Authority = "https://login.microsoftonline.com/common";

        // Android: "msauth://com.companyname.aumaui/HASH"
        // iOS:     "msauth.com.companyname.aumaui://auth"
        private const string RedirectUri = "msauth://com.companyname.aumaui";

        private static readonly string[] Scopes = { "User.Read" };

        public MsalService()
        {
            var builder = PublicClientApplicationBuilder
                .Create(ClientId)
                .WithAuthority(Authority)
                .WithRedirectUri(RedirectUri);

            // Platform-specific configuration
#if ANDROID
            builder = builder.WithParentActivityOrWindow(() => Platform.CurrentActivity);
#elif IOS
            builder = builder.WithIosKeychainSecurityGroup("com.companyname.aumaui");
#endif

            _pca = builder.Build();
        }

        public bool HasCachedAccount => _cachedAccount != null;

        public async Task<MsalAuthResult> SignInInteractiveAsync()
        {
            try
            {
                var result = await _pca
                    .AcquireTokenInteractive(Scopes)
                    .ExecuteAsync();

                _cachedAccount = result.Account;

                return MapToResult(result);
            }
            catch (MsalClientException ex) when (ex.ErrorCode == "authentication_canceled")
            {
                throw new OperationCanceledException("User canceled Microsoft sign-in.", ex);
            }
        }

        public async Task<MsalAuthResult?> TrySignInSilentAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();
                var account = accounts.FirstOrDefault();

                if (account == null)
                    return null;

                var result = await _pca
                    .AcquireTokenSilent(Scopes, account)
                    .ExecuteAsync();

                _cachedAccount = result.Account;

                return MapToResult(result);
            }
            catch (MsalUiRequiredException)
            {
                // Token expired and can't be silently refreshed — caller should use interactive
                Debug.WriteLine("Silent token acquisition failed — interactive login required.");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Silent auth failed: {ex.Message}");
                return null;
            }
        }

        public async Task SignOutAsync()
        {
            var accounts = await _pca.GetAccountsAsync();
            foreach (var account in accounts)
            {
                await _pca.RemoveAsync(account);
            }
            _cachedAccount = null;
        }

        // ─── Private Helpers ─────────────────────────────────────────────

        private static MsalAuthResult MapToResult(AuthenticationResult result) => new()
        {
            MicrosoftUserId = result.UniqueId,
            Email = result.Account.Username,
            DisplayName = result.ClaimsPrincipal?.FindFirst("name")?.Value
                          ?? result.Account.Username,
            AccessToken = result.AccessToken
        };
    }
}
