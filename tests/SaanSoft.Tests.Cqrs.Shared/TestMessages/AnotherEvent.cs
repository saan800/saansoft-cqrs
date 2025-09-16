using System.Diagnostics.CodeAnalysis;

namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

public class AnotherEvent : Event
{
    public AnotherEvent() : base() { }

    [SetsRequiredMembers]
    public AnotherEvent(Guid key) : base(key) { }

    [SetsRequiredMembers]
    public AnotherEvent(Guid key, IMessage triggeredByMessage) : base(key, triggeredByMessage) { }
}
