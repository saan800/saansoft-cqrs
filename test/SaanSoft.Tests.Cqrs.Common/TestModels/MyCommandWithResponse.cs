namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyCommandWithResponse : Command<MyCommandWithResponse, string>
{
    public MyCommandWithResponse(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    public MyCommandWithResponse(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }

    public required string Message { get; set; }
}
