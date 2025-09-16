namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

/// <summary>
/// If Message=null - handler will return null response
/// </summary>
public class MyQuery : Query<MyQueryResult?>
{
    public MyQuery() : base() { }
    public MyQuery(IMessage triggeredByMessage) : base(triggeredByMessage) { }
    public string? Message { get; set; }
}
