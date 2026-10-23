using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using Askstatus.Common.Sensor;
using Askstatus.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Askstatus.Application.Tests
{
    public class GetSensorsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsSensorDtos_WhenSensorsExist()
        {
            // Arrange
            var sensors = new List<Sensor>
            {
                new Sensor { Id = 1, Name = "TempSensor", SensorType = SensorType.Temperature, FormatString = "°C", SensorName = "TS1", SensorModel = "ModelX", ValueName = "Value1", MQTTEnabled = true },
                new Sensor { Id = 2, Name = "HumSensor", SensorType = SensorType.Humidity, FormatString = "%", SensorName = "HS1", SensorModel = "ModelY", ValueName = "Value2", MQTTEnabled = false }
            };

            var sensorRepoMock = new Mock<IRepository<Sensor>>();
            sensorRepoMock.Setup(r => r.ListAllAsync()).ReturnsAsync(sensors);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SensorRepository).Returns(sensorRepoMock.Object);

            var handler = new GetSensorsQueryHandler(unitOfWorkMock.Object);
            var query = new GetSensorsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.First().Id.Should().Be(1);
            result.Value.First().Name.Should().Be("TempSensor");
            result.Value.First().SensorType.Should().Be(SensorType.Temperature);
            result.Value.First().FormatString.Should().Be("°C");
            result.Value.First().SensorName.Should().Be("TS1");
            result.Value.First().SensorModel.Should().Be("ModelX");
            result.Value.First().ValueName.Should().Be("Value1");
            result.Value.First().MQTTEnabled.Should().Be(true);
            result.Value.Last().Id.Should().Be(2);
            result.Value.Last().Name.Should().Be("HumSensor");
            result.Value.Last().SensorType.Should().Be(SensorType.Humidity);
            result.Value.Last().FormatString.Should().Be("%");
            result.Value.Last().SensorName.Should().Be("HS1");
            result.Value.Last().SensorModel.Should().Be("ModelY");
            result.Value.Last().ValueName.Should().Be("Value2");
            result.Value.Last().MQTTEnabled.Should().Be(false);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenNoSensorsExist()
        {
            // Arrange
            var sensorRepoMock = new Mock<IRepository<Sensor>>();
            sensorRepoMock.Setup(r => r.ListAllAsync()).ReturnsAsync(new List<Sensor>());

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.SensorRepository).Returns(sensorRepoMock.Object);

            var handler = new GetSensorsQueryHandler(unitOfWorkMock.Object);
            var query = new GetSensorsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }
}
