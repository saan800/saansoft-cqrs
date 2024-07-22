namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyEvent : Event
{
    public MyEvent(Guid key, string? correlationId = null, string? authenticatedId = null)
        : base(key, correlationId, authenticatedId) { }

    public MyEvent(Guid key, IMessage triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
