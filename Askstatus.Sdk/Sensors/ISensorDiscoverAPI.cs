using Askstatus.Common.Sensor;
using Refit;

namespace Askstatus.Sdk.Sensors;
public interface ISensorDiscoverAPI
{
    [Get("/api/sensordiscover/all")]
    Task<IApiResponse<IEnumerable<SensorInfo>>> DiscoverAll();
    [Get("/api/sensordiscover/notassigned")]
    Task<IApiResponse<IEnumerable<SensorInfo>>> NotAssigned();
}
