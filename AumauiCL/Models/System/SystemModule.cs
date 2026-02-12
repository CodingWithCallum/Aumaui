using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

namespace AumauiCL.Models.System
{
    public class SystemModule : BaseAggregate, IEntity
    {
        private int _id;
        private string _moduleName = string.Empty;
        private bool _isEnabled;
        private string _version = string.Empty;

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string ModuleName
        {
            get => _moduleName;
            set => SetField(ref _moduleName, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetField(ref _isEnabled, value);
        }

        public string Version
        {
            get => _version;
            set => SetField(ref _version, value);
        }
    }
}
