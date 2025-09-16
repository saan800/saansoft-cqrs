using System.Diagnostics.CodeAnalysis;

namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

/// <summary>
/// Guaranteed not to be in the ServiceProvider, so can use it for tests where:
/// - want to mock throwing an exception in the handler
/// - tests where there are none in the ServiceProvider
/// </summary>
public class NoHandlerEvent : Event
{
    public NoHandlerEvent() : base() { }

    [SetsRequiredMembers]
    public NoHandlerEvent(Guid key) : base(key) { }

    [SetsRequiredMembers]
    public NoHandlerEvent(Guid key, IMessage triggeredByMessage) : base(key, triggeredByMessage) { }
}
