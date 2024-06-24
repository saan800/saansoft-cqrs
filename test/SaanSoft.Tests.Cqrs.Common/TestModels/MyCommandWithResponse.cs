using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyCommandWithResponse : Command<MyCommandWithResponse, string>
{
    public MyCommandWithResponse(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public MyCommandWithResponse(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}