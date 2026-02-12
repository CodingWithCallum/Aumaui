using System.ComponentModel;
using System.Runtime.CompilerServices;
using AumauiCL.Interfaces;
using AumauiCL.Models.Core;
using AumauiCL.Services.Api;
using AumauiCL.Services.Data;

namespace AumauiCL.Services.Sync
{
    public class SyncProgressData
    {
        public string Message { get; set; } = string.Empty;
        public double Percentage { get; set; }
        public bool IsBusy { get; set; }
    }

    public class SyncService : ISyncService
    {
        private readonly DatabaseService _databaseService;
        private readonly MockApiService _apiService;

        private double _syncProgress;
        private string _syncStatus = "Ready";

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<SyncProgressData>? OnProgress;

        public double SyncProgress
        {
            get => _syncProgress;
            private set
            {
                if (_syncProgress != value)
                {
                    _syncProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SyncStatus
        {
            get => _syncStatus;
            private set
            {
                if (_syncStatus != value)
                {
                    _syncStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public SyncService(DatabaseService databaseService, MockApiService apiService)
        {
            _databaseService = databaseService;
            _apiService = apiService;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyProgress(string message, double percentage, bool isBusy = true)
        {
            SyncStatus = message;
            SyncProgress = percentage;

            OnProgress?.Invoke(this, new SyncProgressData
            {
                Message = message,
                Percentage = percentage,
                IsBusy = isBusy
            });
        }

        public async Task ExecuteSyncAsync()
        {
            await SyncAllAsync();
        }

        public async Task SyncAllAsync()
        {
            NotifyProgress("Starting Full Sync...", 0);

            try
            {
                // Defined order of sync
                await SyncModuleAsync<Models.User.UserModel>("users");
                await SyncModuleAsync<Models.Audits.AuditAggregate>("audits");
                await SyncModuleAsync<Models.Incidents.IncidentAggregate>("incidents");
                await SyncModuleAsync<Models.Checklists.ChecklistAggregate>("checklists");

                NotifyProgress("Sync Completed Successfully", 100, false);
            }
            catch (Exception ex)
            {
                NotifyProgress($"Sync Failed: {ex.Message}", 0, false);
                // Log error
            }
        }

        public async Task SyncModuleAsync<T>(string endpointName) where T : class, IEntity, ISyncable, new()
        {
            NotifyProgress($"Syncing {endpointName}...", SyncProgress); // Keep current percentage or 0?

            // 1. PUSH: Get unsynced local items
            var unsyncedItems = await _databaseService.GetUnsyncedItemsAsync<T>();
            int pushedCount = 0;

            foreach (var item in unsyncedItems)
            {
                try
                {
                    // Push to API
                    var uploadedItem = await _apiService.PushItemAsync(endpointName, item);

                    // Mark as synced
                    item.SyncState.IsSynced = true;
                    item.SyncState.WasFailed = false;
                    item.SyncState.FailReason = string.Empty;

                    // Save local state
                    await _databaseService.SaveItemAsync(item);
                    pushedCount++;
                }
                catch (Exception ex)
                {
                    item.SyncState.WasFailed = true;
                    item.SyncState.FailReason = ex.Message;
                    await _databaseService.SaveItemAsync(item);
                }
            }

            // 2. PULL: Get latest from API
            // Note: In a real scenario, we might pass a 'LastSyncDate'
            var serverItems = await _apiService.GetItemsAsync<T>(endpointName);

            foreach (var serverItem in serverItems)
            {
                // Check if we have it locally
                var localItem = await _databaseService.GetItemAsync<T>(serverItem.ID);
                if (localItem != null)
                {
                    // Conflict resolution: Server Wins for now
                    // Copy properties from serverItem to localItem
                    // For simplicity in this mock, we just overwrite (upsert)
                    serverItem.SyncState.IsSynced = true;
                    await _databaseService.SaveItemAsync(serverItem);
                }
                else
                {
                    // New item
                    serverItem.SyncState.IsSynced = true;
                    await _databaseService.SaveItemAsync(serverItem);
                }
            }

            // Simple increment for demo purposes
            var newProgress = Math.Min(SyncProgress + 25, 100);
            NotifyProgress($"Synced {endpointName}: {pushedCount} pushed, {serverItems.Count} pulled.", newProgress);
        }
    }
}
