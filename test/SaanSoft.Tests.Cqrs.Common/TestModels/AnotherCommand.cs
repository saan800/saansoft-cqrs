using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherCommand : Command
{
    public AnotherCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public AnotherCommand(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
