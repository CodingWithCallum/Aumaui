using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

namespace AumauiCL.Models.Attachments
{
    public class Attachment : BaseAggregate, IEntity, ISyncable
    {
        private int _id;
        private string _fileName = string.Empty;
        private string _filePath = string.Empty;
        private string _fileType = string.Empty;
        private long _fileSize;
        private DateTime _createdDate;

        // Cache
        private SyncState? _syncState;
        private AuditMetadata? _metadata;

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string FileName
        {
            get => _fileName;
            set => SetField(ref _fileName, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetField(ref _filePath, value);
        }

        public string FileType
        {
            get => _fileType;
            set => SetField(ref _fileType, value);
        }

        public long FileSize
        {
            get => _fileSize;
            set => SetField(ref _fileSize, value);
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetField(ref _createdDate, value, nameof(Metadata));
        }

        [Ignore]
        public SyncState SyncState => _syncState ??= new SyncState();

        [Ignore]
        public AuditMetadata Metadata => _metadata ??= new AuditMetadata
        {
            CreatedDate = CreatedDate
            // Populate other fields as needed
        };

        protected override void InvalidateComputedProperty(string? propertyName)
        {
            if (propertyName == nameof(Metadata))
            {
                _metadata = null;
                OnPropertyChanged(nameof(Metadata));
            }
        }
    }
}
