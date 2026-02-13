using AumauiCL.Models.User;

namespace AumauiCL.Interfaces
{
    public interface IAuthService
    {
        Task<UserModel?> GetCurrentUserAsync();
        Task<UserModel> LoginWithMicrosoftAsync(string companyCode);
        Task<UserModel> LoginWithStandardAsync(string companyCode, string email, string password);
        Task LogoutAsync();
        Task<bool> TryRefreshTokenAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}
