using System;
using Askstatus.Domain.Models;
using FluentAssertions;
using Xunit;


namespace Askstatus.Domain.Models.UnitTests;

/// <summary>
/// Unit tests for the ShellyS3SN_0U12A.TryParse method.
/// </summary>
public class ShellyS3SN_0U12ATests
{
    /// <summary>
    /// Tests that TryParse correctly extracts temperature value from valid JSON
    /// with the temperature field present.
    /// </summary>
    /// <param name="json">The JSON input string.</param>
    /// <param name="expectedValue">The expected parsed temperature value.</param>
    [Theory]
    [InlineData("{\"tC\":21.6}", 21.6)]
    [InlineData("{\"tC\":0}", 0.0)]
    [InlineData("{\"tC\":-40.5}", -40.5)]
    [InlineData("{\"tC\":100.0}", 100.0)]
    [InlineData("{\"tC\":0.001}", 0.001)]
    public void TryParse_WithValidTemperatureJson_ReturnsTrueAndSetsTemperatureValue(string json, double expectedValue)
    {
        // Arrange & Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(expectedValue);
    }

    /// <summary>
    /// Tests that TryParse correctly handles special floating point values (Infinity)
    /// for temperature field when AllowNamedFloatingPointLiterals is enabled.
    /// </summary>
    /// <param name="json">The JSON input with special float values.</param>
    /// <param name="expectedValue">The expected parsed value.</param>
    [Theory]
    [InlineData("{\"tC\":\"Infinity\"}", double.PositiveInfinity)]
    [InlineData("{\"tC\":\"-Infinity\"}", double.NegativeInfinity)]
    public void TryParse_WithSpecialFloatValuesInTemperature_ReturnsTrueAndSetsSpecialValue(string json, double expectedValue)
    {
        // Arrange & Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        if (double.IsNaN(expectedValue))
        {
            value.Should().Be(double.NaN);
        }
        else if (double.IsPositiveInfinity(expectedValue))
        {
            value.Should().Be(double.PositiveInfinity);
        }
        else if (double.IsNegativeInfinity(expectedValue))
        {
            value.Should().Be(double.NegativeInfinity);
        }
    }

    /// <summary>
    /// Tests that TryParse correctly extracts humidity value from valid JSON
    /// with the humidity field present (and no temperature field).
    /// </summary>
    /// <param name="json">The JSON input string.</param>
    /// <param name="expectedValue">The expected parsed humidity value.</param>
    [Theory]
    [InlineData("{\"rh\":41.2}", 41.2)]
    [InlineData("{\"rh\":0}", 0.0)]
    [InlineData("{\"rh\":100.0}", 100.0)]
    [InlineData("{\"rh\":50.5}", 50.5)]
    public void TryParse_WithValidHumidityJson_ReturnsTrueAndSetsHumidityValue(string json, double expectedValue)
    {
        // Arrange & Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(expectedValue);
    }

    /// <summary>
    /// Tests that TryParse returns 100 when external power is present (External.Present = true).
    /// </summary>
    [Fact]
    public void TryParse_WithExternalPresentTrue_ReturnsTrueAndSets100()
    {
        // Arrange
        string json = "{\"battery\":{\"V\":5.73,\"percent\":86},\"external\":{\"present\":true}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(100.0);
    }

    /// <summary>
    /// Tests that TryParse returns battery percent when external power is not present
    /// (External.Present = false).
    /// </summary>
    /// <param name="percent">The battery percent value.</param>
    /// <param name="expectedValue">The expected parsed value (battery percent as double).</param>
    [Theory]
    [InlineData(86, 86.0)]
    [InlineData(0, 0.0)]
    [InlineData(100, 100.0)]
    [InlineData(50, 50.0)]
    [InlineData(1, 1.0)]
    public void TryParse_WithExternalPresentFalse_ReturnsTrueAndSetsBatteryPercent(int percent, double expectedValue)
    {
        // Arrange
        string json = $"{{\"battery\":{{\"V\":5.73,\"percent\":{percent}}},\"external\":{{\"present\":false}}}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(expectedValue);
    }

    /// <summary>
    /// Tests that TryParse prioritizes temperature over humidity when both fields are present.
    /// </summary>
    [Fact]
    public void TryParse_WithTemperatureAndHumidity_PrioritizesTemperature()
    {
        // Arrange
        string json = "{\"tC\":21.6,\"rh\":41.2}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(21.6);
    }

    /// <summary>
    /// Tests that TryParse prioritizes temperature over external power when both are present.
    /// </summary>
    [Fact]
    public void TryParse_WithTemperatureAndExternal_PrioritizesTemperature()
    {
        // Arrange
        string json = "{\"tC\":21.6,\"battery\":{\"V\":5.73,\"percent\":86},\"external\":{\"present\":true}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(21.6);
    }

