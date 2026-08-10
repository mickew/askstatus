using System.ComponentModel.DataAnnotations;

namespace Askstatus.Common.Sensor;
public enum SensorType
{
    [Display(Name = "Unknown")]
    Unknown,
    [Display(Name = "Temperature")]
    Temperature,
    [Display(Name = "Humidity")]
    Humidity,
    [Display(Name = "Battery")]
    Battery,
    [Display(Name = "Status")]
    Status,
    [Display(Name = "Power In")]
    PowerIn,
    [Display(Name = "Power Out")]
    PowerOut,
    [Display(Name = "Load Watts")]
    LoadWatts,
    [Display(Name = "Load Percent")]
    LoadPercent,
    [Display(Name = "Runtime")]
    Runtime,
    [Display(Name = "Battery Usage")]
    BatteryUsage,
    [Display(Name = "Wind Speed Average")]
    WindSpeedAverage,
    [Display(Name = "Wind Speed Gust")]
    WindSpeedGust,
    [Display(Name = "Wind Direction")]
    WindDirection,
    [Display(Name = "Rain Rate")]
    RainRate,
    [Display(Name = "Ping Latency")]
    PingLatency,
    [Display(Name = "Download Speed")]
    DownloadSpeed,
    [Display(Name = "Upload Speed")]
    UploadSpeed,
    [Display(Name = "ISP")]
    Isp,
    [Display(Name = "Server")]
    Server,
}
