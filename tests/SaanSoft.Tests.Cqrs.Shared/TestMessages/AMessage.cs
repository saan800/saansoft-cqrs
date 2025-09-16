namespace SaanSoft.Tests.Cqrs.Shared.TestMessages;

/// <summary>
/// Generic message that inherits directly from MessageBase
/// Use to test non-command/event/query specific logic
/// </summary>
public class AMessage : MessageBase
{
    public AMessage() : base() { }
    public AMessage(IMessage triggeredByMessage) : base(triggeredByMessage) { }
}
