namespace Askstatus.Application.Sensors;

internal static class WindDirectionConverter
{
    // 16-Point Compass Directions
    private static readonly string[] CompassPoints16 =
    {
        "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE",
        "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW"
    };

    // 8-Point Compass Directions
    private static readonly string[] CompassPoints8 =
    {
        "N", "NE", "E", "SE", "S", "SW", "W", "NW"
    };

    /// <summary>
    /// Converts wind degrees (0-360) to a compass direction.
    /// </summary>
    /// <param name="degrees">Wind angle in degrees.</param>
    /// <param name="use16Points">If true, returns 16-point resolution (e.g. NNE). If false, returns 8-point (e.g. NE).</param>
    public static string DegreesToDirection(double degrees, bool use16Points = true)
    {
        string[] points = use16Points ? CompassPoints16 : CompassPoints8;

        // Normalize degrees into 0..359 range (handles values like -10 or 370)
        double normalizedDegrees = (degrees % 360 + 360) % 360;

        // Calculate step size (22.5° for 16-point, 45° for 8-point)
        double step = 360.0 / points.Length;

        // Offset by half a step so that "North" is centered around 0° (e.g. 348.75° to 11.25° for 16-point)
        int index = (int)Math.Floor((normalizedDegrees + (step / 2.0)) / step) % points.Length;

        return points[index];
    }
}
