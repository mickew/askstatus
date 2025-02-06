using Askstatus.Common.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages;

public partial class ChangePasswordDialog
{
    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    [Parameter] public ChangePasswordRequest ChangePassword { get; set; } = new ChangePasswordRequest();

    public ChangePasswordRequestFluentValidator ChangePasswordRequestValidator = new ChangePasswordRequestFluentValidator();


    public MudForm? Form;

    private DefaultFocus DefaultFocus { get; set; } = DefaultFocus.FirstChild;

    private void Cancel()
    {
        MudDialog!.Cancel();
    }

    private async Task SavePassword()
    {
        await Form!.Validate();
        if (Form!.IsValid)
        {
            MudDialog!.Close(DialogResult.Ok(ChangePassword));
        }
    }
}
