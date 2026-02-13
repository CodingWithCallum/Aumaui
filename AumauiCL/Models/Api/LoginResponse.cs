namespace AumauiCL.Models.Api
{
    public class LoginUserData
    {
        public string? UserID { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Company { get; set; }
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
