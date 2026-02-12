using AumauiCL.Models.Core;

namespace AumauiCL.Interfaces
{
    public interface ISyncable
    {
        SyncState SyncState { get; }
    }
}
