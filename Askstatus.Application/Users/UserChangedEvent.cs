using Askstatus.Application.Events;

namespace Askstatus.Application.Users;
public sealed record UserChangedIntegrationEvent(Guid Id, UserVMWithLink User, UserEventType EventType) : IntegrationEvent(Id);
