namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;

/// <summary>
/// Many countries have "Right to be forgotten" laws around personal data
/// </summary>
public class ForgetUserCommand : Command
{
    public ForgetUserCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public ForgetUserCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid Key { get; set; }
}
