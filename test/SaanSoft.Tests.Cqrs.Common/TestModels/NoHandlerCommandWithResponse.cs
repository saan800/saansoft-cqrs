namespace SaanSoft.Tests.Cqrs.Common.TestModels;

/// <summary>
/// Guaranteed not to be in the ServiceProvider, so can use it for tests where:
/// - want to mock throwing an exception in the handler
/// - tests where there are none in the ServiceProvider
/// </summary>
public class NoHandlerCommandWithResponse : Command<NoHandlerCommandWithResponse, string>
{
    public NoHandlerCommandWithResponse(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public NoHandlerCommandWithResponse(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public required string Message { get; set; }
}
