using System.ComponentModel;
using AumauiCL.Interfaces;
using AumauiCL.Models.User;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AumauiCL.ViewModels;

public partial class DashboardViewModel : ObservableObject, IDisposable
{
    private readonly IAuthService _authService;
    private readonly ISyncService _syncService;

    [ObservableProperty]
    private UserModel? _currentUser;

    [ObservableProperty]
    private string _syncStatus = "Ready";

    [ObservableProperty]
    private double _syncProgress;

    [ObservableProperty]
    private bool _isBusy;

    public DashboardViewModel(IAuthService authService, ISyncService syncService)
    {
        _authService = authService;
        _syncService = syncService;

        // Subscribe to SyncService changes
        _syncService.PropertyChanged += OnSyncServicePropertyChanged;
        UpdateSyncState();
    }

    private void OnSyncServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ISyncService.SyncStatus) ||
            e.PropertyName == nameof(ISyncService.SyncProgress))
        {
            UpdateSyncState();
        }
    }

    private void UpdateSyncState()
    {
        SyncStatus = _syncService.SyncStatus;
        SyncProgress = _syncService.SyncProgress;
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            CurrentUser = await _authService.GetCurrentUserAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SyncNowAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            await _syncService.ExecuteSyncAsync();
        }
        finally
        {
            // SyncService updates status/progress, we just need to unlock UI
            IsBusy = false;
            // Refresh user in case sync updated it
            CurrentUser = await _authService.GetCurrentUserAsync();
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        // Navigation should be handled by View
    }

    public void Dispose()
    {
        _syncService.PropertyChanged -= OnSyncServicePropertyChanged;
    }
}
