namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyQuery : Query<MyQuery, MyQueryResponse>
{
    public MyQuery(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId) { }

    public MyQuery(IMessage<Guid> triggeredByMessage)
        : base(triggeredByMessage) { }

    public string? SomeData { get; set; }
}
