using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace Aumaui.Platforms.Android
{
    /// <summary>
    /// Required by MSAL on Android to handle the browser redirect after Microsoft login.
    /// This activity receives the authentication callback from the system browser
    /// and passes it back to MSAL's token acquisition flow.
    /// 
    /// The DataScheme must match the redirect URI configured in Azure AD.
    /// TODO: Replace "msauth" and DataHost with your actual redirect URI components.
    ///       Example: if redirect URI is "msauth://com.companyname.aumaui/HASH",
    ///       then DataScheme = "msauth" and DataHost = "com.companyname.aumaui"
    /// </summary>
    [Activity(Exported = true)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataScheme = "msauth",
        DataHost = "com.companyname.aumaui")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}
