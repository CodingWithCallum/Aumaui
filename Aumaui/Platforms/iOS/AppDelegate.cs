using Foundation;
using Microsoft.Identity.Client;
using UIKit;

namespace Aumaui
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        /// <summary>
        /// Forwards the authentication redirect back to MSAL after the browser callback.
        /// Required for the interactive Microsoft login flow on iOS.
        /// </summary>
        public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
        {
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return base.OpenUrl(application, url, options);
        }
    }
}
