using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

namespace AumauiCL.Models.RiskAssessments
{
    public class RiskAssessmentAggregate : BaseAggregate, IEntity, ISyncable
    {
        private int _id;
        private string _title = string.Empty;
        private string _riskLevel = string.Empty;
        private DateTime _assessmentDate;

        // Context
        private int _companyID;
        private int _divisionID;
        private int _departmentID;
        private int _teamID;

        // Cache

        private OrganizationContext? _context;

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string? ExternalId { get; set; }

        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        public string RiskLevel
        {
            get => _riskLevel;
            set => SetField(ref _riskLevel, value);
        }

        public DateTime AssessmentDate
        {
            get => _assessmentDate;
            set => SetField(ref _assessmentDate, value);
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
