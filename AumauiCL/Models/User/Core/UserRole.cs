using SQLite;

namespace AumauiCL.Models.User.Core
{
    public class UserRole
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int UserID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; }
    }
}