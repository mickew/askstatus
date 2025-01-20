using System.ComponentModel.DataAnnotations;

namespace Askstatus.Web.API;

public class AskstatusApiSettings
{
    public const string Section = "AskstatusSettings";

    [Required]
    [Url]
    public string? BackendUrl { get; set; }

    [Required]
    [Url]
    public string? FrontendUrl { get; set; }
}

