using Askstatus.Sdk.Identity;
using Askstatus.Sdk.PowerDevices;
using Askstatus.Sdk.Sensors;
using Askstatus.Sdk.System;
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

        PowerDeviceAPI = RestService.For<IPowerDeviceAPI>(httpClient, new RefitSettings
        {
            ContentSerializer = contentSerializer
        });

        DeviceDiscoverAPI = RestService.For<IDeviceDiscoverAPI>(httpClient, new RefitSettings
        {
            ContentSerializer = contentSerializer
        });

        SystemAPI = RestService.For<ISystemAPI>(httpClient, new RefitSettings
        {
            ContentSerializer = contentSerializer
        });

        SensorDiscoverAPI = RestService.For<ISensorDiscoverAPI>(httpClient, new RefitSettings
        {
            ContentSerializer = contentSerializer
        });

        SensorAPI = RestService.For<ISensorAPI>(httpClient, new RefitSettings
        {
            ContentSerializer = contentSerializer
        });
    }

    public IIdentityApi IdentityApi { get; private set; }

    public IUserAPI UserAPI { get; private set; }

    public IRoleAPI RoleAPI { get; private set; }

    public IPowerDeviceAPI PowerDeviceAPI { get; private set; }

    public IDeviceDiscoverAPI DeviceDiscoverAPI { get; private set; }

    public ISystemAPI SystemAPI { get; private set; }

    public ISensorDiscoverAPI SensorDiscoverAPI { get; private set; }

    public ISensorAPI SensorAPI { get; private set; }
}
