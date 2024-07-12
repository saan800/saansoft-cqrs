using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Events;

public class UserForgottenEvent : Event
{
    public UserForgottenEvent(Guid key, string? correlationId = null, string? authenticatedId = null) :
        base(key, correlationId, authenticatedId)
    {
    }

    public UserForgottenEvent(Guid key, IMessage<Guid> triggeredByMessage) : base(key, triggeredByMessage)
    {
    }
}
