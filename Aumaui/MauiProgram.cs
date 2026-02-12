using AumauiCL.Interfaces;
using AumauiCL.Services.Api; // Assuming ApiService/MockApiService is here, though MockApiService was used in SyncService constructor? 
using AumauiCL.Services.Auth;
using AumauiCL.Services.Data;
using AumauiCL.Services.Sync;
// Wait, SyncService takes MockApiService, but MauiProgram registers ApiService? 
// Let's check constructor of SyncService again. 
// SyncService takes (DatabaseService, MockApiService). 
// MauiProgram registers ISyncService, SyncService. 
// MauiProgram registers IApiService, ApiService.
// I need to ensure MockApiService is registered or SyncService uses IApiService.
// SyncService.cs: public SyncService(DatabaseService databaseService, MockApiService apiService)
// So I must register MockApiService.

using AumauiCL.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Aumaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Register Blazor Hybrid & Developer Tools
            builder.Services.AddMauiBlazorWebView();

            // HTTP Client
            builder.Services.AddHttpClient();

            // Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<MockApiService>(); // Register MockApi for now
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<ISyncService, SyncService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();

            // UI Library (Fluent UI)
            builder.Services.AddFluentUIComponents();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
