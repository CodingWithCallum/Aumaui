using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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
    [Required(ErrorMessage = "Company Code is required.")]
    [MaxLength(6, ErrorMessage = "Company Code must be 6 characters or fewer.")]
    [RegularExpression(@"^[A-Za-z0-9]{1,6}$", ErrorMessage = "Company Code must be alphanumeric.")]
    private string _companyCode = string.Empty;

    [ObservableProperty]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool CanLogin =>
        !HasErrors &&
        !string.IsNullOrWhiteSpace(CompanyCode);

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginWithMicrosoftAsync()
    {
        if (IsBusy) return;

        // Manual validation for Microsoft Login
        if (string.IsNullOrWhiteSpace(CompanyCode))
        {
            ErrorMessage = "Company Code is required for Microsoft Login.";
            return;
        }

        if (CompanyCode.Length > 6)
        {
            ErrorMessage = "Company Code must be 6 characters or fewer.";
            return;
        }

        if (!Regex.IsMatch(CompanyCode, @"^[A-Za-z0-9]+$"))
        {
            ErrorMessage = "Company Code must contain only letters and numbers.";
            return;
        }

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
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(CompanyCode))
        {
            ErrorMessage = "Company Code, Email, and Password are required.";
            return;
        }

        if (CompanyCode.Length > 6)
        {
            ErrorMessage = "Company Code must be 6 characters or fewer.";
            return;
        }

        if (!Regex.IsMatch(CompanyCode, @"^[A-Za-z0-9]+$"))
        {
            ErrorMessage = "Company Code must contain only letters and numbers.";
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
