using System.ComponentModel;

namespace AumauiCL.Interfaces;

public interface ISyncService : INotifyPropertyChanged
{
    double SyncProgress { get; }
    string SyncStatus { get; }
    Task ExecuteSyncAsync();
}
