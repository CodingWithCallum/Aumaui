using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

using Annotations = System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.Incidents
{
    public class IncidentAggregate : BaseAggregate, IEntity, ISyncable
    {
        private int _id;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private int _severity;
        private DateTime _incidentDate;

        // Context
        private int _companyID;
        private int _divisionID;
        private int _departmentID;
        private int _teamID;

        // Cache

        private OrganizationContext? _context;
        private IncidentConfiguration? _configuration;

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string? ExternalId { get; set; }

        [Annotations.Required, Annotations.MaxLength(100)]
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        [Annotations.MaxLength(500)]
        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public int Severity
        {
            get => _severity;
            set => SetField(ref _severity, value);
        }

        public DateTime IncidentDate
        {
            get => _incidentDate;
            set => SetField(ref _incidentDate, value);
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
        public IncidentConfiguration Configuration => _configuration ??= new IncidentConfiguration();

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
