namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

/// <summary>
/// Guaranteed not to be in the ServiceProvider, so can use it for tests where:
/// - want to mock throwing an exception in the handler
/// - tests where there are none in the ServiceProvider
/// </summary>
public class NoHandlerQuery : Query<string>
{
    public NoHandlerQuery() : base() { }
    public NoHandlerQuery(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
