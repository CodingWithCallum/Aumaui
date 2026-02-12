using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

namespace AumauiCL.Models.Audits
{
    public class AuditAggregate : BaseAggregate, IEntity, ISyncable
    {
        #region Private Fields
        private int _id;
        private string _reference = string.Empty;
        private DateTime _scheduledDate;
        private DateTime? _completedDate;

        // Context Fields (for OrganizationContext)
        private int _companyID;
        private int _divisionID;
        private int _departmentID;
        private int _teamID;

        // Cache
        private SyncState? _syncState;
        private OrganizationContext? _context;
        private AuditConfiguration? _configuration;
        private AuditStatus? _status;
        private AuditTemplate? _template;
        private AuditMetadata? _metadata;
        #endregion

        #region Public Properties
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string Reference
        {
            get => _reference;
            set => SetField(ref _reference, value);
        }

        public DateTime ScheduledDate
        {
            get => _scheduledDate;
            set => SetField(ref _scheduledDate, value);
        }

        public DateTime? CompletedDate
        {
            get => _completedDate;
            set => SetField(ref _completedDate, value);
        }

        // Organization Context Properties
        public int CompanyID
        {
            get => _companyID;
            set => SetField(ref _companyID, value, nameof(Context));
        }

        public int DivisionID
        {
            get => _divisionID;
            set => SetField(ref _divisionID, value, nameof(Context));
        }

        public int DepartmentID
        {
            get => _departmentID;
            set => SetField(ref _departmentID, value, nameof(Context));
        }

        public int TeamID
        {
            get => _teamID;
            set => SetField(ref _teamID, value, nameof(Context));
        }

        // Navigation / Value Objects
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
        public AuditConfiguration Configuration => _configuration ??= new AuditConfiguration();

        [Ignore]
        public AuditStatus Status => _status ??= new AuditStatus();

        [Ignore]
        public AuditTemplate Template => _template ??= new AuditTemplate();

        [Ignore]
        public AuditMetadata Metadata => _metadata ??= new AuditMetadata
        {
            CreatedDate = DateTime.UtcNow // Placeholder logic
        };
        #endregion

        #region Cache Management
        protected override void InvalidateComputedProperty(string? propertyName)
        {
            switch (propertyName)
            {
                case nameof(Context):
                    _context = null;
                    OnPropertyChanged(nameof(Context));
                    break;
                case nameof(Metadata):
                    _metadata = null;
                    OnPropertyChanged(nameof(Metadata));
                    break;
            }
        }
        #endregion
    }
}
