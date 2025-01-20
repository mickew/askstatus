using System.ComponentModel.DataAnnotations;

namespace Askstatus.Web.App;

public class AskstatusSettings
{
    public const string Section = "AskstatusSettings";

    [Required]
    [Url]
    public string? AskstatusUrl { get; set; }

    [Required]
    [Url]
    public string? AskStatusSignalRUrl { get; set; }

}
