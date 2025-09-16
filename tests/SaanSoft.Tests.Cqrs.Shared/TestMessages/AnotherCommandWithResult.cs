namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

public class AnotherCommandWithResult : Command<string>
{
    public AnotherCommandWithResult() : base() { }
    public AnotherCommandWithResult(IMessage triggeredByMessage) : base(triggeredByMessage) { }

    public required string Message { get; set; }
}

