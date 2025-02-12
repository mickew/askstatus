using Askstatus.Common.Models;

namespace Askstatus.Domain.Entities;
public class SystemLog
{
    public int Id { get; set; }
    public DateTime EventTime { get; set; }
    public SystemLogEventType EventType { get; set; }
    public string? User { get; set; }
    public string? Message { get; set; }

}
