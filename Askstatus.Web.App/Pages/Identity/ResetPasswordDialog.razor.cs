using Askstatus.Common.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Identity;

public partial class ResetPasswordDialog
{
    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    [Parameter] public ResetPasswordRequest ResetPasswordRequest { get; set; } = new ResetPasswordRequest();

    public ResetPasswordRequestFluentValidator ResetPasswordRequestFluentValidator = new ResetPasswordRequestFluentValidator();

    public MudForm? Form;

    private void Cancel()
    {
        MudDialog!.Cancel();
    }

    private async Task Ok()
    {
        await Form!.Validate();
        if (Form!.IsValid)
        {
            MudDialog!.Close(DialogResult.Ok(ResetPasswordRequest));
        }
    }
}
