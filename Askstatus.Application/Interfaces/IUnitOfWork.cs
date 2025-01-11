namespace Askstatus.Application.Interfaces;
public interface IUnitOfWork : IDisposable
{
    Task SaveChangesAsync();
    IPowerDeviceRepository PowerDeviceRepository { get; }
}
