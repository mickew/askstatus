using Askstatus.Common.Authorization;
using Askstatus.Common.Users;
using Askstatus.Sdk;
using Askstatus.Sdk.Users;
using Microsoft.AspNetCore.Components;

namespace Askstatus.Web.App.Pages.AccessControl;

public partial class Index
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

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

        var response = await ApiService.RoleAPI.UpdatePermissions(new RoleRequest(role.Id, role.Name, role.Permissions));
        if (!response.IsSuccessStatusCode)
        {
            // handle error
            return;
        }
    }

}
