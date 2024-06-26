namespace SaanSoft.Tests.Cqrs.Common.TestModels;

/// <summary>
/// Guaranteed not to be in the ServiceProvider, so can use it for tests where:
/// - want to mock throwing an exception in the handler
/// - tests where there are none in the ServiceProvider
/// </summary>
public class NoHandlerEvent : Event
{
    public NoHandlerEvent(Guid key, Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(key, id, correlationId, authenticatedId) { }

    public NoHandlerEvent(Guid key, IMessage<Guid> triggeredByMessage)
        : base(key, triggeredByMessage) { }
}
