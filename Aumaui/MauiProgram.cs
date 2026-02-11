using AumauiCL.Interfaces;
using AumauiCL.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using MyMauiApp.Services.Api;
using MyMauiApp.Services.Auth;
using MyMauiApp.Services.Data;
using MyMauiApp.Services.Sync;

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
            // Register our custom provider as a scoped service
            builder.Services.AddScoped<HostAuthenticationStateProvider>();
            // Tell Blazor to use our custom provider for the standard 'AuthenticationStateProvider'
            builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<HostAuthenticationStateProvider>());

            // Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<ISyncService, SyncService>();
            builder.Services.AddTransient<AumauiCL.ViewModels.LoginViewModel>();

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
