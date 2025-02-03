using MediatR;

namespace Askstatus.Application.Events;

public interface IIntegrationEvent : INotification
{
    Guid Id { get; init; }
}
