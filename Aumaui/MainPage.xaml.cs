using AumauiCL.Interfaces; // Add this
namespace Aumaui
{
    public partial class MainPage : ContentPage
    {
        private readonly IAuthService _authService;

        public MainPage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await VerifyAuthAsync();
        }

        private async Task VerifyAuthAsync()
        {
            Console.WriteLine("--- START AUTH VERIFICATION ---");
            try
            {
                // 1. Tenant Resolution
                var tenant = await _authService.ResolveTenantAsync("Contoso");
                Console.WriteLine($"Resolved Tenant: {tenant}");

                // 2. Standard Login
                var user = await _authService.LoginWithStandardAsync("Contoso", "test@test.com", "password");
                Console.WriteLine($"Logged in user: {user.Email}, Token: {user.AccessToken}");

                // 3. Check Persistence
                var storedUser = await _authService.GetCurrentUserAsync();
                if (storedUser?.Email == user.Email)
                {
                    Console.WriteLine("Persistence Verified: User found in DB.");
                }
                else
                {
                    Console.WriteLine("Persistence Failed: User not found.");
                }

                // 4. Logout
                await _authService.LogoutAsync();
                var clearedUser = await _authService.GetCurrentUserAsync();
                if (clearedUser == null)
                {
                    Console.WriteLine("Logout Verified: User cleared.");
                }
                else
                {
                    Console.WriteLine("Logout Failed: User still exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Verification Failed: {ex}");
            }
            Console.WriteLine("--- END AUTH VERIFICATION ---");
        }
    }
}
