using AumauiCL.Models.Core;

namespace AumauiCL.Interfaces
{
    public interface ISyncable
    {
        bool IsSynced { get; set; }
        bool WasFailed { get; set; }
        string FailReason { get; set; }
    }
}
