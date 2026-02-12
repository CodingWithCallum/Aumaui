using AumauiCL.Interfaces;

namespace AumauiCL.Services.Api
{
    public class MockApiService
    {
        // Simulate network delay
        private const int DelayMs = 800;

        public async Task<List<T>> GetItemsAsync<T>(string endpoint) where T : new()
        {
            await Task.Delay(DelayMs);

            // Return empty list by default, or simulate data based on Type name
            return new List<T>();
        }

        public async Task<T> PushItemAsync<T>(string endpoint, T item) where T : class
        {
            await Task.Delay(DelayMs);

            // Echo back the item, effectively "saving" it
            return item;
        }

        // Specific simulation for User Profile
        public async Task<Models.User.UserModel> GetUserProfileAsync()
        {
            await Task.Delay(DelayMs);
            return new Models.User.UserModel
            {
                Name = "Simulated User",
                Email = "user@example.com",
                Company = "Simulated Co.",
                CompanyID = 1,
                // Simulate existing ID from server
                ID = 999
            };
        }
    }
}
