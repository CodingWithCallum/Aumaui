using SQLite;

namespace AumauiCL.Models.User.Extended
{
    public class UserPreferences
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int UserID { get; set; }
        public string Theme { get; set; } = "System";
        public string Language { get; set; } = "en-US";
        public string TimeZone { get; set; } = "UTC";
        public bool NotificationsEnabled { get; set; } = true;
        public string DateFormat { get; set; } = "yyyy-MM-dd";
    }
}