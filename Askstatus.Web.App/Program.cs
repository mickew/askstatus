using Askstatus.Application.Authorization;
using Askstatus.Sdk;
using Askstatus.Web.App.Identity;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using MudBlazor.Services;

namespace Askstatus.Web.App;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddOptions<AskstatusSettings>()
            .Bind(builder.Configuration.GetSection(AskstatusSettings.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddBlazoredLocalStorage();

        var app = builder.Build();
        var askstatusSettings = app.Services.GetRequiredService<IOptions<AskstatusSettings>>().Value;

        // register the cookie handler
        builder.Services.AddTransient<CookieHandler>();

        // set up authorization
        builder.Services.AddAuthorizationCore();

        builder.Services.AddMudServices();

        // register the custom state provider
        builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

        builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, FlexibleAuthorizationPolicyProvider>();

        // register the account management interface
        builder.Services.AddScoped(
            sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

        builder.Services.AddHttpClient("AskStatus.Web.API",
            client => client.BaseAddress = new Uri(askstatusSettings.AskstatusUrl!)).AddHttpMessageHandler<CookieHandler>();

        builder.Services.AddTransient<AskstatusApiService>(o =>
        {
            if (o.GetRequiredService<IHttpClientFactory>().CreateClient("AskStatus.Web.API") is HttpClient client)
            {
                return new AskstatusApiService(client);
            }
            return null!;
        });
        //// Supply HttpClient instances that include access tokens when making requests to the server project
        //builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Askstatus.Web.API"));


        await builder.Build().RunAsync();
    }
}
