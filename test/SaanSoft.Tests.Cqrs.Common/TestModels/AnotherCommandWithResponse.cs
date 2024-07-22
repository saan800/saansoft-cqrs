namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherCommandWithResponse : Command<AnotherCommandWithResponse, string>
{
    public AnotherCommandWithResponse(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public AnotherCommandWithResponse(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public required string Message { get; set; }
}

