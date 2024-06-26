namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherCommand : Command
{
    public AnotherCommand(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    public AnotherCommand(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
