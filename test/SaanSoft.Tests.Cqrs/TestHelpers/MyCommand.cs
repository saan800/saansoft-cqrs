using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class MyCommand : BaseCommand
{
    public MyCommand(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(triggeredById, correlationId, authenticatedId) { }

    public MyCommand(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
