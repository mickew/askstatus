using Askstatus.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Askstatus.Infrastructure.Services;
public sealed class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SaveFileAsync(string fileName, Stream data, CancellationToken cancellationToken)
    {
        try
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                await data.CopyToAsync(stream, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file with file name {filename}", fileName);
            return false;
        }
        return true;
    }
}
