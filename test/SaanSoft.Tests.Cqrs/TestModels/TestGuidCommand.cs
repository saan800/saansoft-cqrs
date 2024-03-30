using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestModels;

public class TestGuidCommand : BaseCommand
{
    public TestGuidCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public TestGuidCommand(Guid triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(triggeredById, correlationId, authenticatedId) { }

    public TestGuidCommand(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
