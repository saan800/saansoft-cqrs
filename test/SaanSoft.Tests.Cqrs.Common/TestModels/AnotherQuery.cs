using IMessage = SaanSoft.Cqrs.GuidIds.Messages.IMessage;

namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherQuery : Query<AnotherQuery, MyQueryResponse>
{
    public AnotherQuery(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public AnotherQuery(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public string? Message { get; set; }
}
