namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class AnotherQuery : Query<AnotherQuery, MyQueryResponse>
{
    public AnotherQuery(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    public AnotherQuery(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }

    public string? SomeData { get; set; }
}
