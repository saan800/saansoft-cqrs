using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class GuidQuery : BaseQuery<GuidQuery, QueryResult>
{
    public GuidQuery(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(triggeredById, correlationId, authenticatedId) { }

    public GuidQuery(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
