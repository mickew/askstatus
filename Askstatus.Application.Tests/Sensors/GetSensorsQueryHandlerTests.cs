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
                new Sensor { Id = 1, Name = "TempSensor", SensorType = SensorType.Temperature, FormatString = "°C", SensorName = "TS1", SensorModel = "ModelX", ValueName = "Value1" },
                new Sensor { Id = 2, Name = "HumSensor", SensorType = SensorType.Humidity, FormatString = "%", SensorName = "HS1", SensorModel = "ModelY", ValueName = "Value2" }
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
