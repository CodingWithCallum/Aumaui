using System.Text.Json.Serialization;

namespace AumauiCL.Models.Api
{
    public class LoginUserData
    {
        public string? SHEQsysUserId { get; set; }
        public string? ADUserId { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public int? CompanyID { get; set; }  // Changed to int
        public string? Company { get; set; }
        public string? CompanyCode { get; set; }
        public int? DivisionID { get; set; }  // Changed to int
        public string? Division { get; set; }
        public int? DepartmentID { get; set; }  // Changed to int
        public string? Department { get; set; }
        public int? TeamID { get; set; }  // Changed to int
        public string? Team { get; set; }
        public string? JobTitle { get; set; }
        public string? Title { get; set; }
        public string? Telephone { get; set; }
        public string? MobileNumber { get; set; }
        public bool CanSignIn { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsTerminated { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsUmbani { get; set; }
        public bool RestrictedAccess_Artisan { get; set; }

        // Keep the old UserID property for backward compatibility
        // This can map to SHEQsysUserId
        public string? UserID => SHEQsysUserId;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Message { get; set; }
        public LoginUserData? User { get; set; }
    }
}