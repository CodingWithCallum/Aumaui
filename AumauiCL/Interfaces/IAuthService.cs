using MyMauiApp.Models;

namespace AumauiCL.Interfaces
{
    public interface IAuthService
    {
        Task<UserRecord?> GetCurrentUserAsync();
        Task<string> ResolveTenantAsync(string companyCode);
        Task<UserRecord> LoginWithMicrosoftAsync(string companyCode);
        Task<UserRecord> LoginWithStandardAsync(string companyCode, string email, string password);
        Task LogoutAsync();
    }
}
