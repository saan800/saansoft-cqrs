using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Events;

public class UserCreatedEvent : Event
{
    public UserCreatedEvent(Guid key, string? correlationId = null, string? authenticatedId = null) :
        base(key, correlationId, authenticatedId)
    {
    }

    public UserCreatedEvent(Guid key, IMessage<Guid> triggeredByMessage) : base(key, triggeredByMessage)
    {
    }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
}
