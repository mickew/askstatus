using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using Askstatus.Web.App.Identity;

namespace Askstatus.Web.App.Pages.Identity;

public partial class Login
{
    [Inject] private IDialogService? DialogService { get; set; }

    [Inject]
    IAccountManagement? Acct { get; set; }

    [Inject] NavigationManager? NavigationManager { get; set; }

    protected RegisterAccountForm _model = new RegisterAccountForm();

    public class RegisterAccountForm
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

    }

    private async Task OnValidSubmit(EditContext context)
    {
        var result = await Acct!.LoginAsync(_model.Username!, _model.Password!);
        if (result.Succeeded)
        {
            NavigationManager!.NavigateTo("/");
        }
        else
        {
            var s = result.ErrorList[0];
            await DialogService!.ShowMessageBox(
                "Warning",
                s);
            StateHasChanged();
        }
    }
}
