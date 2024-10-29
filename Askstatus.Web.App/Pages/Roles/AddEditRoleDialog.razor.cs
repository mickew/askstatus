using Askstatus.Common.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Roles;

public partial class AddEditRoleDialog
{
    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    [Parameter] public RoleDto role { get; set; } = new RoleDto();

    private void Cancel()
    {
        MudDialog!.Cancel();
    }

    private void SaveRole()
    {
        MudDialog!.Close(DialogResult.Ok(role));
    }
}
