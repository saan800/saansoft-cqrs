using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class MyEvent : BaseEvent
{
    public MyEvent(Guid key, Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(key, triggeredById, correlationId, authenticatedId) { }

    public MyEvent(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
