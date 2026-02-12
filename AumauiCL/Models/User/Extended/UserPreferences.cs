using SQLite;
using Annotation = System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.User.Extended // FIX: Consistent casing
{
    public class UserPreferences
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Annotation.Required, Indexed]
        public int UserID { get; set; }

        [Annotation.MaxLength(20)]
        public string Theme { get; set; } = "System";

        [Annotation.MaxLength(10)]
        public string Language { get; set; } = "en-US";

        [Annotation.MaxLength(50)]
        public string TimeZone { get; set; } = "UTC";

        public bool NotificationsEnabled { get; set; } = true;

        [Annotation.MaxLength(20)]
        public string DateFormat { get; set; } = "yyyy-MM-dd";

        // ADD: MAUI-specific preferences
        public bool OfflineModeEnabled { get; set; } = true;
        public int SyncFrequencyMinutes { get; set; } = 30;
        public bool BiometricAuthEnabled { get; set; } = false;
        public string PreferredFontSize { get; set; } = "Medium";

        // ADD: Validation methods
        public bool IsValidTheme() => Theme is "Light" or "Dark" or "System";

        public bool IsValidLanguage() => Language.Length >= 2 && Language.Contains('-');

        // ADD: Helper for datetime formatting
        [Ignore]
        public string FormatDateTime(DateTime dateTime) => dateTime.ToString(DateFormat);

        // ADD: Update method
        public void UpdateLastChanged()
        {
            // Add LastModified tracking if needed
        }
    }
}
