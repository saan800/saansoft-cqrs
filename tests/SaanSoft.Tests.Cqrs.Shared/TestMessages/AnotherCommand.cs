namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

public class AnotherCommand : Command
{
    public AnotherCommand() : base() { }
    public AnotherCommand(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
