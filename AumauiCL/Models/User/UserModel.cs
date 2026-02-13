using System.ComponentModel;
using System.Runtime.CompilerServices;
using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using AumauiCL.Models.User.Core;
using AumauiCL.Models.User.Extended;
using SQLite;
using Annotations = System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.User
{
    [SQLite.Table("Users")]
    public class UserModel : BaseAggregate, IEntity, ISyncable
    {
        #region Private Fields
        private int _id; // Backing field for ID
        private string _microsoftId = string.Empty;
        private string? _externalId;
        private string _userName = string.Empty;
        private string _title = string.Empty;
        private string _name = string.Empty;
        private string _jobTitle = string.Empty;
        private string _email = string.Empty;
        private string _telephone = string.Empty;
        private string _mobileNumber = string.Empty;
        private string _companyCode = string.Empty;
        private string _company = string.Empty;
        private string _division = string.Empty;
        private string _department = string.Empty;
        private string _team = string.Empty;
        private bool _canSignIn = true;
        private bool _isLockedOut = false;
        private bool _isTerminated = false;
        private string _roles = string.Empty;

        // Cached computed properties

        private UserIdentity? _identity;
        private UserContactInformation? _contactInfo;
        private UserOrganization? _organization;
        private UserAccountStatus? _accountStatus;
        #endregion

        #region Public Properties
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        // Identity
        [Unique]
        [Annotations.Required, Annotations.MaxLength(50)]
        public string MicrosoftId
        {
            get => _microsoftId;
            set => SetField(ref _microsoftId, value, nameof(Identity));
        }

        [Unique]
        [Annotations.Required, Annotations.MaxLength(50)]
        public string? ExternalId
        {
            get => _externalId;
            set => SetField(ref _externalId, value, nameof(Identity));
        }

        [Annotations.Required, Annotations.MaxLength(50)]
        public string UserName
        {
            get => _userName;
            set => SetField(ref _userName, value, nameof(Identity));
        }

        // Personal Information
        [Annotations.MaxLength(10)]
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        [Annotations.Required, Annotations.MaxLength(100)]
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        [Annotations.MaxLength(100)]
        public string JobTitle
        {
            get => _jobTitle;
            set => SetField(ref _jobTitle, value);
        }

        // Contact Information
        [Annotations.Required, Annotations.EmailAddress, Annotations.MaxLength(100)]
        [Indexed]
        public string Email
        {
            get => _email;
            set => SetField(ref _email, value, nameof(ContactInfo));
        }

        [Annotations.MaxLength(20)]
        public string Telephone
        {
            get => _telephone;
            set => SetField(ref _telephone, value, nameof(ContactInfo));
        }

        [Annotations.MaxLength(20)]
        public string MobileNumber
        {
            get => _mobileNumber;
            set => SetField(ref _mobileNumber, value, nameof(ContactInfo));
        }

        // Organization
        [Annotations.Required, Annotations.MaxLength(10)]
        public string CompanyCode
        {
            get => _companyCode;
            set => SetField(ref _companyCode, value, nameof(Organization));
        }

        [Indexed]
        public int CompanyID { get; set; }

        [Annotations.Required, Annotations.MaxLength(100)]
        public string Company
        {
            get => _company;
            set => SetField(ref _company, value, nameof(Organization));
        }

        [Indexed]
        public int DivisionID { get; set; }

        [Annotations.MaxLength(100)]
        public string Division
        {
            get => _division;
            set => SetField(ref _division, value, nameof(Organization));
        }

        [Indexed]
        public int DepartmentID { get; set; }

        [Annotations.MaxLength(100)]
        public string Department
        {
            get => _department;
            set => SetField(ref _department, value, nameof(Organization));
        }

        [Indexed]
        public int TeamID { get; set; }

        [Annotations.MaxLength(100)]
        public string Team
        {
            get => _team;
            set => SetField(ref _team, value, nameof(Organization));
        }

        // Account Status
        public bool CanSignIn
        {
            get => _canSignIn;
            set => SetField(ref _canSignIn, value, nameof(AccountStatus));
        }

        public bool IsLockedOut
        {
            get => _isLockedOut;
            set => SetField(ref _isLockedOut, value, nameof(AccountStatus));
        }

        public bool IsTerminated
        {
            get => _isTerminated;
            set => SetField(ref _isTerminated, value, nameof(AccountStatus));
        }

        // Legacy role support
        [Obsolete("Use UserRoles navigation property instead")]
        [Annotations.MaxLength(500)]
        public string Roles
        {
            get => _roles;
            set => SetField(ref _roles, value);
        }

        // Navigation Properties


        [Ignore]
        public List<UserRole> UserRoles { get; set; } = new();

        [Ignore]
        public UserIdentity Identity =>
            _identity ??= new UserIdentity
            {
                MicrosoftId = MicrosoftId,
                ExternalId = ExternalId,
                UserName = UserName
            };

        [Ignore]
        public UserContactInformation ContactInfo =>
            _contactInfo ??= new UserContactInformation
            {
                Email = Email,
                PhoneNumbers = new List<PhoneNumber>
                {
                    new(Telephone, PhoneType.Work, true),
                    new(MobileNumber, PhoneType.Mobile)
                }
            };

        [Ignore]
        public UserOrganization Organization =>
            _organization ??= new UserOrganization
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
        public UserAccountStatus AccountStatus =>
            _accountStatus ??= new UserAccountStatus
            {
                CanSignIn = CanSignIn,
                IsLockedOut = IsLockedOut,
                IsTerminated = IsTerminated
            };

        // Extended Models (loaded separately)
        [Ignore]
        public UserPreferences? Preferences { get; set; }

        [Ignore]
        public UserAudit? Audit { get; set; }
        #endregion

        #region Cache Management
        protected override void InvalidateComputedProperty(string? propertyName)
        {
            switch (propertyName)
            {
                case nameof(Identity):
                    _identity = null;
                    OnPropertyChanged(nameof(Identity));
                    break;
                case nameof(ContactInfo):
                    _contactInfo = null;
                    OnPropertyChanged(nameof(ContactInfo));
                    break;
                case nameof(Organization):
                    _organization = null;
                    OnPropertyChanged(nameof(Organization));
                    break;
                case nameof(AccountStatus):
                    _accountStatus = null;
                    OnPropertyChanged(nameof(AccountStatus));
                    break;
            }
        }
        #endregion

        #region Cache Management


        public void InvalidateAllCache()
        {
            _identity = null;
            _contactInfo = null;
            _organization = null;
            _accountStatus = null;
            OnPropertyChanged(nameof(Identity));
            OnPropertyChanged(nameof(ContactInfo));
            OnPropertyChanged(nameof(Organization));
            OnPropertyChanged(nameof(AccountStatus));
        }
        #endregion

        #region Helper Methods
        public void UpdateContactInfo(string email, string telephone, string mobile)
        {
            Email = email;
            Telephone = telephone;
            MobileNumber = mobile;
        }

        public void UpdateOrganization(string companyCode, int companyId, string company,
                                     int divisionId, string division, int departmentId,
                                     string department, int teamId, string team)
        {
            CompanyCode = companyCode;
            CompanyID = companyId;
            Company = company;
            DivisionID = divisionId;
            Division = division;
            DepartmentID = departmentId;
            Department = department;
            TeamID = teamId;
            Team = team;
        }

        public void UpdateAccountStatus(bool canSignIn, bool isLockedOut, bool isTerminated)
        {
            CanSignIn = canSignIn;
            IsLockedOut = isLockedOut;
            IsTerminated = isTerminated;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(MicrosoftId) &&
                   !string.IsNullOrEmpty(ExternalId) &&
                   !string.IsNullOrEmpty(UserName) &&
                   !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(Name);
        }
        #endregion
    }
}
