using System.ComponentModel.DataAnnotations;

namespace Askstatus.Domain;

public class AskstatusApiSettings
{
    public const string Section = "AskstatusSettings";

    [Required]
    [Url]
    public string? BackendUrl { get; set; }

    [Required]
    [Url]
    public string? FrontendUrl { get; set; }

    [Required]
    public string? MQTTServer { get; set; }

    [Required]
    public int MQTTPort { get; set; }

    [Required]
    public string? MQTTClientId { get; set; }
}

