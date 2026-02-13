using SQLite;
using Annotation = System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.User.Core
{
    public class UserRole : AumauiCL.Interfaces.IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string? ExternalId { get; set; }

        [Annotation.Required]
        public int UserID { get; set; }

        [Annotation.Required, Annotation.MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [Annotation.MaxLength(200)]
        public string RoleDescription { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Role validation
        [Ignore]
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

        [Ignore]
        public bool IsCurrentlyActive => IsActive && !IsExpired;

        // Common role checks
        [Ignore]
        public bool IsAdminRole => RoleName.Contains("Admin", StringComparison.OrdinalIgnoreCase);

        [Ignore]
        public bool IsManagerRole => RoleName.Contains("Manager", StringComparison.OrdinalIgnoreCase);
    }
}
