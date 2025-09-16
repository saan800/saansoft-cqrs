using System.Diagnostics.CodeAnalysis;

namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

public class MyEvent : Event
{
    public MyEvent() : base() { }

    [SetsRequiredMembers]
    public MyEvent(Guid key) : base(key) { }

    [SetsRequiredMembers]
    public MyEvent(Guid key, IMessage triggeredByMessage) : base(key, triggeredByMessage) { }
}
