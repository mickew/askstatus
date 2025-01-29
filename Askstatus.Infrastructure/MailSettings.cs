using System.ComponentModel.DataAnnotations;

namespace Askstatus.Infrastructure;
public class MailSettings
{
    public const string Section = "MailSettings";

    [Required]
    public bool Enabled { get; set; }
    [Required ()]
    public string? Host { get; set; }
    [Required]
    public int Port { get; set; }
    [Required(AllowEmptyStrings = true)]
    public string? Account { get; set; }
    [Required(AllowEmptyStrings = true)]
    public string? Password { get; set; }
    [Required(AllowEmptyStrings = true)]
    public string? ClientId { get; set; }
    [Required(AllowEmptyStrings = true)]
    public string? ClientSecret { get; set; }
    [Required]
    public bool EnableSsl { get; set; }
}
