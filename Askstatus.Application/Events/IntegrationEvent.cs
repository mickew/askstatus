namespace Askstatus.Application.Events;

public abstract record IntegrationEvent(Guid Id) : IIntegrationEvent;
