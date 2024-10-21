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
        IEnumerable<RoleDto> roles;
        var roleResponse = await ApiService.RoleAPI.GetRoles();
        if (!roleResponse.IsSuccessStatusCode)
        {
            Logger.LogError(roleResponse.Error, roleResponse.Error.Content);
            Snackbar.Add(roleResponse.Error.Content!, Severity.Error);
            return;
        }
        roles = roleResponse.Content!;
        UserVM theUser;
        if (add)
        {
            theUser = user;
        }
        else
        {
            var response = await ApiService.UserAPI.GetUserById(user.Id);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError(response.Error, response.Error.Content);
                Snackbar.Add(response.Error.Content!, Severity.Error);
                return;
            }
            theUser = response.Content!;
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
                            Snackbar.Add(response.Error.Content!, Severity.Error);
                            return;
                        }
                        theUser = response.Content!;
                    }
                    catch (ApiException ex)
                    {
                        Logger.LogError(ex, "Error adding user");
                        Logger.LogError(ex.Message);
                        Snackbar.Add(ex.Message, Severity.Error);
                        return;
                    }
                    Users.Add(theUser);
                    Snackbar.Add($"User {user.UserName} added", Severity.Success);
                }
                else
                {
                    try
                    {
                        var response = await ApiService.UserAPI.UpdateUser(new UserRequest(theUser.Id, theUser.UserName, theUser.Email, theUser.FirstName, theUser.LastName, theUser.Roles));
                        if (!response.IsSuccessStatusCode)
                        {
                            Logger.LogError(response.Error, response.Error.Content);
                            Snackbar.Add(response.Error.Content!, Severity.Error);
                            return;
                        }
                    }
                    catch (ApiException ex)
                    {
                        Logger.LogError(ex, "Error updating user");
                        Logger.LogError(ex.Message);
                        Snackbar.Add(ex.Message, Severity.Error);
                        return;
                    }
                    Users.Remove(user);
                    Users.Add(theUser);
                    Snackbar.Add($"User {user.UserName} updated", Severity.Success);
                }
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
            try
            {
                var response = await ApiService.UserAPI.DeleteUser(user.Id);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError(response.Error, response.Error.Content);
                    Snackbar.Add(response.Error.Content!, Severity.Error);
                    return;
                }
            }
            catch (ApiException ex)
            {
                Logger.LogError(ex, "Error deleting user");
                Logger.LogError(ex.Message);
                Snackbar.Add(ex.Message, Severity.Error);
                return;
            }
            Users.Remove(user);
            StateHasChanged();
            Snackbar.Add($"User {user.UserName} deleted", Severity.Success);
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
            try
            {
                var response = await ApiService.UserAPI.ResetPassword(user.Id);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError(response.Error, response.Error.Content);
                    Snackbar.Add(response.Error.Content!, Severity.Error);
                    return;
                }
            }
            catch (ApiException ex)
            {
                Logger.LogError(ex, "Error resetting password");
                Logger.LogError(ex.Message);
                Snackbar.Add(ex.Message, Severity.Error);
                return;
            }
            Snackbar.Add($"Password for user {user.UserName} reset", Severity.Success);
        }
    }
}
