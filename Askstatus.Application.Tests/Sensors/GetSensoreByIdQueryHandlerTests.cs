using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using Askstatus.Common.Sensor;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests;

public class GetSensoreByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetSensoreByIdQueryHandler>> _loggerMock;
    private readonly Mock<IRepository<Askstatus.Domain.Entities.Sensor>> _sensorRepositoryMock;
    private readonly GetSensoreByIdQueryHandler _handler;

    public GetSensoreByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetSensoreByIdQueryHandler>>();
        _sensorRepositoryMock = new Mock<IRepository<Askstatus.Domain.Entities.Sensor>>();
        _unitOfWorkMock.Setup(u => u.SensorRepository).Returns(_sensorRepositoryMock.Object);
        _handler = new GetSensoreByIdQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSensorDto_WhenSensorExists()
    {
        // Arrange
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            Id = 1,
            Name = "TestSensor",
            SensorType = SensorType.Temperature,
            FormatString = "°C",
            SensorName = "TempSensor",
            ValueName = "Value1"
        };
        _sensorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sensor);
        var query = new GetSensoreByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(sensor.Id);
        result.Value.Name.Should().Be(sensor.Name);
        result.Value.SensorType.Should().Be(sensor.SensorType);
        result.Value.FormatString.Should().Be(sensor.FormatString);
        result.Value.SensorName.Should().Be(sensor.SensorName);
        result.Value.ValueName.Should().Be(sensor.ValueName);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenSensorDoesNotExist()
    {
        // Arrange
        _sensorRepositoryMock.Setup(r => r.GetByIdAsync(2)).Returns(Task.FromResult<Askstatus.Domain.Entities.Sensor>(null!));
        var query = new GetSensoreByIdQuery(2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is NotFoundError && e.Message == "Sensor not found");
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Sensor with id 2 not found")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
}
