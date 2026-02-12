using SQLite;
using Annotation = System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.User.Extended
{
    public class UserAudit
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Annotation.Required, Indexed] // ADD: Index for faster queries
        public int UserID { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        [Annotation.MaxLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        [Annotation.MaxLength(100)]
        public string ModifiedBy { get; set; } = string.Empty;

        // MAUI-specific tracking
        public DateTime? LastSyncDate { get; set; }
        public string? DeviceId { get; set; }
        public string? AppVersion { get; set; }

        // Computed properties for UI
        [Ignore]
        public TimeSpan? TimeSinceLastLogin => LastLoginDate.HasValue ?
            DateTime.UtcNow - LastLoginDate.Value : null;

        [Ignore]
        public string LastLoginDisplay => LastLoginDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never";

        // Update tracking
        public void MarkAsModified(string modifiedBy)
        {
            LastModifiedDate = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}
