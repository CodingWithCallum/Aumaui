using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using SQLite;

namespace AumauiCL.Models.Checklists
{
    public class ChecklistTemplate : BaseAggregate, IEntity
    {
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private bool _isActive;

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

        public bool IsActive
        {
            get => _isActive;
            set => SetField(ref _isActive, value);
        }
    }
}
