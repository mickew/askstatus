using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;

namespace Askstatus.Web.App.Pages.Roles;

public partial class Index
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private ILogger<Index> Logger { get; set; } = null!;

    protected bool UserGotNoRights { get; set; } = true;

    private ICollection<RoleDto> Roles { get; set; } = new List<RoleDto>();

    protected override async Task OnInitializedAsync()
    {
        var response = await ApiService.RoleAPI.GetRoles();
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            return;
        }
        Roles = response.Content!.ToList();
    }

    private async Task AddEditRole(RoleDto role, bool add)
    {
        var parameters = new DialogParameters<AddEditRoleDialog> { { x => x.role, role } };

        var dialog = await DialogService.ShowAsync<AddEditRoleDialog>(add ? "Add Role!" : "Edit Role!", parameters);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            if (result.Data is RoleDto)
            {
                var severety = Severity.Success;
                string message = $"Role {role.Name} updated";
                role = ((RoleDto)result.Data);
                if (string.IsNullOrEmpty(role.Id))
                {
                    try
                    {
                        role.Permissions = Permissions.None;
                        var response = await ApiService.RoleAPI.CreateRole(new RoleRequest(role.Id, role.Name, role.Permissions));
                        if (!response.IsSuccessStatusCode)
                        {
                            Logger.LogError(response.Error, response.Error.Content);
                            message = response.Error.Message;
                            severety = Severity.Error;
                        }
                        if (severety == Severity.Success)
                        {
                            role = response.Content!;
                        }
                    }
                    catch (ApiException ex)
                    {
                        Logger.LogError(ex, "Error adding role");
                        Logger.LogError(ex.Message);
                        message = ex.Message;
                        severety = Severity.Error;
                    }
                    if (severety == Severity.Success)
                    {
                        Roles.Add(role);
                        message = $"Role {role.Name} added";
                    }
                }
                else
                {
                    try
                    {
                        var response = await ApiService.RoleAPI.UpdateRole(new RoleRequest(role.Id, role.Name, role.Permissions));
                        if (!response.IsSuccessStatusCode)
                        {
                            Logger.LogError(response.Error, response.Error.Content);
                            message = response.Error.Message;
                            severety = Severity.Error;
                        }
                    }
                    catch (ApiException ex)
                    {
                        Logger.LogError(ex, "Error updating role");
                        Logger.LogError(ex.Message);
                        message = ex.Message;
                        severety = Severity.Error;
                    }
                }
                Snackbar.Add(message, severety);
            }
        }
    }

    private async Task DeleteRole(RoleDto role)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Delete role {role.Name} ?",
            yesText: "Delete!", cancelText: "Cancel");
        if (result != null && result.Value)
        {
            var severety = Severity.Success;
            string message = $"Role {role.Name} deleted";
            try
            {
                var response = await ApiService.RoleAPI.DeleteRole(role.Id);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogError(response.Error, response.Error.Content);
                    message = response.Error.Message;
                    severety = Severity.Error;
                }
            }
            catch (ApiException ex)
            {
                Logger.LogError(ex, "Error deleting role");
                Logger.LogError(ex.Message);
                message = ex.Message;
                severety = Severity.Error;
            }
            Roles.Remove(role);
            StateHasChanged();
            Snackbar.Add(message, severety);
        }
    }
}
