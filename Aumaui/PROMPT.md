# Project: Aumaui (MAUI Blazor Hybrid) & AumauiCL (Core Logic)

## 1. Solution Architecture
- **App Project:** `Aumaui` (Target: .NET 10 MAUI Blazor Hybrid).
  - Role: UI rendering (Razor Components), Platform-specific handlers, and App Startup.
- **Core Library:** `AumauiCL` (Target: .NET MAUI Class Library).
  - Role: Business Logic, API Clients, SQLite Services, and ViewModels.
- **Data Flow:** `Aumaui` references `AumauiCL`. All logic must reside in `AumauiCL`.

## 2. Strategic Authentication Requirements
The app uses a dual-authentication strategy:
1. **Tenant Identification:** Every login (Standard or Microsoft) MUST be preceded or accompanied by a **Company Code**. 
2. **Standard Login:** Authenticates against a custom API using the Company Code + Email + Password.
3. **Microsoft SSO (MSAL):** - Uses `Microsoft.Identity.Client`.
   - The user enters their Company Code first to resolve the specific Tenant ID for the MSAL `WithAuthority` call (Multitenant flow).
4. **Offline Persistence:** On successful login, auth tokens and tenant-specific configurations must be stored in the local SQLite database via `AumauiCL.Services.Data.DatabaseService`.

## 3. Technology & Patterns
- **Framework:** .NET 10 LTS.
- **UI:** Fluent UI for Blazor components (Microsoft aesthetic).
- **State:** `CommunityToolkit.Mvvm` (use `ObservableProperty` and `RelayCommand`).
- **Database:** `sqlite-net-pcl` for local storage/syncing.
- **Coding Style:** - Use **Primary Constructors** (C# 12/14) for all injected services.
  - All external calls must be `async` and use the `IAuthService` and `ISyncService` interfaces.

## 4. Immediate Agent Goal
Scaffold the `LoginPage.razor` in the `Aumaui` project. It should have:
- An input for 'Company Code'.
- A 'Login with Microsoft' button (Work Account).
- Standard Email/Password fields for 'Standard Login'.
- Implementation logic must call the corresponding methods in `AumauiCL.Services.AuthService`.