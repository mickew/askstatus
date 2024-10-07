using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace Askstatus.Web.App;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.Services.AddMudServices();

        //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddHttpClient("AskStatus.Web.API", client => client.BaseAddress = new Uri("https://localhost:7298"));
        //.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        //// Supply HttpClient instances that include access tokens when making requests to the server project
        //builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Askstatus.Web.API"));


        await builder.Build().RunAsync();
    }
}
