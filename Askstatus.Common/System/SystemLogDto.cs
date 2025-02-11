using Askstatus.Common.Models;

namespace Askstatus.Common.System;
public record SystemLogDto(string Id, DateTime CreatedAt, SystemLogEventType EventType, string User, string Message);
