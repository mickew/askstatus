using System.Data;
using Askstatus.Common.Users;
using Askstatus.Sdk;
using Askstatus.Sdk.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;

namespace Askstatus.Web.App.Pages.Users;

public partial class Index
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private NavigationManager? Navigation { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private ILogger<Index> Logger { get; set; } = null!;

    protected bool UserGotNoRights { get; set; } = true;

    private ICollection<UserVM> Users { get; set; } = new List<UserVM>();

    protected override async Task OnInitializedAsync()
    {
        var response = await ApiService.UserAPI.GetUsers();
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            return;
        }
        Users = response.Content!.ToList();
    }

    private async Task AddEditUser(UserVM user, bool add)
    {
        var severety = Severity.Success;
        string message = $"User {user.UserName} updated";
        IEnumerable<RoleDto> roles = new List<RoleDto>();
        try
        {
            var roleResponse = await ApiService.RoleAPI.GetRoles();
            if (!roleResponse.IsSuccessStatusCode)
            {
                Logger.LogError(roleResponse.Error, roleResponse.Error.Content);
                message = roleResponse.Error.Message;
                severety = Severity.Error;
            }
            if (severety == Severity.Success)
            {
                roles = roleResponse.Content!;
            }
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex, "Error adding user");
            Logger.LogError(ex.Message);
            message = ex.Message;
            severety = Severity.Error;
        }
        if (severety != Severity.Success)
        {
            Snackbar.Add(message, severety);
            return;
        }
        UserVM theUser = null!;
        if (add)
        {
            theUser = user;
        }
        else
        {
            try
            {
                var response = await ApiService.UserAPI.GetUserById(user.Id);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError(response.Error, response.Error.Content);
                    message = response.Error.Message;
                    severety = Severity.Error;
                }
                if (severety == Severity.Success)
                {
                    theUser = response.Content!;
                }
            }
            catch (ApiException ex)
            {
                Logger.LogError(ex, "Error adding user");
                Logger.LogError(ex.Message);
                message = ex.Message;
                severety = Severity.Error;
            }
        }
        if (severety != Severity.Success)
        {
            Snackbar.Add(message, severety);
            return;
        }
        var parameters = new DialogParameters();
        parameters.Add(nameof(AddEditUserDialog.User), theUser);
        parameters.Add(nameof(AddEditUserDialog.Roles), roles);
        //var parameters = new DialogParameters<AddEditUserDialog> { { x => x.User, user } };

        var dialog = await DialogService.ShowAsync<AddEditUserDialog>(add ? "Add User!" : "Edit User!", parameters);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            if (result.Data is UserVM)
            {
                theUser = ((UserVM)result.Data);
                if (string.IsNullOrEmpty(user.Id))
                {
                    try
                    {
                        var response = await ApiService.UserAPI.CreateUser(new UserRequest(theUser.Id, theUser.UserName, theUser.Email, theUser.FirstName, theUser.LastName, theUser.Roles));
                        if (!response.IsSuccessStatusCode)
                        {
                            Logger.LogError(response.Error, response.Error.Content);
                            message = response.Error.Message;
                            severety = Severity.Error;
                        }
                        if (severety == Severity.Success)
                        {
                            theUser = response.Content!;
                        }
                    }
                    catch (ApiException ex)
                    {
                        Logger.LogError(ex, "Error adding user");
                        Logger.LogError(ex.Message);
                        message = ex.Message;
                        severety = Severity.Error;
                    }
                    if (severety == Severity.Success)
                    {
                        Users.Add(theUser);
                        message = $"User {user.UserName} added";
                        severety = Severity.Success;
                    }
                }
                else
                {
                    try
                    {
                        var response = await ApiService.UserAPI.UpdateUser(new UserRequest(theUser.Id, theUser.UserName, theUser.Email, theUser.FirstName, theUser.LastName, theUser.Roles));
                        if (!response.IsSuccessStatusCode)
                        {
                            Logger.LogError(response.Error, response.Error.Content);
                            message = response.Error.Message;
                            severety = Severity.Error;
                        }
                    }
                    catch (ApiException ex)
                    {
                        Logger.LogError(ex, "Error updating user");
                        Logger.LogError(ex.Message);
                        message = ex.Message;
                        severety = Severity.Error;
                    }
                    if (severety == Severity.Success)
                    {
                        Users.Remove(user);
                        Users.Add(theUser);
                        message = $"User {user.UserName} updated";
                        severety = Severity.Success;
                    }
                }
                Snackbar.Add(message, severety);
                StateHasChanged();
            }
        }
    }

    private async Task DeleteUser(UserVM user)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Delete user {user.UserName} ?",
            yesText: "Delete!", cancelText: "Cancel");
        if (result != null && result.Value)
        {
            var severety = Severity.Success;
            string message = $"User {user.UserName} deleted";
            try
            {
                var response = await ApiService.UserAPI.DeleteUser(user.Id);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError(response.Error, response.Error.Content);
                    message = response.Error.Message;
                    severety = Severity.Error;
                }
            }
            catch (ApiException ex)
            {
                Logger.LogError(ex, "Error deleting user");
                Logger.LogError(ex.Message);
                message = ex.Message;
                severety = Severity.Error;
            }
            if (severety == Severity.Success)
            {
                Users.Remove(user);
                StateHasChanged();
            }
            Snackbar.Add(message, severety);
        }
    }

    private async Task ResetPassword(UserVM user)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Reset password for user {user.UserName} ?",
            yesText: "Reset!", cancelText: "Cancel");
        if (result != null && result.Value)
        {
            var severety = Severity.Success;
            string message = $"Password for user {user.UserName} reset";
            try
            {
                var response = await ApiService.UserAPI.ResetPassword(user.Id);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError(response.Error, response.Error.Content);
                    message = response.Error.Message;
                    severety = Severity.Error;
                }
            }
            catch (ApiException ex)
            {
                Logger.LogError(ex, "Error resetting password");
                Logger.LogError(ex.Message);
                message = ex.Message;
                severety = Severity.Error;
            }
            Snackbar.Add(message, severety);
        }
    }
}
