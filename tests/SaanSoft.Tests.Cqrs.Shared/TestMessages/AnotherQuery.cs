namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

public class AnotherQuery : Query<MyQueryResult>
{
    public AnotherQuery() : base() { }
    public AnotherQuery(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
