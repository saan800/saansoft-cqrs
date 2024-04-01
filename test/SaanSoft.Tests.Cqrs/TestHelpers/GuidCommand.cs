using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class GuidCommand : BaseCommand
{
    public GuidCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public GuidCommand(Guid triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(triggeredById, correlationId, authenticatedId) { }

    public GuidCommand(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
