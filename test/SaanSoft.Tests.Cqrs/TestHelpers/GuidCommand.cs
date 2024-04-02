using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class GuidCommand : BaseCommand
{
    public GuidCommand(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(triggeredById, correlationId, authenticatedId) { }

    public GuidCommand(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
