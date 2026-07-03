using Askstatus.Infrastructure.Services;
using FluentAssertions;

namespace Askstatus.Infrastructure.Tests;

public class MqttTopicMatcherTests
{
    [Theory]
    [InlineData("home/livingroom/online", true, "livingroom")]
    [InlineData("prefix/device123/online", true, "device123")]
    [InlineData("a/b/online", true, "b")]
    [InlineData("mqtt/my-device/online", true, "my-device")]
    [InlineData("shellies/device/online", true, "device")]
    [InlineData("home/livingroom/offline", false, null)]
    [InlineData("home/livingroom/online/extra", false, null)]
    [InlineData("/livingroom/online", false, null)]
    [InlineData("home/sub/path/online", false, null)]
    [InlineData("noslash", false, null)]
    [InlineData("", false, null)]
    public void OnlineTopicRegex_ShouldMatchCorrectTopics(string topic, bool shouldMatch, string? expectedGroup1)
    {
        // Arrange
        var regex = MqttTopicMatcher.OnlineTopicRegex();

        // Act
        var match = regex.Match(topic);

        // Assert
        match.Success.Should().Be(shouldMatch);
        if (shouldMatch)
        {
            match.Groups[1].Value.Should().Be(expectedGroup1);
        }
    }

    [Theory]
    [InlineData("shellies/shelly1/status/switch:0", true, "shelly1", "switch:0")]
    [InlineData("shellies/shellyplug-s-12345/status/temperature", true, "shellyplug-s-12345", "temperature")]
    [InlineData("shellies/device/status/input:0", true, "device", "input:0")]
    [InlineData("shellies/a/status/b", true, "a", "b")]
    [InlineData("shellies/dev/name/status/eth", true, "dev/name", "eth")]
    [InlineData("notshellies/device/status/switch:0", false, null, null)]
    [InlineData("shellies/device/other/switch:0", false, null, null)]
    [InlineData("shellies//status/", false, null, null)]
    [InlineData("shellies/device/status/", false, null, null)]
    [InlineData("", false, null, null)]
    public void ShellieStatusTopicRegex_ShouldMatchCorrectTopics(string topic, bool shouldMatch, string? expectedGroup1, string? expectedGroup2)
    {
        // Arrange
        var regex = MqttTopicMatcher.ShellieStatusTopicRegex();

        // Act
        var match = regex.Match(topic);

        // Assert
        match.Success.Should().Be(shouldMatch);
        if (shouldMatch)
        {
            match.Groups[1].Value.Should().Be(expectedGroup1);
            match.Groups[2].Value.Should().Be(expectedGroup2);
        }
    }

    [Theory]
    [InlineData("shellies/shelly1/status/input:0", true, "shelly1")]
    [InlineData("shellies/device/status/input:1", true, "device")]
    [InlineData("shellies/shellyplug-s-12345/status/input_9", true, "shellyplug-s-12345")]
    [InlineData("shellies/dev/status/input-5", true, "dev")]
    [InlineData("shellies/device/status/inputX3", true, "device")]
    [InlineData("shellies/device/status/input 0", false, null)]
    [InlineData("shellies/device/status/input:00", false, null)]
    [InlineData("shellies/device/status/input:", false, null)]
    [InlineData("notshellies/device/status/input:0", false, null)]
    [InlineData("shellies/device/other/input:0", false, null)]
    [InlineData("", false, null)]
    public void ShellieStatusInputTopicRegex_ShouldMatchCorrectTopics(string topic, bool shouldMatch, string? expectedGroup1)
    {
        // Arrange
        var regex = MqttTopicMatcher.ShellieStatusInputTopicRegex();

        // Act
        var match = regex.Match(topic);

        // Assert
        match.Success.Should().Be(shouldMatch);
        if (shouldMatch)
        {
            match.Groups[1].Value.Should().Be(expectedGroup1);
        }
    }

    [Theory]
    [InlineData("shellies/shelly1/status/eth", true, "shelly1")]
    [InlineData("shellies/shellyplug-s-12345/status/eth", true, "shellyplug-s-12345")]
    [InlineData("shellies/my-device/status/eth", true, "my-device")]
    [InlineData("shellies/dev/name/status/eth", true, "dev/name")]
    [InlineData("shellies/device/status/ethernet", false, null)]
    [InlineData("shellies/device/status/et", false, null)]
    [InlineData("shellies/device/status/ETH", false, null)]
    [InlineData("notshellies/device/status/eth", false, null)]
    [InlineData("shellies/device/other/eth", false, null)]
    [InlineData("", false, null)]
    public void ShellieStatusEthTopicRegex_ShouldMatchCorrectTopics(string topic, bool shouldMatch, string? expectedGroup1)
    {
        // Arrange
        var regex = MqttTopicMatcher.ShellieStatusEthTopicRegex();

        // Act
        var match = regex.Match(topic);

        // Assert
        match.Success.Should().Be(shouldMatch);
        if (shouldMatch)
        {
            match.Groups[1].Value.Should().Be(expectedGroup1);
        }
    }

