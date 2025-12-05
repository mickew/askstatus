using Askstatus.Application.Interfaces;
using Askstatus.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Askstatus.Infrastructure.Data;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UnitOfWork> _logger;
    private bool _disposed = false;

    public UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public IRepository<PowerDevice> PowerDeviceRepository => _serviceProvider.GetRequiredService<IRepository<PowerDevice>>();

    public IRepository<SystemLog> SystemLogRepository => _serviceProvider.GetRequiredService<IRepository<SystemLog>>();

    public IRepository<Sensor> SensorRepository => _serviceProvider.GetRequiredService<IRepository<Sensor>>();

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes");
        }
        return -1;
    }
}
