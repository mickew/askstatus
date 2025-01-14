using Askstatus.Common.PowerDevice;
using Refit;

namespace Askstatus.Sdk.PowerDevices;
public interface IPowerDeviceAPI
{
    [Get("/api/powerdevice")]
    Task<IApiResponse<IEnumerable<PowerDeviceDto>>> GetPowerDevices();

    [Get("/api/powerdevice/{id}")]
    Task<IApiResponse<PowerDeviceDto>> GetPowerDeviceById(int id);

    [Post("/api/powerdevice")]
    Task<IApiResponse<PowerDeviceDto>> CreatePowerDevice(PowerDeviceRequest request);

    [Put("/api/powerdevice")]
    Task<IApiResponse> UpdatePowerDevice(PowerDeviceRequest request);

    [Delete("/api/powerdevice/{id}")]
    Task<IApiResponse> DeletePowerDevice(int id);
}
