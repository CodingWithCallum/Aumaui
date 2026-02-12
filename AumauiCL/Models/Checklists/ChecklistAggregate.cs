using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

namespace AumauiCL.Models.Checklists
{
    public class ChecklistAggregate : BaseAggregate, IEntity, ISyncable
    {
        private int _id;
        private string _name = string.Empty;
        private bool _isCompleted;
        private DateTime? _completedDate;

        // Context
        private int _companyID;
        private int _divisionID;
        private int _departmentID;
        private int _teamID;

        // Cache
        private SyncState? _syncState;
        private OrganizationContext? _context;
        private ChecklistTemplate? _template;

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetField(ref _isCompleted, value);
        }

        public DateTime? CompletedDate
        {
            get => _completedDate;
            set => SetField(ref _completedDate, value);
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
        public SyncState SyncState => _syncState ??= new SyncState();

        [Ignore]
        public OrganizationContext Context => _context ??= new OrganizationContext
        {
            CompanyID = CompanyID,
            DivisionID = DivisionID,
            DepartmentID = DepartmentID,
            TeamID = TeamID
        };

        [Ignore]
        public ChecklistTemplate Template => _template ??= new ChecklistTemplate();

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
