using AumauiCL.Models.Core;

namespace AumauiCL.Models.Incidents
{
    public class IncidentConfiguration : BaseAggregate
    {
        private bool _allowAnonymousReporting;
        private int _defaultSeverityLevel;
        private string _notificationEmail = string.Empty;

        public bool AllowAnonymousReporting
        {
            get => _allowAnonymousReporting;
            set => SetField(ref _allowAnonymousReporting, value);
        }

        public int DefaultSeverityLevel
        {
            get => _defaultSeverityLevel;
            set => SetField(ref _defaultSeverityLevel, value);
        }

        public string NotificationEmail
        {
            get => _notificationEmail;
            set => SetField(ref _notificationEmail, value);
        }
    }
}
