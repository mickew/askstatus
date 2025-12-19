using System.Text.Json.Serialization;

namespace Askstatus.Domain.Models;
public static class ShellyS3SN_0U12A
{
    private record Status(
        [property: JsonPropertyName("tC")] double? TemperatureCelsius,
        [property: JsonPropertyName("rh")] double? Humidity
    );

    public static bool TryParse(string json, out double value)
    {
        value = default;
        try
        {
            Status? status = System.Text.Json.JsonSerializer.Deserialize<Status>(json);
            if (status?.TemperatureCelsius.HasValue == true)
            {
                value = status.TemperatureCelsius.Value;
            }
            else if (status?.Humidity.HasValue == true)
            {
                value = status.Humidity.Value;
            }

            return value != default;
        }
        catch
        {
            return false;
        }
    }
}
