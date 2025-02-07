using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Components;

namespace Askstatus.Web.App.Layout;

public partial class LayoutBase : LayoutComponentBase
{
    protected string Version { get; private set; } = string.Empty;

    protected string AspDotnetVersion { get; private set; } = string.Empty;

    protected string Year => DateTime.Now.Year.ToString();


    protected override async Task OnInitializedAsync()
    {
        Assembly currentAssembly = typeof(MainLayout).Assembly;
        if (currentAssembly == null)
        {
            currentAssembly = Assembly.GetCallingAssembly();
        }

        AspDotnetVersion = currentAssembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName!;
        Version = $"{currentAssembly.GetName().Version!.Major}.{currentAssembly.GetName().Version!.Minor}.{currentAssembly.GetName().Version!.Build}";
        await base.OnInitializedAsync();
    }
}
