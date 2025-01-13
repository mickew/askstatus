namespace Askstatus.Application.Interfaces;
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    IPowerDeviceRepository PowerDeviceRepository { get; }
}
