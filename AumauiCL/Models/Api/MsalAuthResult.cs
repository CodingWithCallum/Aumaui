namespace AumauiCL.Models.Api
{
    /// <summary>
    /// Lightweight DTO that decouples the app from MSAL's AuthenticationResult.
    /// Carries only the fields needed to call the API's Microsoft login endpoint.
    /// </summary>
    public class MsalAuthResult
    {
        /// <summary>Microsoft Object ID (oid claim)</summary>
        public string MicrosoftUserId { get; set; } = string.Empty;

        /// <summary>Microsoft account email (preferred_username claim)</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Microsoft account display name (name claim)</summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>Microsoft access token — sent to API for server-side validation via Graph API</summary>
        public string AccessToken { get; set; } = string.Empty;
    }
}
