namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

/// <summary>
/// If Message=null - handler will return null response
/// </summary>
public class MyCommandWithResponse : Command<string?>
{
    public MyCommandWithResponse() : base() { }
    public MyCommandWithResponse(IMessage triggeredByMessage) : base(triggeredByMessage) { }

    public string? Message { get; set; }
}
