using Askstatus.Common.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Users;

public partial class AddEditUserDialog
{
    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    [Parameter] public UserVM User { get; set; } = new UserVM();

    [Parameter] public ICollection<RoleDto> Roles { get; set; } = new List<RoleDto>();

    public UserVMFluentValidator UserDtoValidator = new UserVMFluentValidator();

    public MudForm? form;

    private DefaultFocus DefaultFocus { get; set; } = DefaultFocus.FirstChild;

    private void Cancel()
    {
        MudDialog!.Cancel();
    }

    private async Task SaveUser()
    {
        await form!.Validate();
        if (form!.IsValid)
        {
            MudDialog!.Close(DialogResult.Ok(User));
        }
    }

    private void ToggleSelectedRole(string roleName)
    {
        if (User.Roles.Contains(roleName))
        {
            User.Roles.Remove(roleName);
        }
        else
        {
            User.Roles.Add(roleName);
        }

        StateHasChanged();
    }

}
