namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherCommandWithResponse : Command<AnotherCommandWithResponse, string>
{
    public AnotherCommandWithResponse(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    public AnotherCommandWithResponse(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }

    public required string Message { get; set; }
}

