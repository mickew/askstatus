using System.Text.Json.Serialization;

namespace Askstatus.Domain.Models;
public static class ShellyS3SN_0U12A
{
    private record Status(
        [property: JsonPropertyName("tC")] double? TemperatureCelsius,
        [property: JsonPropertyName("rh")] double? Humidity,
        [property: JsonPropertyName("battery")] Battery Battery,
        [property: JsonPropertyName("external")] External External
    );

    private record Battery(
    [property: JsonPropertyName("V")] double? V,
    [property: JsonPropertyName("percent")] int Percent
    );

    private record External(
        [property: JsonPropertyName("present")] bool? Present
    );


    // shellies/shellyhtg3-d885ac141c9c/status/temperature:0  {"id": 0,"tC":21.6, "tF":70.8}
    // shellies/shellyhtg3-d885ac141c9c/status/humidity:0   {"id": 0,"rh":41.2}
    // shellies/shellyhtg3-d885ac141c9c/status/devicepower:0   {"id": 0,"battery":{"V":5.73, "percent":86},"external":{"present":false}}

    public static bool TryParse(string json, out double value)
    {
        value = double.NaN;
        try
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
            Status? status = System.Text.Json.JsonSerializer.Deserialize<Status>(json, options);
            if (status?.TemperatureCelsius.HasValue == true)
            {
                value = status.TemperatureCelsius.Value;
            }
            else if (status?.Humidity.HasValue == true)
            {
                value = status.Humidity.Value;
            }
            else if (status?.External?.Present.HasValue == true)
            {
                if (status.External.Present.Value)
                {
                    value = 100; // External power present
                }
                else
                {
                    value = (double)status.Battery.Percent; // External power not present

                }
            }

            return !double.IsNaN(value);
        }
        catch
        {
            return false;
        }
    }
}
