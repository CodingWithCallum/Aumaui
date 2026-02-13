using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

namespace AumauiCL.Models.Audits
{
    public class AuditTemplate : BaseAggregate, IEntity
    {
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _version = string.Empty;

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string? ExternalId { get; set; }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public string Version
        {
            get => _version;
            set => SetField(ref _version, value);
        }
    }
}
