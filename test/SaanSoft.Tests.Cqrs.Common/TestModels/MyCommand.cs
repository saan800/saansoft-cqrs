using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyCommand : Command
{
    public MyCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public MyCommand(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
