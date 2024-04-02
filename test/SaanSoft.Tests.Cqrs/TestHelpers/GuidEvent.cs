using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class GuidEvent : BaseEvent
{
    public GuidEvent(Guid key, Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(key, triggeredById, correlationId, authenticatedId) { }

    public GuidEvent(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
