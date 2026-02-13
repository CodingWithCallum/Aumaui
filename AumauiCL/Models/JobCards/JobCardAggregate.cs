using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

namespace AumauiCL.Models.JobCards
{
    public class JobCardAggregate : BaseAggregate, IEntity, ISyncable
    {
        private int _id;
        private string _jobNumber = string.Empty;
        private string _description = string.Empty;
        private string _status = string.Empty;
        private DateTime _dueDate;

        // Context
        private int _companyID;
        private int _divisionID;
        private int _departmentID;
        private int _teamID;

        // Cache

        private OrganizationContext? _context;
        private JobCardConfiguration? _configuration;

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string? ExternalId { get; set; }

        public string JobNumber
        {
            get => _jobNumber;
            set => SetField(ref _jobNumber, value);
        }

        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public string Status
        {
            get => _status;
            set => SetField(ref _status, value);
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set => SetField(ref _dueDate, value);
        }

        public int CompanyID
        {
            get => _companyID;
            set => SetField(ref _companyID, value, nameof(OrganizationContext));
        }

        public int DivisionID
        {
            get => _divisionID;
            set => SetField(ref _divisionID, value, nameof(OrganizationContext));
        }

        public int DepartmentID
        {
            get => _departmentID;
            set => SetField(ref _departmentID, value, nameof(OrganizationContext));
        }

        public int TeamID
        {
            get => _teamID;
            set => SetField(ref _teamID, value, nameof(OrganizationContext));
        }



        [Ignore]
        public OrganizationContext Context => _context ??= new OrganizationContext
        {
            CompanyID = CompanyID,
            DivisionID = DivisionID,
            DepartmentID = DepartmentID,
            TeamID = TeamID
        };

        [Ignore]
        public JobCardConfiguration Configuration => _configuration ??= new JobCardConfiguration();

        protected override void InvalidateComputedProperty(string? propertyName)
        {
            if (propertyName == nameof(OrganizationContext))
            {
                _context = null;
                OnPropertyChanged(nameof(OrganizationContext));
            }
        }
    }
}
