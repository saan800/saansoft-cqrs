namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

public class AnotherCommandWithResponse : Command<string>
{
    public AnotherCommandWithResponse() : base() { }
    public AnotherCommandWithResponse(IMessage triggeredByMessage) : base(triggeredByMessage) { }

    public required string Message { get; set; }
}