    /// <summary>
    /// Tests that TryParse prioritizes humidity over external power when both are present.
    /// </summary>
    [Fact]
    public void TryParse_WithHumidityAndExternal_PrioritizesHumidity()
    {
        // Arrange
        string json = "{\"rh\":41.2,\"battery\":{\"V\":5.73,\"percent\":86},\"external\":{\"present\":true}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(41.2);
    }

    /// <summary>
    /// Tests that TryParse returns false and sets value to NaN for various invalid inputs.
    /// </summary>
    /// <param name="json">The invalid JSON input.</param>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid json")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("null")]
    [InlineData("{\"unknown\":\"field\"}")]
    [InlineData("{}")]
    [InlineData("[]")]
    public void TryParse_WithInvalidOrEmptyJson_ReturnsFalseAndSetsNaN(string json)
    {
        // Arrange & Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(double.NaN);
    }

    /// <summary>
    /// Tests that TryParse returns false and sets value to NaN when JSON has fields
    /// but all nullable fields are null or missing HasValue.
    /// </summary>
    [Fact]
    public void TryParse_WithAllNullableFieldsNull_ReturnsFalseAndSetsNaN()
    {
        // Arrange
        string json = "{\"tC\":null,\"rh\":null,\"battery\":{\"V\":null,\"percent\":0},\"external\":{\"present\":null}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(double.NaN);
    }

    /// <summary>
    /// Tests that TryParse handles JSON with missing battery field when External.Present is false
    /// by catching the exception and returning false.
    /// </summary>
    [Fact]
    public void TryParse_WithExternalPresentFalseButMissingBattery_ReturnsFalseAndSetsNaN()
    {
        // Arrange
        string json = "{\"external\":{\"present\":false}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(double.NaN);
    }

    /// <summary>
    /// Tests that TryParse handles very long JSON strings without issues.
    /// </summary>
    [Fact]
    public void TryParse_WithVeryLongJsonString_HandlesGracefully()
    {
        // Arrange
        string padding = new string('x', 10000);
        string json = $"{{\"tC\":25.5,\"extra\":\"{padding}\"}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(25.5);
    }

    /// <summary>
    /// Tests that TryParse correctly handles the full example JSON from comments
    /// for temperature sensor data.
    /// </summary>
    [Fact]
    public void TryParse_WithRealTemperatureExample_ReturnsTrueAndSetsValue()
    {
        // Arrange
        string json = "{\"id\": 0,\"tC\":21.6, \"tF\":70.8}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(21.6);
    }

    /// <summary>
    /// Tests that TryParse correctly handles the full example JSON from comments
    /// for humidity sensor data.
    /// </summary>
    [Fact]
    public void TryParse_WithRealHumidityExample_ReturnsTrueAndSetsValue()
    {
        // Arrange
        string json = "{\"id\": 0,\"rh\":41.2}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(41.2);
    }

    /// <summary>
    /// Tests that TryParse correctly handles the full example JSON from comments
    /// for device power data with external power not present.
    /// </summary>
    [Fact]
    public void TryParse_WithRealDevicePowerExample_ReturnsTrueAndSetsBatteryPercent()
    {
        // Arrange
        string json = "{\"id\": 0,\"battery\":{\"V\":5.73, \"percent\":86},\"external\":{\"present\":false}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(86.0);
    }

    /// <summary>
    /// Tests that TryParse handles extreme integer boundary values for battery percent.
    /// </summary>
    /// <param name="percent">The battery percent value.</param>
    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(-1)]
    [InlineData(101)]
    public void TryParse_WithExtremeBatteryPercentValues_ReturnsTrueAndSetsValue(int percent)
    {
        // Arrange
        string json = $"{{\"battery\":{{\"V\":5.73,\"percent\":{percent}}},\"external\":{{\"present\":false}}}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be((double)percent);
    }

    /// <summary>
    /// Tests that TryParse handles extreme double boundary values for temperature.
    /// </summary>
    /// <param name="temperature">The temperature value.</param>
    [Theory]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    [InlineData(double.Epsilon)]
    [InlineData(-double.Epsilon)]
    public void TryParse_WithExtremeTemperatureValues_ReturnsTrueAndSetsValue(double temperature)
    {
        // Arrange
        string json = $"{{\"tC\":{temperature.ToString(System.Globalization.CultureInfo.InvariantCulture)}}}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(temperature);
    }

    /// <summary>
    /// Tests that TryParse handles JSON with special characters and escaped strings.
    /// </summary>
    [Fact]
    public void TryParse_WithSpecialCharactersInJson_HandlesGracefully()
    {
        // Arrange
        string json = "{\"tC\":21.6,\"extra\":\"\\\"test\\\" \\n\\r\\t\"}";

        // Act
        bool result = ShellyS3SN_0U12A.TryParse(json, out double value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(21.6);
    }
}
