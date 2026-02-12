using SQLite;
using Client_MAUI_CL.Models.User.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace AumauiCL.Models.User
{
    public class UserModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        // Identity
        [Unique]
        public string MicrosoftId { get; set; }

        [Unique]
        public string ExternalId { get; set; }

        public string UserName { get; set; }

        // Personal Information
        public string Title { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }

        // Contact Information (embedded)
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string MobileNumber { get; set; }

        // Organization (embedded)
        public string CompanyCode { get; set; }
        public int CompanyID { get; set; }
        public string Company { get; set; }
        public int DivisionID { get; set; }
        public string Division { get; set; }
        public int DepartmentID { get; set; }
        public string Department { get; set; }
        public int TeamID { get; set; }
        public string Team { get; set; }

        // Account Status
        public bool CanSignIn { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsTerminated { get; set; }

        // Legacy role support
        [Obsolete("Use UserRoles navigation property instead")]
        public string Roles { get; set; }

        // Navigation Properties (for future expansion)
        [Ignore]
        public List<UserRole> UserRoles { get; set; } = new();

        [Ignore]
        public UserIdentity Identity => new()
        {
            ID = ID,
            MicrosoftId = MicrosoftId,
            ExternalId = ExternalId,
            UserName = UserName
        };

        [Ignore]
        public UserContactInformation ContactInfo => new()
        {
            Email = Email,
            Telephone = Telephone,
            MobileNumber = MobileNumber
        };

        [Ignore]
        public UserOrganization Organization => new()
        {
            CompanyCode = CompanyCode,
            CompanyID = CompanyID,
            Company = Company,
            DivisionID = DivisionID,
            Division = Division,
            DepartmentID = DepartmentID,
            Department = Department,
            TeamID = TeamID,
            Team = Team
        };

        [Ignore]
        public UserAccountStatus AccountStatus => new()
        {
            CanSignIn = CanSignIn,
            IsLockedOut = IsLockedOut,
            IsTerminated = IsTerminated
        };

        // Extended Models
        [Ignore]
        public UserPreferences Preferences { get; set; } = new();

        [Ignore]
        public UserAudit Audit { get; set; } = new();
    }
}