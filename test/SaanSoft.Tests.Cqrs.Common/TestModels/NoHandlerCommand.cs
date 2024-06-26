namespace SaanSoft.Tests.Cqrs.Common.TestModels;

/// <summary>
/// Guaranteed not to be in the ServiceProvider, so can use it for tests where:
/// - want to mock throwing an exception in the handler
/// - tests where there are none in the ServiceProvider
/// </summary>
public class NoHandlerCommand : Command
{
    public NoHandlerCommand(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    public NoHandlerCommand(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
