using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherEvent : Event
{
    public AnotherEvent(Guid key, string? correlationId = null, string? authenticatedId = null)
        : base(key, correlationId, authenticatedId) { }

    public AnotherEvent(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
