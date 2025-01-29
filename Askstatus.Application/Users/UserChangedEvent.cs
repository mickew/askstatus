using Askstatus.Common.Models;
using MediatR;

namespace Askstatus.Application.Users;
public sealed record UserChangedEvent(UserVMWithLink User, UserEventType EventType) : INotification
{
}
