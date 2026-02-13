using AumauiCL.Interfaces;
using AumauiCL.Services.Api;
using AumauiCL.Services.Auth;
using AumauiCL.Services.Data;
using AumauiCL.Services.Sync;
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

            // HTTP Client with auth handler pipeline
            builder.Services.AddTransient<AumauiCL.Services.Api.AuthHeaderHandler>();
            builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.BaseAddress = new Uri(AumauiCL.Services.Api.ApiConfig.BaseUrl);
            }).AddHttpMessageHandler<AumauiCL.Services.Api.AuthHeaderHandler>();

            // Services
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<ISecureStorageService, AumauiCL.Services.Storage.SecureStorageService>();
            builder.Services.AddSingleton<AumauiCL.Services.Authentication.HostAuthenticationStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<AumauiCL.Services.Authentication.HostAuthenticationStateProvider>());
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<IMsalService, AumauiCL.Services.Auth.MsalService>();
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
