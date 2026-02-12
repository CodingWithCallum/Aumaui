using SQLite;

namespace AumauiCL.Models.User.Extended
{
    public class UserAudit
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int UserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}