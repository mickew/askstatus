namespace Askstatus.Application.Interfaces;
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    IRepository<Askstatus.Domain.Entities.PowerDevice> PowerDeviceRepository { get; }
}
