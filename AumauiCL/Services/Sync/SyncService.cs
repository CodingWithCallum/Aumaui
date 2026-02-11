using AumauiCL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using MyMauiApp.Models;
using MyMauiApp.Services.Api;
using MyMauiApp.Services.Data;

namespace MyMauiApp.Services.Sync;

public partial class SyncService : ObservableObject, ISyncService
{
    private readonly IApiService _apiService;
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private double _syncProgress;

    [ObservableProperty]
    private string _syncStatus = "Initialized";

    public SyncService(IApiService apiService, DatabaseService databaseService)
    {
        _apiService = apiService;
        _databaseService = databaseService;
    }

    public async Task ExecuteSyncAsync()
    {
        try
        {
            SyncProgress = 0;
            SyncStatus = "Connecting to API...";

            // Step 1: Fetch Data
            var items = await _apiService.GetBusinessItemsAsync();

            SyncProgress = 50;
            SyncStatus = $"Fetched {items.Count} items. Saving...";

            // Step 2: Upsert Data
            int total = items.Count;
            int current = 0;

            foreach (var item in items)
            {
                await _databaseService.UpsertItemAsync(item);
                current++;

                // Calculate progress from 50% to 100%
                SyncProgress = 50 + (50 * ((double)current / total));
            }

            SyncStatus = "Sync Complete!";
            SyncProgress = 100;
        }
        catch (Exception ex)
        {
            SyncStatus = $"Error: {ex.Message}";
            // In a real app, you might want to log this or handle it more gracefully
        }
    }
}
