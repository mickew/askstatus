using Askstatus.Sdk.Identity;
using Refit;

namespace Askstatus.Sdk;
public sealed class AskstatusApiService
{
    public AskstatusApiService(HttpClient httpClient)
    {
        var contentSerializer = new NewtonsoftJsonContentSerializer();
        IdentityApi = RestService.For<IIdentityApi>(httpClient, new RefitSettings
        {
            ContentSerializer = contentSerializer
        });
    }

    public IIdentityApi IdentityApi { get; private set; }
}
