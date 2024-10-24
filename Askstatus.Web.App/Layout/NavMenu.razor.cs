using Askstatus.Common.Users;
using Askstatus.Sdk;
using Askstatus.Web.App.Pages;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;

namespace Askstatus.Web.App.Layout;

public partial class NavMenu
{
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    [Inject]
    public AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private ILogger<NavMenu> Logger { get; set; } = null!;

    private async Task ChangePassword()
    {
        var parameters = new DialogParameters<ChangePasswordDialog> { { x => x.ChangePassword, new ChangePasswordRequest() } };
        var dialog = await DialogService.ShowAsync<ChangePasswordDialog>("Change Password", parameters);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            if (result.Data is ChangePasswordRequest)
            {
                var severety = Severity.Success;
                string message = "Password changed";
                var changePassword = ((ChangePasswordRequest)result.Data);
                try
                {
                    var response = await ApiService.UserAPI.ChangePassword(changePassword);
                    if (!response.IsSuccessStatusCode)
                    {
                        Logger.LogError(response.Error, response.Error.Content);
                        message = response.Error.Message;
                        severety = Severity.Error;
                    }
                }
                catch (ApiException ex)
                {
                    Logger.LogError(ex, "Error changing password");
                    Logger.LogError(ex.Message);
                    message = ex.Message;
                    severety = Severity.Error;
                }
                Snackbar.Add(message, severety);
            }
        }
    }
}
