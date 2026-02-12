using SQLite;
using Annotation = System.ComponentModel.DataAnnotations;

namespace Client_MAUI_CL.Models.TestingFolder.User.Core
{
    public class UserRole
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Annotation.Required]
        public int UserID { get; set; }

        [Annotation.Required, Annotation.MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [Annotation.MaxLength(200)]
        public string RoleDescription { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;

        // ADD: Role validation
        [Ignore]
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

        [Ignore]
        public bool IsCurrentlyActive => IsActive && !IsExpired;

        // ADD: Common role checks
        [Ignore]
        public bool IsAdminRole => RoleName.Contains("Admin", StringComparison.OrdinalIgnoreCase);

        [Ignore]
        public bool IsManagerRole => RoleName.Contains("Manager", StringComparison.OrdinalIgnoreCase);
    }
}
