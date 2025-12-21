using Askstatus.Common.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.AskStatus;

public partial class SelectUserDialog
{
    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    [Parameter] public ICollection<UserVM> Users { get; set; } = new List<UserVM>();

    private UserVM? _selectedUser;

    private bool _submitDisabled = true;


    private void Submit() => MudDialog!.Close(DialogResult.Ok(_selectedUser));

    private void Cancel() => MudDialog!.Cancel();

    private void OnSelectedItemChanged(UserVM user)
    {
        _selectedUser = user;
        _submitDisabled = user == null;
    }
}
