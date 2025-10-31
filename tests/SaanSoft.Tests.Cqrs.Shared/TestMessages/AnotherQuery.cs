namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

public class AnotherQuery : Query<MyQueryResponse>
{
    public AnotherQuery() : base() { }
    public AnotherQuery(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
