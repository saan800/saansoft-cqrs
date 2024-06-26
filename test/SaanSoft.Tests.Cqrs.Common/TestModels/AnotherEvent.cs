namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherEvent : Event
{
    public AnotherEvent(Guid key, Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(key, id, correlationId, authenticatedId) { }

    public AnotherEvent(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
