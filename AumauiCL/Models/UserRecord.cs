using SQLite;

namespace MyMauiApp.Models;

public class UserRecord
{
    [PrimaryKey]
    public int ID { get; set; } // Primary key for SQLite
    [Unique]
    public string MicrosoftID { get; set; } = string.Empty; // Unique identifier from Microsoft SSO
    [Unique]
    public string XsysID { get; set; } = string.Empty; // Unique identifier from tenant's system

    public int CompanyID { get; set; } // Unique identifier for the company, useful for multi-tenant scenarios
    public string DisplayName { get; set; } = string.Empty; 
    public string TenantId { get; set; } = string.Empty; // Useful for Microsoft SSO
    public string CompanyCode { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTime LastLogin { get; set; }
}