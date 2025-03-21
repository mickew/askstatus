﻿using Askstatus.Common.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Identity;

public partial class ForgotPasswordDialog
{
    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    [Parameter] public ForgotPasswordRquest ForgotPasswordRquest { get; set; } = new ForgotPasswordRquest();

    public ForgotPasswordRquestFluentValidator ForgotPasswordRquestValidator = new ForgotPasswordRquestFluentValidator();

    public MudForm? Form;

    private DefaultFocus DefaultFocus { get; set; } = DefaultFocus.FirstChild;

    private void Cancel()
    {
        MudDialog!.Cancel();
    }

    private async Task Ok()
    {
        await Form!.Validate();
        if (Form!.IsValid)
        {
            MudDialog!.Close(DialogResult.Ok(ForgotPasswordRquest));
        }
    }
}

