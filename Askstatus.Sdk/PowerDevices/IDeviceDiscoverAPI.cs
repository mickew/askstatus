using Askstatus.Common.Models;
using Refit;

namespace Askstatus.Sdk.PowerDevices;
public interface IDeviceDiscoverAPI
{
    [Get("/api/devicediscover")]
    Task<IApiResponse<DicoverInfo>> Discover([AliasAs("host")] string host);
}
