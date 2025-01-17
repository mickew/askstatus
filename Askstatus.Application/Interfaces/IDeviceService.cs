using Askstatus.Common.Models;
using FluentResults;

namespace Askstatus.Application.Interfaces;
public interface IDeviceService
{
    Task<Result<bool>> State(string host, int channel);

    Task<Result> Switch(string host, int channel, bool onOff);

    Task<Result> Toggle(string host, int channel);

    Task<Result<IEnumerable<WebHookInfo>>> GetWebHooks(string host);
}
