namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

public class MyCommand : Command
{
    public MyCommand() : base() { }
    public MyCommand(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
