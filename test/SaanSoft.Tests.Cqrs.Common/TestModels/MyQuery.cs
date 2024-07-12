namespace SaanSoft.Tests.Cqrs.Common.TestModels;

public class MyQuery : Query<MyQuery, MyQueryResponse>
{
    public MyQuery(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public MyQuery(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public string? Message { get; set; }
}
