using AumauiCL.Models.Core;

namespace AumauiCL.Models.JobCards
{
    public class JobCardConfiguration : BaseAggregate
    {
        private bool _requireUpdates;
        private int _defaultDurationMinutes;

        public bool RequireUpdates
        {
            get => _requireUpdates;
            set => SetField(ref _requireUpdates, value);
        }

        public int DefaultDurationMinutes
        {
            get => _defaultDurationMinutes;
            set => SetField(ref _defaultDurationMinutes, value);
        }
    }
}
