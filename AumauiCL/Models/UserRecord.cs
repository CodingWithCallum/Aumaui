using SQLite;

namespace MyMauiApp.Models;

public class UserRecord
{
    [PrimaryKey]
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty; // Useful for Microsoft SSO
    public string CompanyCode { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTime LastLogin { get; set; }
}