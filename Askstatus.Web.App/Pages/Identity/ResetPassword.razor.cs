using Askstatus.Common.Users;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using Refit;

namespace Askstatus.Web.App.Pages.Identity;

public partial class ResetPassword
{
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private ILogger<ResetPassword> Logger { get; set; } = null!;

    [Inject]
    public ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public NavigationManager? Navigation { get; set; } = null!;

    public string? UserId { get; set; }

    public string? Token { get; set; }

    private bool IsPasswordReset { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("OnInitializedAsync started");
        await GetQueryStringValues();
        Logger.LogInformation("Query string userid : {UserId}", UserId);
        Logger.LogInformation("Query string token : {Token}", Token);
        if (Token != null && UserId != null)
        {
            await ResetThePassword();
        }
    }

    private async Task ResetThePassword()
    {
        var resetPasswordRequest = new ResetPasswordRequest(UserId!, Token!, string.Empty);
        var parameters = new DialogParameters<ResetPasswordDialog> { { x => x.ResetPasswordRequest, resetPasswordRequest } };
        var dialog = await DialogService.ShowAsync<ResetPasswordDialog>("Enter your new password and press set password.", parameters);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            if (result.Data is ResetPasswordRequest)
            {
                var severety = Severity.Success;
                string message = "Password set successful, Try to login with the new password!";
                resetPasswordRequest = ((ResetPasswordRequest)result.Data);
                try
                {
                    var response = await ApiService.UserAPI.ResetUserPassword(resetPasswordRequest);
                    if (response.IsSuccessStatusCode)
                    {
                        IsPasswordReset = true;
                    }
                    else
                    {
                        Logger.LogError(response.Error, response.Error.Content);
                        severety = Severity.Error;
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            message = $"User not found!";
                        }
                        else
                        {
                            message = response.Error.Message;
                        }
                    }
                }
                catch (ApiException ex)
                {
                    Logger.LogError(ex, "Error reset password");
                    message = ex.Message;
                    severety = Severity.Error;
                }
                Snackbar.Add(message, severety);
            }
        }
    }

    private Task GetQueryStringValues()
    {
        var uri = new Uri(Navigation!.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);
        try
        {
            UserId = query["userId"];
            Token = query["token"];
        }
        catch (Exception)
        {
            Logger.LogError("Error getting query string values userId or token");
        }
        return Task.CompletedTask;
    }
}
