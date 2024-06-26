namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyEvent : Event
{
    public MyEvent(Guid key, Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(key, id, correlationId, authenticatedId) { }

    public MyEvent(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