    [Theory]
    [InlineData("shellies/shelly1/sensor/temperature", true, "shelly1", "temperature")]
    [InlineData("shellies/shellyplug-s-12345/sensor/humidity", true, "shellyplug-s-12345", "humidity")]
    [InlineData("shellies/device/sensor/battery", true, "device", "battery")]
    [InlineData("shellies/dev/name/sensor/voltage", true, "dev/name", "voltage")]
    [InlineData("notshellies/device/sensor/temperature", false, null, null)]
    [InlineData("shellies/device/other/temperature", false, null, null)]
    [InlineData("shellies/device/sensor/", false, null, null)]
    [InlineData("shellies//sensor/", false, null, null)]
    [InlineData("", false, null, null)]
    public void ShellieSensorTopicRegex_ShouldMatchCorrectTopics(string topic, bool shouldMatch, string? expectedGroup1, string? expectedGroup2)
    {
        // Arrange
        var regex = MqttTopicMatcher.ShellieSensorTopicRegex();

        // Act
        var match = regex.Match(topic);

        // Assert
        match.Success.Should().Be(shouldMatch);
        if (shouldMatch)
        {
            match.Groups[1].Value.Should().Be(expectedGroup1);
            match.Groups[2].Value.Should().Be(expectedGroup2);
        }
    }

    [Theory]
    [InlineData("pitemps/pi1/status/temperature", true, "pi1", "temperature")]
    [InlineData("pitemps/raspberry-pi-4/status/cpu_temp", true, "raspberry-pi-4", "cpu_temp")]
    [InlineData("pitemps/device/status/humidity", true, "device", "humidity")]
    [InlineData("pitemps/dev/name/status/sensor1", true, "dev/name", "sensor1")]
    [InlineData("pitemps/my-pi/status/temp:0", true, "my-pi", "temp:0")]
    [InlineData("notpitemps/pi1/status/temperature", false, null, null)]
    [InlineData("pitemps/device/other/temperature", false, null, null)]
    [InlineData("pitemps/device/status/", false, null, null)]
    [InlineData("pitemps//status/", false, null, null)]
    [InlineData("", false, null, null)]
    public void PiTempTopicRegex_ShouldMatchCorrectTopics(string topic, bool shouldMatch, string? expectedGroup1, string? expectedGroup2)
    {
        // Arrange
        var regex = MqttTopicMatcher.PiTempTopicRegex();

        // Act
        var match = regex.Match(topic);

        // Assert
        match.Success.Should().Be(shouldMatch);
        if (shouldMatch)
        {
            match.Groups[1].Value.Should().Be(expectedGroup1);
            match.Groups[2].Value.Should().Be(expectedGroup2);
        }
    }

    [Theory]
    [InlineData("nutups/ups1/status/battery", true, "ups1", "battery")]
    [InlineData("nutups/my-ups-device/status/voltage", true, "my-ups-device", "voltage")]
    [InlineData("nutups/device/status/charge", true, "device", "charge")]
    [InlineData("nutups/dev/name/status/load", true, "dev/name", "load")]
    [InlineData("nutups/ups-1/status/input:0", true, "ups-1", "input:0")]
    [InlineData("notnutups/ups1/status/battery", false, null, null)]
    [InlineData("nutups/device/other/battery", false, null, null)]
    [InlineData("nutups/device/status/", false, null, null)]
    [InlineData("nutups//status/", false, null, null)]
    [InlineData("", false, null, null)]
    public void NutUpsSensorTopicRegex_ShouldMatchCorrectTopics(string topic, bool shouldMatch, string? expectedGroup1, string? expectedGroup2)
    {
        // Arrange
        var regex = MqttTopicMatcher.NutUpsSensorTopicRegex();

        // Act
        var match = regex.Match(topic);

        // Assert
        match.Success.Should().Be(shouldMatch);
        if (shouldMatch)
        {
            match.Groups[1].Value.Should().Be(expectedGroup1);
            match.Groups[2].Value.Should().Be(expectedGroup2);
        }
    }
}
