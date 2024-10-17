using Askstatus.Sdk.Identity;
using Askstatus.Sdk.Users;
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

        UserAPI = RestService.For<IUserAPI>(httpClient, new RefitSettings
        {
            ContentSerializer = contentSerializer
        });

        RoleAPI = RestService.For<IRoleAPI>(httpClient, new RefitSettings
        {
            ContentSerializer = contentSerializer
        });
    }

    public IIdentityApi IdentityApi { get; private set; }

    public IUserAPI UserAPI { get; private set; }

    public IRoleAPI RoleAPI { get; private set; }
}
