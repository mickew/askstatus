using Askstatus.Common.System;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.AskStatus;

public partial class SendEmail
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    public SystemSendMailRequest SystemSendMailRequest { get; set; } = new SystemSendMailRequest();

    public SystemSendMailRequestFluentValidator SystemSendMailRequestFluentValidator = new SystemSendMailRequestFluentValidator();

    public MudForm? Form;

    private async Task SendMail()
    {
        await Form!.Validate();
        if (Form!.IsValid)
        {
            var result = await ApiService.SystemAPI.SendMail(SystemSendMailRequest);
            if (!result.IsSuccessStatusCode)
            {
                Snackbar.Add($"Error sending email: {result.Error.Content}", Severity.Error);
                return;
            }
            Snackbar.Add("Email sent successfully", Severity.Success);
            SystemSendMailRequest = new SystemSendMailRequest();
        }
    }

    private async Task SelectUser()
    {
        var usersResponse = await ApiService.UserAPI.GetUsers();
        if (!usersResponse.IsSuccessStatusCode)
        {
            Snackbar.Add($"Error fetching users: {usersResponse.Error.Content}", Severity.Error);
            return;
        }
        var users = usersResponse.Content!;
        var parameters = new DialogParameters<SelectUserDialog> { { x => x.Users, users } };
        var dialog = await DialogService.ShowAsync<SelectUserDialog>("Select User", parameters);
        var result = await dialog.Result;
        if (result is null || result.Canceled)
        {
            return;
        }
        if (result.Data is not null && result.Data is Askstatus.Common.Users.UserVM selectedUser)
        {
            SystemSendMailRequest.EmailTo = selectedUser.Email;
            SystemSendMailRequest.FirstName = selectedUser.FirstName;
        }
    }
}
