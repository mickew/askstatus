using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Services;
public class Shelly2DeviceService : IDeviceService
{
    private readonly ILogger<Shelly2DeviceService> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public Shelly2DeviceService(ILogger<Shelly2DeviceService> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task<Result<bool>> State(string host, int channel)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://{host}/rpc/Switch.GetStatus?id={channel}");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                ChannelStatus? result = JsonSerializer.Deserialize<ChannelStatus>(apiString);
                return Result.Ok<bool>(result!.Output);
            }
            else
            {
                var apiString = await response.Content.ReadAsStringAsync();
                _logger.LogError(apiString);
                return Result.Fail<bool>(new BadRequestError(apiString));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        return Result.Fail<bool>(new BadRequestError("Failed to get status from device"));
    }

    public async Task<Result> Switch(string host, int channel, bool onOff)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://{host}/rpc/Switch.Set?id={channel}&on={onOff.ToString().ToLower()}");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                ToggleSwitchStatus? result = JsonSerializer.Deserialize<ToggleSwitchStatus>(apiString);
                if (result!.Was_On == onOff)
                {
                    _logger.LogWarning("Host {host} was alreddy {onof} when switching {onoff}", host, BooleanToOnOff(result!.Was_On), BooleanToOnOff(onOff));
                    return Result.Ok();
                }
                else
                {
                    _logger.LogInformation("Host {host} was switched {onoff}", host, BooleanToOnOff(onOff));
                    return Result.Ok();
                }
            }
            else
            {
                var apiString = await response.Content.ReadAsStringAsync();
                _logger.LogError(apiString);
                return Result.Fail(new BadRequestError(apiString));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        return Result.Fail(new BadRequestError("Failed to get status from device"));
    }

    public async Task<Result> Toggle(string host, int channel)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://{host}/rpc/Switch.Toggle?id={channel}");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                ToggleSwitchStatus? result = JsonSerializer.Deserialize<ToggleSwitchStatus>(apiString);
                if (result!.Was_On == true)
                {
                    _logger.LogInformation("Host {host} was switched {onoff}", host, BooleanToOnOff(false));
                    return Result.Ok();
                }
                else
                {
                    _logger.LogInformation("Host {host} was switched {onoff}", host, BooleanToOnOff(true));
                    return Result.Ok();
                }
            }
            else
            {
                var apiString = await response.Content.ReadAsStringAsync();
                _logger.LogError(apiString);
                return Result.Fail(new BadRequestError(apiString));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        return Result.Fail(new BadRequestError("Failed to get status from device"));
    }

    private string BooleanToOnOff(bool onOff) => onOff ? "on" : "off";

    internal record ChannelStatus(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("source")] string Source,
        [property: JsonPropertyName("output")] bool Output,
        [property: JsonPropertyName("temperature")] DeviceTemperature Temperature);

    internal record DeviceTemperature(
        [property: JsonPropertyName("tc")] double Tc,
        [property: JsonPropertyName("tf")] double Tf);

    internal record ToggleSwitchStatus(
        [property: JsonPropertyName("was_on")] bool Was_On);

}
