using AumauiCL.Models.Core;

namespace AumauiCL.Models.Audits
{
    public class AuditConfiguration : BaseAggregate
    {
        private bool _isEnabled;
        private string _defaultAssignee = string.Empty;
        private int _daysToComplete;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetField(ref _isEnabled, value);
        }

        public string DefaultAssignee
        {
            get => _defaultAssignee;
            set => SetField(ref _defaultAssignee, value);
        }

        public int DaysToComplete
        {
            get => _daysToComplete;
            set => SetField(ref _daysToComplete, value);
        }
    }
}
