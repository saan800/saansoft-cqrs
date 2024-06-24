using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherCommandWithResponse : Command<MyCommandWithResponse, string>
{
    public AnotherCommandWithResponse(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public AnotherCommandWithResponse(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}

