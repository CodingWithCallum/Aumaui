using System.ComponentModel.DataAnnotations;
using AumauiCL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AumauiCL.ViewModels;

public partial class LoginViewModel : ObservableValidator
{
    private readonly IAuthService _authService;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginWithMicrosoftCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoginWithStandardCommand))]
    [Required(ErrorMessage = "Company Code is required")]
    private string _companyCode = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    private string _email = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Password is required")]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool CanLogin => !string.IsNullOrWhiteSpace(CompanyCode) && !HasErrors;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginWithMicrosoftAsync()
    {
        if (IsBusy) return;
        ValidateAllProperties();
        if (HasErrors) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            await _authService.LoginWithMicrosoftAsync(CompanyCode);
            // Navigation will be handled by the View (LoginPage.razor) observing the state or a success event
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginWithStandardAsync()
    {
        if (IsBusy) return;

        // Manual validation for standard login fields
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Email and Password are required.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            await _authService.LoginWithStandardAsync(CompanyCode, Email, Password);
            // Navigation will be handled by the View
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login Failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
