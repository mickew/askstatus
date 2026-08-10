using System.Text.RegularExpressions;

namespace Askstatus.Infrastructure.Services;

public static partial class MqttTopicMatcher
{
    // The parentheses added here define Capture Group 1
    [GeneratedRegex(@"^[^/]+/([^/]+)/online$")]
    public static partial Regex OnlineTopicRegex();

    [GeneratedRegex(@"^shellies\/(.+)\/status\/(.+)$")]
    public static partial Regex ShellieStatusTopicRegex();

    [GeneratedRegex(@"^shellies\/(.+)\/status\/input\S\d$")]
    public static partial Regex ShellieStatusInputTopicRegex();

    [GeneratedRegex(@"^shellies\/(.+)\/status\/eth$")]
    public static partial Regex ShellieStatusEthTopicRegex();

    [GeneratedRegex(@"^shellies\/(.+)\/sensor\/(.+)$")]
    public static partial Regex ShellieSensorTopicRegex();

    [GeneratedRegex(@"^pitemps\/(.+)\/status\/(.+)$")]
    public static partial Regex PiTempTopicRegex();

    [GeneratedRegex(@"^nutups\/(.+)\/status\/(.+)$")]
    public static partial Regex NutUpsSensorTopicRegex();

    [GeneratedRegex(@"^telldus\/(.+)\/status\/(.+)$")]
    public static partial Regex TelldusSensorTopicRegex();

    [GeneratedRegex(@"^speedtest\/(.+)\/status\/(.+)$")]
    public static partial Regex SpeedtestSensorTopicRegex();
}
