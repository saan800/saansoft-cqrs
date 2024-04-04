using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.TestHelpers;

public class MyQuery : Query<MyQuery, QueryResponse>
{
    public MyQuery(Guid? triggeredById = null, string? correlationId = null, string? authenticatedId = null)
        : base(triggeredById, correlationId, authenticatedId) { }

    public MyQuery(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
