using Askstatus.Common.Sensor;
using Refit;

namespace Askstatus.Sdk.Sensors;
public interface ISensorAPI
{
    [Get("/api/sensor")]
    Task<IApiResponse<IEnumerable<SensorDto>>> GetSensors();

    [Get("/api/sensor/{id}")]
    Task<IApiResponse<SensorDto>> GetSensorById(int id);

    [Post("/api/sensor")]
    Task<IApiResponse<SensorDto>> CreateSensor(SensorRequest request);

    [Put("/api/sensor")]
    Task<IApiResponse> UpdateSensor(SensorRequest request);

    [Delete("/api/sensor/{id}")]
    Task<IApiResponse> DeleteSensor(int id);
}
