using System.ComponentModel.DataAnnotations;
using Askstatus.Common.Users;
using Askstatus.Sdk;
using Askstatus.Web.App.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Refit;

namespace Askstatus.Web.App.Pages.Identity;

public partial class Login
{
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [Inject]
    IAccountManagement? Acct { get; set; }

    [Inject] NavigationManager? NavigationManager { get; set; }

    [Inject]
    public AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private ILogger<Login> Logger { get; set; } = null!;

    protected RegisterAccountForm _model = new RegisterAccountForm();

    public class RegisterAccountForm
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

    }

    private async Task OnValidSubmit(EditContext context)
    {
        var result = await Acct!.LoginAsync(_model.Username!, _model.Password!);
        if (result.Succeeded)
        {
            NavigationManager!.NavigateTo("/");
        }
        else
        {
            var s = result.ErrorList[0];
            await DialogService!.ShowMessageBox(
                "Warning",
                s);
            StateHasChanged();
        }
    }
    private async Task ForgotPassword()
    {
        var parameters = new DialogParameters<ForgotPasswordDialog> { { x => x.ForgotPasswordRquest, new ForgotPasswordRquest() } };
        var dialog = await DialogService.ShowAsync<ForgotPasswordDialog>("Enter your email addres and press reset password.", parameters);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            if (result.Data is ForgotPasswordRquest)
            {
                var severety = Severity.Success;
                string message = "Password reset successful, Check your email for reset link!";
                var forgotPassword = ((ForgotPasswordRquest)result.Data);
                try
                {
                    var response = await ApiService.UserAPI.ForgotPassword(forgotPassword);
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.LogError(response.Error, response.Error.Content);
                        severety = Severity.Error;
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            message = $"Email address {forgotPassword.Email} not found!";
                        }
                        else
                        {
                            message = response.Error.Message;
                        }
                    }
                }
                catch (ApiException ex)
                {
                    Logger.LogError(ex, "Error forgot password");
                    message = ex.Message;
                    severety = Severity.Error;
                }
                Snackbar.Add(message, severety);
            }
        }
    }
}
