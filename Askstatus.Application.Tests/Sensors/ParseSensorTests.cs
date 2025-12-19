using Askstatus.Application.Sensors;
using FluentAssertions;

namespace Askstatus.Application.Tests.Sensors;
public class ParseSensorTests
{
    [Fact]
    public void TryParseValue_ShouldReturnTrue_ForValidSHHT1Value()
    {
        // Arrange
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            SensorModel = "SHHT-1"
        };
        var value = "23.5";
        // Act
        var result = ParseSensor.TryParseValue(value, sensor, out double parsedValue);
        // Assert
        result.Should().BeTrue();
        parsedValue.Should().Be(23.5);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    public void TryParseValue_ShouldReturnFalse_ForInvalidSHHT1Value(string value)
    {
        // Arrange
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            SensorModel = "SHHT-1"
        };
        // Act
        var result = ParseSensor.TryParseValue(value, sensor, out double parsedValue);
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TryParseValue_ShouldReturnTrue_ForValidS3SN0U12AValue()
    {
        // Arrange
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            SensorModel = "S3SN-0U12A"
        };
        var value = "{\"tC\":22.3,\"rh\":55.0}";
        // Act
        var result = ParseSensor.TryParseValue(value, sensor, out double parsedValue);
        // Assert
        result.Should().BeTrue();
        parsedValue.Should().Be(22.3);
    }

    [Theory]
    [InlineData("")]
    [InlineData("{\"invalid_json\":}")]
    public void TryParseValue_ShouldReturnFalse_ForInvalidS3SN0U12AValue(string value)
    {
        // Arrange
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            SensorModel = "S3SN-0U12A"
        };
        // Act
        var result = ParseSensor.TryParseValue(value, sensor, out double parsedValue);
        // Assert
        result.Should().BeFalse();
    }
}
