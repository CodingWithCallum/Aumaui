using System.ComponentModel;

namespace AumauiCL.Interfaces;

public interface ISyncService : INotifyPropertyChanged
{
    double SyncProgress { get; }
    string SyncStatus { get; }
    Task ExecuteSyncAsync();
    Task SyncModuleAsync<T>(string endpointName) where T : class, IEntity, ISyncable, new();

    /// <summary>Returns the UTC timestamp of the last successful full sync, or null if never synced.</summary>
    Task<DateTime?> GetLastSyncDateAsync();

    /// <summary>Returns true if the last sync is older than <paramref name="maxAge"/> or has never occurred.</summary>
    Task<bool> ShouldSyncAsync(TimeSpan maxAge);
}
