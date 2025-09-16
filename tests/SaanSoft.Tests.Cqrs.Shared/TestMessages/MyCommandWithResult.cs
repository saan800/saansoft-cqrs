namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

/// <summary>
/// If Message=null - handler will return null response
/// </summary>
public class MyCommandWithResult : Command<string?>
{
    public MyCommandWithResult() : base() { }
    public MyCommandWithResult(IMessage triggeredByMessage) : base(triggeredByMessage) { }

    public string? Message { get; set; }
}
