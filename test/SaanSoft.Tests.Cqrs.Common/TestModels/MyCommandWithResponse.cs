namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyCommandWithResponse : Command<MyCommandWithResponse, string>
{
    public MyCommandWithResponse(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public MyCommandWithResponse(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public required string Message { get; set; }
}
