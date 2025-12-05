namespace Askstatus.Application.Interfaces;
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    IRepository<Askstatus.Domain.Entities.PowerDevice> PowerDeviceRepository { get; }
    IRepository<Askstatus.Domain.Entities.SystemLog> SystemLogRepository { get; }
    IRepository<Askstatus.Domain.Entities.Sensor> SensorRepository { get; }
}
