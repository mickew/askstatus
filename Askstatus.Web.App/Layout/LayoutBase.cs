using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Askstatus.Web.App.Layout;

public partial class LayoutBase : LayoutComponentBase
{
    [Inject]
    private IWebAssemblyHostEnvironment HostEnvironment { get; set; } = null!;

    protected string Environment { get; private set; } = string.Empty;

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
        Environment = HostEnvironment.Environment;
        AspDotnetVersion = currentAssembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName!;
        Version = $"{currentAssembly.GetName().Version!.Major}.{currentAssembly.GetName().Version!.Minor}.{currentAssembly.GetName().Version!.Build}";
        await base.OnInitializedAsync();
    }
}
