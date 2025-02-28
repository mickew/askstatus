﻿using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Askstatus.Application.DiscoverDevice;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using Askstatus.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Askstatus.Infrastructure.Services;

internal class MqttClientService : IMqttClientService
{
    private readonly ConcurrentDictionary<string, ShellieAnnounce> _shellieAnnounces = new ConcurrentDictionary<string, ShellieAnnounce>();
    private readonly ConcurrentDictionary<string, DeviceSensor> _sensors = new ConcurrentDictionary<string, DeviceSensor>();
    private readonly ILogger<MqttClientService> _logger;
    private readonly IOptions<AskstatusApiSettings> _apiOptions;
    private readonly MqttClientOptions _options;
    private readonly IMqttClient _mqttClient;

    public MqttClientService(ILogger<MqttClientService> logger, IOptions<AskstatusApiSettings> apiOptions)
    {
        _logger = logger;
        _apiOptions = apiOptions;
        _options = new MqttClientOptionsBuilder()
            .WithTcpServer(_apiOptions.Value.MQTTServer, _apiOptions.Value.MQTTPort)
            .WithClientId(_apiOptions.Value.MQTTClientId)
            .Build();
        _mqttClient = new MqttClientFactory().CreateMqttClient();
        ConfigureMqttClient();
    }

    public async Task<IEnumerable<DeviceSensor>> GetSensorsAsync()
    {
        return await Task.FromResult(_sensors.Select(x => x.Value).ToList());
    }

    public async Task<IEnumerable<ShellieAnnounce>> GetShellieDevicesAsync()
    {
        return await Task.FromResult(_shellieAnnounces.Select(x => x.Value).ToList());
    }

    public async Task RefreshShellieDevicesAsync()
    {
        _shellieAnnounces.Clear();
        await PublishAsync();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MqttClientService starting...");
        try
        {
            await _mqttClient.ConnectAsync(_options);
        }
        catch (Exception)
        {
        }
        _ = Task.Run(
           async () =>
           {
               while (!cancellationToken.IsCancellationRequested)
               {
                   try
                   {
                       // This code will also do the very first connect! So no call to _ConnectAsync_ is required in the first place.
                       if (!await _mqttClient.TryPingAsync())
                       {
                           await _mqttClient.ConnectAsync(_mqttClient.Options, CancellationToken.None);

                           // Subscribe to topics when session is clean etc.
                           _logger.LogInformation("The MQTT client is connected.");
                       }
                   }
                   catch (Exception ex)
                   {
                       // Handle the exception properly (logging etc.).
                       _logger.LogError(ex, "The MQTT client  connection failed");
                   }
                   finally
                   {
                       // Check the connection state every 5 seconds and perform a reconnect if required.
                       await Task.Delay(TimeSpan.FromSeconds(5));
                   }
               }
           });
        _logger.LogInformation("MqttClientService started");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MqttClientService stopping....");
        if (cancellationToken.IsCancellationRequested)
        {
            var disconnectOption = new MqttClientDisconnectOptions
            {
                Reason = MqttClientDisconnectOptionsReason.NormalDisconnection,
                ReasonString = "NormalDiconnection"
            };
            await _mqttClient.DisconnectAsync(disconnectOption, cancellationToken);
        }
        await _mqttClient.DisconnectAsync();
        _logger.LogInformation("MqttClientService stopped");
    }

    private void ConfigureMqttClient()
    {
        _mqttClient.ConnectedAsync += HandleConnectedAsync;
        _mqttClient.DisconnectedAsync += HandleDisconnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;
    }

    private async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        await ParseTopic(args.ApplicationMessage.Topic, args.ApplicationMessage.ConvertPayloadToString());
    }

    private async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        _logger.LogInformation("MQTTClient disconnected from server");
        await Task.CompletedTask;
    }

    private async Task HandleConnectedAsync(MqttClientConnectedEventArgs args)
    {
        _logger.LogInformation("MQTTClient connected to server");
        await _mqttClient.SubscribeAsync("shellies/announce");
        await _mqttClient.SubscribeAsync("shellies/+/status/eth");
        await _mqttClient.SubscribeAsync("shellies/+/sensor/#");
        await PublishAsync();
    }

    private async Task ParseTopic(string topic, string payload)
    {
        if (topic == "shellies/announce")
        {
            await ProcessAnnounce(payload);
            return;
        }
        Regex rg = new Regex(@"^shellies\/(.+)\/status\/eth$");
        Match match = rg.Match(topic);
        if (match.Success)
        {
            await ProcessEth(match.Groups[1].Value, payload);
            return;
        }
        rg = new Regex(@"^shellies\/(.+)\/sensor\/(.+)$");
        match = rg.Match(topic);
        if (match.Success)
        {
            await ProcessSensor(match.Groups[1].Value, match.Groups[2].Value, payload);
            return;
        }
        await Task.CompletedTask;
    }

    private async Task ProcessSensor(string id, string value2, string payload)
    {
        _logger.LogInformation($"ProcessSensor: {id} {value2} {payload}");
        if (_sensors.ContainsKey(id))
        {
            var sensorValue = new DeviceSensorValue(value2, payload, DateTime.UtcNow);
            var sensor = _sensors[id];
            sensor.Values.RemoveAll(x => x.Name == value2);
            sensor.Values.Add(sensorValue);
        }
        else
        {
            _sensors.TryAdd(id, new DeviceSensor(id, new List<DeviceSensorValue>() { new DeviceSensorValue(value2, payload, DateTime.UtcNow) }));
        }
        await Task.CompletedTask;
    }

    private async Task ProcessEth(string id, string payload)
    {
        ShellieAnnounce? theItem = _shellieAnnounces.Select(v => v.Value).FirstOrDefault(x => x.Ip == null && x.Id == id);
        if (theItem != null)
        {
            var eth = JsonSerializer.Deserialize<Eth>(payload);
            var newItem = theItem with { Ip = eth!.Ip };
            _shellieAnnounces.TryUpdate(newItem.Id, newItem, theItem);
        }
        await Task.CompletedTask;
    }

    private async Task ProcessAnnounce(string payload)
    {
        var shellieAnnounce = JsonSerializer.Deserialize<ShellieAnnounce>(payload);
        if (shellieAnnounce != null && !_shellieAnnounces.Any(x => x.Key == shellieAnnounce.Id))
        {
            _shellieAnnounces.TryAdd(shellieAnnounce.Id, shellieAnnounce);
        }
        await Task.CompletedTask;
    }

    private async Task PublishAsync()
    {
        await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
            .WithTopic("shellies/command")
            .WithPayload("announce")
            .Build());
        await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
            .WithTopic("shellies/command")
            .WithPayload("status_update")
            .Build());
    }

    private record Eth(
        [property: JsonPropertyName("ip")] string Ip
    );
}
