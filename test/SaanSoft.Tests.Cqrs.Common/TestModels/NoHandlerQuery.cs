namespace SaanSoft.Tests.Cqrs.Common.TestModels;

/// <summary>
/// Guaranteed not to be in the ServiceProvider, so can use it for tests where:
/// - want to mock throwing an exception in the handler
/// - tests where there are none in the ServiceProvider
/// </summary>
public class NoHandlerQuery : Query<NoHandlerQuery, string>
{
    public NoHandlerQuery(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public NoHandlerQuery(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public string? Message { get; set; }
}
