using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestModels;

public class TestGuidQuery : BaseQuery<TestGuidQuery, TestQueryResult>
{
    public TestGuidQuery(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public TestGuidQuery(Guid triggeredById, string? correlationId = null, string? authenticatedId = null)
        : base(triggeredById, correlationId, authenticatedId) { }

    public TestGuidQuery(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
