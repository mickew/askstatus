using Askstatus.Application.Sensors;
using FluentAssertions;

namespace Askstatus.Application.Tests;
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

    [Fact]
    public void TryParseValue_ShouldReturnTrue_ForValidDS18B20Value()
    {
        // Arrange
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            SensorModel = "DS18B20"
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
    public void TryParseValue_ShouldReturnFalse_ForInvalidDS18B20Value(string value)
    {
        // Arrange
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            SensorModel = "DS18B20"
        };
        // Act
        var result = ParseSensor.TryParseValue(value, sensor, out double parsedValue);
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TryParseValue_ShouldReturnTrue_ForValidPINUTValue()
    {
        // Arrange
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            SensorModel = "PINUT"
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
    public void TryParseValue_ShouldReturnFalse_ForInvalidPINUTValue(string value)
    {
        // Arrange
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            SensorModel = "PINUT"
        };
        // Act
        var result = ParseSensor.TryParseValue(value, sensor, out double parsedValue);
        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(23.5, "{0:F1} °C", "23.5 °C")]
    [InlineData(55.0, "{0:F0}%", "55%")]
    [InlineData(1013.25, "{0:F2} hPa", "1013.25 hPa")]
    public void TryFormatValue_ShouldReturnTrue_ForValidFormatString(double value, string formatString, string expected)
    {
        // Act
        var result = ParseSensor.TryFormatValue(value, formatString, out string formattedValue);
        // Assert
        result.Should().BeTrue();
        formattedValue.Should().Be(expected);
    }

    [Theory]
    [InlineData(0.0, "")]
    [InlineData(23.5, null)]
    public void TryFormatValue_ShouldReturnFalse_ForEmptyOrNullFormatString(double value, string? formatString)
    {
        // Act
        var result = ParseSensor.TryFormatValue(value, formatString!, out string formattedValue);
        // Assert
        result.Should().BeFalse();
        formattedValue.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0.0, "N")]
    [InlineData(90.0, "E")]
    [InlineData(180.0, "S")]
    [InlineData(270.0, "W")]
    public void TryFormatValue_ShouldFormatWindDirection_ForWindSpeedFormat(double degrees, string expectedDirection)
    {
        // Arrange
        var formatString = "{$$WIND}";
        // Act
        var result = ParseSensor.TryFormatValue(degrees, formatString, out string formattedValue);
        // Assert
        result.Should().BeTrue();
        formattedValue.Should().Be(expectedDirection);
    }
}
