using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using Askstatus.Common.PowerDevice;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Services;
public sealed class ShellyDiscoverDeviceService : IDiscoverDeviceService
{
    private readonly ILogger<ShellyDiscoverDeviceService> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public ShellyDiscoverDeviceService(ILogger<ShellyDiscoverDeviceService> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task<Result<DicoverInfo>> Discover(string ip)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"http://{ip}/shelly");
        var client = _clientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var apiString = await response.Content.ReadAsStringAsync();
                var info = JsonSerializer.Deserialize<DiscoverResponse>(apiString!);

                if (info!.Gen == 2)
                {
                    return Result.Ok<DicoverInfo>(new(
                        ip, 
                        PowerDeviceTypes.ShellyGen2,
                        info.Name,
                        info.Id,
                        info.Mac,
                        info.Model,
                        info.Gen));
                }
                else
                {
                    return Result.Ok<DicoverInfo>(new(
                        ip,
                        PowerDeviceTypes.ShellyGen1,
                        info.Type,
                        $"{info.Type}-{info.Mac}",
                        info.Mac,
                        info.Type,
                        info.Gen));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
        return Result.Fail<DicoverInfo>( new BadRequestError("Failed to discover device"));
    }
    
    private record DiscoverResponse(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("auth")] bool Auth,
        [property: JsonPropertyName("fw")] string Fw,
        [property: JsonPropertyName("mac")] string Mac,
        [property: JsonPropertyName("slot")] int Slot,
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("gen")] int Gen,
        [property: JsonPropertyName("fw_id")] string FwId,
        [property: JsonPropertyName("ver")] string Ver,
        [property: JsonPropertyName("app")] string App,
        [property: JsonPropertyName("auth_en")] bool AuthEn,
        [property: JsonPropertyName("auth_domain")] object AuthDomain,
        [property: JsonPropertyName("discoverable")] bool Discoverable,
        [property: JsonPropertyName("longid")] int Longid,
        [property: JsonPropertyName("num_outputs")] int NumOutputs
    );

}

