using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherQuery : Query<AnotherQuery, MyQueryResponse>
{
    public AnotherQuery(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public AnotherQuery(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
