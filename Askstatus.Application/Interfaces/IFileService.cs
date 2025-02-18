namespace Askstatus.Application.Interfaces;
public interface IFileService
{
    Task<bool> SaveFileAsync(string fileName, Stream data, CancellationToken cancellationToken);
}
