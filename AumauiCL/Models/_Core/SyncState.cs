namespace AumauiCL.Models.Core
{
    public class SyncState
    {
        public bool IsSynced { get; set; }
        public bool WasFailed { get; set; }
        public string FailReason { get; set; } = string.Empty;
    }
}
