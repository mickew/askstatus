using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using Askstatus.Sdk;
using Askstatus.Sdk.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;

namespace Askstatus.Web.App.Pages.AccessControl;

public partial class Index
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private ILogger<Index> Logger { get; set; } = null!;

    protected bool UserGotNoRights { get; set; } = true;

    private AccessControlVm? _vm;

    protected override async Task OnInitializedAsync()
    {
        var response = await ApiService.RoleAPI.GetPermissions();
        if (!response.IsSuccessStatusCode)
        {
            // handle error
            return;
        }
        _vm = response.Content;
    }

    private async Task Set(RoleDto role, Permissions permission, bool granted)
    {
        role.Set(permission, granted);
        var severety = Severity.Success;
        string message = $"permission for role {role.Name} updated";
        try
        {
            var response = await ApiService.RoleAPI.UpdatePermissions(new RoleRequest(role.Id, role.Name, role.Permissions));
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
        Snackbar.Add(message, severety);
    }
}
