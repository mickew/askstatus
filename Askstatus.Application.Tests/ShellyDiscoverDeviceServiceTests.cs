using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Askstatus.Application.Errors;
using Askstatus.Application.Services;
using Askstatus.Common.PowerDevice;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Askstatus.Application.Tests;
public class ShellyDiscoverDeviceServiceTests
{
    private const string ShellyName = "SHELLY1";
    private const string ShellyHostName = "10.10.10.10";
    private const string ShellyMac = "EC626081CDF4";
    private const string ShellyModel = "SHELLY1";
    private const string ShellyId = "SHELLY1-EC626081CDF4";

    private const string _shelly1Response = $"{{\"type\": \"{ShellyModel}\", \"mac\": \"{ShellyMac}\", \"fw\": \"20230912-082313/1.0.3-g6176478\", \"auth\": false, \"discoverable\": true, \"longid\": 1, \"num_outputs\": 1}}";
    private const string _shelly2Response = $"{{\"name\": \"{ShellyName}\", \"id\": \"{ShellyId}\", \"mac\": \"{ShellyMac}\", \"model\": \"{ShellyModel}\", \"gen\": 2, \"slot\": 0, \"fw_id\": \"20230912-082313/1.0.3-g6176478\", \"ver\": \"1.0.3\", \"app\": \"Pro1\", \"auth_en\": false, \"auth_domain\": null}}";

    [Theory]
    [InlineData(_shelly1Response, PowerDeviceTypes.ShellyGen1, 0)]
    [InlineData(_shelly2Response, PowerDeviceTypes.ShellyGen2, 2)]
    public async Task Discover_Shelly_Should_Return_DicoverInfo(string shellyResponse, PowerDeviceTypes powerDeviceType, int deviceGen)
    {
        var content = new StringContent(shellyResponse);
        var logger = new Mock<ILogger<ShellyDiscoverDeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);

        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.AbsolutePath.Contains("shelly")),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            })
            .Verifiable();

        var httpClient = new HttpClient(httpMessageHandler.Object);
        httpClientFactory.Setup(_ => _.CreateClient(string.Empty))
        .Returns(httpClient).Verifiable();
        var service = new ShellyDiscoverDeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.Discover("10.10.10.10");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.DeviceHostName.Should().Be(ShellyHostName);
        result.Value.DeviceType.Should().Be(powerDeviceType);
        result.Value.DeviceName.Should().Be(ShellyName);
        result.Value.DeviceId.Should().Be(ShellyId);
        result.Value.DeviceMac.Should().Be(ShellyMac);
        result.Value.DeviceModel.Should().Be(ShellyModel);
        result.Value.DeviceGen.Should().Be(deviceGen);
    }

    [Theory]
    [InlineData(_shelly1Response)]
    [InlineData(_shelly2Response)]
    public async Task Discover_Shelly_Should_Return_BadRequest(string shellyResponse)
    {
        var content = new StringContent(shellyResponse);
        var logger = new Mock<ILogger<ShellyDiscoverDeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);

        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.AbsolutePath.Contains("shelly")),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Not Found")
            })
            .Verifiable();

        var httpClient = new HttpClient(httpMessageHandler.Object);
        httpClientFactory.Setup(_ => _.CreateClient(string.Empty))
        .Returns(httpClient).Verifiable();
        var service = new ShellyDiscoverDeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.Discover("10.10.10.10");

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<BadRequestError>();
    }

    private bool Test(HttpRequestMessage info)
    {
        var b = info.RequestUri!.AbsolutePath.Contains("shelly");
        return b;
    }
}
