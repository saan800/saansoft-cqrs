namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyCommand : Command
{
    public MyCommand(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    public MyCommand(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
