namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Events;

public class UserForgottenEvent : Event
{
    public UserForgottenEvent(Guid userKey, string? correlationId = null, string? authenticatedId = null)
        : base(userKey, correlationId, authenticatedId)
    {
    }

    public UserForgottenEvent(Guid userKey, IMessage message) : base(userKey, message)
    {
    }
}
