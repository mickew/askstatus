using Askstatus.Common.Models;
using FluentResults;

namespace Askstatus.Application.Interfaces;
public interface IDiscoverDeviceService
{
    Task<Result<DicoverInfo>> Discover(string ip);
}
