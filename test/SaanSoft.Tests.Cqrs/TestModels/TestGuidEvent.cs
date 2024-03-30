using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestModels;

public class TestGuidEvent : BaseEvent
{
    public TestGuidEvent(Guid key, string? correlationId = null, string? authenticatedId = null)
        : base(key, correlationId, authenticatedId) { }

    public TestGuidEvent(Guid key, Guid triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(key, triggeredById, correlationId, authenticatedId) { }

    public TestGuidEvent(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
