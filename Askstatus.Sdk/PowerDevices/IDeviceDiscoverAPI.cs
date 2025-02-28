using Askstatus.Common.Models;
using Refit;

namespace Askstatus.Sdk.PowerDevices;
public interface IDeviceDiscoverAPI
{
    [Get("/api/devicediscover")]
    Task<IApiResponse<DicoverInfo>> Discover([AliasAs("host")] string host);
    [Get("/api/devicediscover/all")]
    Task<IApiResponse<IEnumerable<DicoverInfo>>> DiscoverAll();
    [Get("/api/devicediscover/notassigned")]
    Task<IApiResponse<IEnumerable<DicoverInfo>>> NotAssigned();
}
