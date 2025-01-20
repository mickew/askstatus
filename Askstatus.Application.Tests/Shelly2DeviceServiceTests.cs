using System.Net;
using Askstatus.Application.Errors;
using Askstatus.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Askstatus.Application.Tests;
public class Shelly2DeviceServiceTests
{
    [Fact]
    public async Task GetStatus_Should_Return_Success()
    {
        // Arrange
        var content = new StringContent("{\"id\": 0, \"source\": \"HTTP_in\", \"output\": true, \"temperature\":{ \"tc\": 24.6, \"tf\": 76.3 }}");
        var logger = new Mock<ILogger<Shelly2DeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
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
        var service = new Shelly2DeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.State("10.10.10.10", 0);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetStatus_Should_Return_BadReqest()
    {
        // Arrange
        var content = new StringContent("{\"code\": -105,\"message\": \"Argument 'id', value 1 not found!\"}");
        var logger = new Mock<ILogger<Shelly2DeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = content
            })
            .Verifiable();

        var httpClient = new HttpClient(httpMessageHandler.Object);
        httpClientFactory.Setup(_ => _.CreateClient(string.Empty))
        .Returns(httpClient).Verifiable();
        var service = new Shelly2DeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.State("10.10.10.10", 1);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<BadRequestError>();
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Argument 'id', value 1 not found!")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Switch_Should_Return_Success(bool turnOn)
    {
        // Arrange
        var content = new StringContent($"{{\"was_on\": {(!turnOn).ToString().ToLower()}}}");
        var logger = new Mock<ILogger<Shelly2DeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
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
        var service = new Shelly2DeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.Switch("10.10.10.10", 0, turnOn);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        logger.Verify(l =>
            l.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains($"Host 10.10.10.10 was switched {BooleanToOnOff(turnOn)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Switch_Should_Return_Success_With_Warning(bool turnOn)
    {
        // Arrange
        var content = new StringContent($"{{\"was_on\": {(turnOn).ToString().ToLower()}}}");
        var logger = new Mock<ILogger<Shelly2DeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
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
        var service = new Shelly2DeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.Switch("10.10.10.10", 0, turnOn);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        logger.Verify(l =>
            l.Log(LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains($"Host 10.10.10.10 was alreddy {BooleanToOnOff(turnOn)} when switching {BooleanToOnOff(turnOn)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task Switch_Should_Return_BadRequest()
    {
        // Arrange
        var content = new StringContent("{\"code\": -105,\"message\": \"Argument 'id', value 1 not found!\"}");
        var logger = new Mock<ILogger<Shelly2DeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = content
            })
            .Verifiable();

        var httpClient = new HttpClient(httpMessageHandler.Object);
        httpClientFactory.Setup(_ => _.CreateClient(string.Empty))
        .Returns(httpClient).Verifiable();
        var service = new Shelly2DeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.Switch("10.10.10.10", 1, true);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<BadRequestError>();
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Argument 'id', value 1 not found!")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Toggle_Should_Return_Success(bool turnOn)
    {
        // Arrange
        var content = new StringContent($"{{\"was_on\": {(!turnOn).ToString().ToLower()}}}");
        var logger = new Mock<ILogger<Shelly2DeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
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
        var service = new Shelly2DeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.Toggle("10.10.10.10", 0);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        logger.Verify(l =>
            l.Log(LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains($"Host 10.10.10.10 was switched {BooleanToOnOff(turnOn)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public async Task Toggle_Should_Return_BadRequest()
    {
        // Arrange
        var content = new StringContent("{\"code\": -105,\"message\": \"Argument 'id', value 1 not found!\"}");
        var logger = new Mock<ILogger<Shelly2DeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = content
            })
            .Verifiable();

        var httpClient = new HttpClient(httpMessageHandler.Object);
        httpClientFactory.Setup(_ => _.CreateClient(string.Empty))
        .Returns(httpClient).Verifiable();
        var service = new Shelly2DeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.Toggle("10.10.10.10", 1);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<BadRequestError>();
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Argument 'id', value 1 not found!")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    private string BooleanToOnOff(bool onOff) => onOff ? "on" : "off";

    [Fact]
    public async Task GetWebHooks_Should_Return_Success()
    {
        // Arrange
        string reqRetunString = @"{""hooks"": [{""id"": 1,""cid"": 0,""enable"": true,""event"": ""switch.on"",""name"": ""OnHookSender"",""ssl_ca"": ""ca.pem"",""urls"": [""http://10.10.10.10/api/PowerDevice/webhook?mac=${config.sys.device.mac}&state=${status[\""switch:0\""].output}""],""condition"": null,""repeat_period"": 0},{""id"": 2,""cid"": 0,""enable"": true,""event"": ""switch.off"",""name"": ""OffHookSender"",""ssl_ca"": ""ca.pem"",""urls"": [""http://10.10.10.10/api/PowerDevice/webhook?mac=${config.sys.device.mac}&state=${status[\""switch:0\""].output}""],""condition"": null,""repeat_period"": 0}],""rev"": 32}";
        var content = new StringContent(reqRetunString);
        var logger = new Mock<ILogger<Shelly2DeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
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
        var service = new Shelly2DeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.GetWebHooks("10.10.10.10");

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWebHooks_Should_Return_BadRequest()
    {
        // Arrange
        var content = new StringContent("{\"code\": -105,\"message\": \"Argument 'id', value 1 not found!\"}");
        var logger = new Mock<ILogger<Shelly2DeviceService>>();
        var httpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
        var httpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandler.Protected()
            // Setup the PROTECTED method to mock
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = content
            })
            .Verifiable();

        var httpClient = new HttpClient(httpMessageHandler.Object);
        httpClientFactory.Setup(_ => _.CreateClient(string.Empty))
        .Returns(httpClient).Verifiable();
        var service = new Shelly2DeviceService(logger.Object, httpClientFactory.Object);

        // Act
        var result = await service.GetWebHooks("10.10.10.10");

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.First().Should().BeOfType<BadRequestError>();
        logger.Verify(l =>
        l.Log(LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Argument 'id', value 1 not found!")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
}


