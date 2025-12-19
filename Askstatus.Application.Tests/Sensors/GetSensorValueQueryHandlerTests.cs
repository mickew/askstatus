using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Askstatus.Application.Tests
{
    public class GetSensorValueQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMqttClientService> _mqttClientServiceMock;
        private readonly Mock<ILogger<GetSensorValueQueryHandler>> _loggerMock;
        private readonly GetSensorValueQueryHandler _handler;

        public GetSensorValueQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mqttClientServiceMock = new Mock<IMqttClientService>();
            _loggerMock = new Mock<ILogger<GetSensorValueQueryHandler>>();
            _handler = new GetSensorValueQueryHandler(_unitOfWorkMock.Object, _mqttClientServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenSensorDoesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.SensorRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Askstatus.Domain.Entities.Sensor?)null!);
            var query = new GetSensorValueQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e is NotFoundError);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Sensor with id 1 not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenSensorValueDoesNotExist()
        {
            // Arrange
            var sensorEntity = new Askstatus.Domain.Entities.Sensor
            {
                Id = 1,
                SensorName = "sensor1",
                ValueName = "temp",
                Name = "TestSensor",
                SensorType = 0,
                FormatString = string.Empty,
                SensorModel = "model"
            };
            _unitOfWorkMock.Setup(u => u.SensorRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Askstatus.Domain.Entities.Sensor)sensorEntity);
            _mqttClientServiceMock.Setup(m => m.GetSensorsAsync())
                .ReturnsAsync(new List<DeviceSensor> {
                    new DeviceSensor("sensor1", "TestSensor", "model", new List<DeviceSensorValue>())
                });
            var query = new GetSensorValueQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e is NotFoundError);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains($"Sensor value {sensorEntity.ValueName} for sensor {sensorEntity.SensorName} not found")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldApplyFormatString_WhenFormatStringIsProvided()
        {
            // Arrange
            var sensorEntity = new Askstatus.Domain.Entities.Sensor
            {
                Id = 1,
                SensorName = "sensor1",
                ValueName = "temp",
                Name = "TestSensor",
                SensorType = 0,
                FormatString = "Value: {0}",
                SensorModel = "SHHT-1"
            };
            var now = DateTime.UtcNow;
            var sensorValue = new DeviceSensorValue("temp", "42", now);
            _unitOfWorkMock.Setup(u => u.SensorRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Askstatus.Domain.Entities.Sensor)sensorEntity);
            _mqttClientServiceMock.Setup(m => m.GetSensorsAsync())
                .ReturnsAsync(new List<DeviceSensor> {
                    new DeviceSensor("sensor1", "TestSensor", "model", new List<DeviceSensorValue> { sensorValue })
                });
            var query = new GetSensorValueQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be($"Value: {sensorValue.Value}");
        }

        [Fact]
        public async Task Handle_ShouldReturnRawValue_WhenFormatStringIsNullOrEmpty()
        {
            // Arrange
            var sensorEntity = new Askstatus.Domain.Entities.Sensor
            {
                Id = 1,
                SensorName = "sensor1",
                ValueName = "temp",
                Name = "TestSensor",
                SensorType = 0,
                FormatString = string.Empty,
                SensorModel = "model"
            };
            var now = DateTime.UtcNow;
            var sensorValue = new DeviceSensorValue("temp", "42", now);
            _unitOfWorkMock.Setup(u => u.SensorRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Askstatus.Domain.Entities.Sensor)sensorEntity);
            _mqttClientServiceMock.Setup(m => m.GetSensorsAsync())
                .ReturnsAsync(new List<DeviceSensor> {
                    new DeviceSensor("sensor1", "TestSensor", "model", new List<DeviceSensorValue> { sensorValue })
                });
            var query = new GetSensorValueQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(sensorValue.Value);
        }

        [Fact]
        public async Task Handle_ShouldReturnServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.SensorRepository.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));
            var query = new GetSensorValueQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e is ServerError);
        }
    }
}
