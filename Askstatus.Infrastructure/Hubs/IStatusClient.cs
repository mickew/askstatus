namespace Askstatus.Infrastructure.Hubs;
public interface IStatusClient
{
    Task UpdateDeviceStatus(int id, bool state);
    Task SensorValueChanged(int sensorId, string newValue, DateTime timeStamp);
}
