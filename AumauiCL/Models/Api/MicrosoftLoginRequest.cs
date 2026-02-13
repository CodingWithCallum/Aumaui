namespace AumauiCL.Models.Api
{
    public class MicrosoftLoginRequest
    {
        public string MicrosoftUserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}
