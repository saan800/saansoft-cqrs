using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyQuery : Query<MyQuery, QueryResponse>
{
    public MyQuery(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public MyQuery(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }
}
