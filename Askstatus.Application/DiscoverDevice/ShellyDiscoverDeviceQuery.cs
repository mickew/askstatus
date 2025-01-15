using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.DiscoverDevice;
public sealed record ShellyDiscoverDeviceQuery(string Ip) : IRequest<Result<DicoverInfo>>;

public sealed class ShellyDiscoverDeviceQueryHandler : IRequestHandler<ShellyDiscoverDeviceQuery, Result<DicoverInfo>>
{
    private readonly ILogger<ShellyDiscoverDeviceQueryHandler> _logger;
    private readonly IDiscoverDeviceService _discoverDeviceService;
    public ShellyDiscoverDeviceQueryHandler(ILogger<ShellyDiscoverDeviceQueryHandler> logger, IDiscoverDeviceService discoverDeviceService)
    {
        _logger = logger;
        _discoverDeviceService = discoverDeviceService;
    }
    public async Task<Result<DicoverInfo>> Handle(ShellyDiscoverDeviceQuery request, CancellationToken cancellationToken)
    {
        var result = await _discoverDeviceService.Discover(request.Ip);
        if (result.IsFailed)
        {
            _logger.LogError("Failed to discover device at {Ip}", request.Ip);
        }
        return result;
    }
}
