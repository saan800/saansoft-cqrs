namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;

/// <summary>
/// Many countries have "Right to be forgotten" laws around personal data
/// </summary>
public class ForgetUserCommand : Command
{
    public ForgetUserCommand(Guid userKey, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        UserKey = userKey;
    }

    public ForgetUserCommand(Guid userKey, IMessage triggeredByMessage)
        : base(triggeredByMessage)
    {
        UserKey = userKey;
    }

    public Guid UserKey { get; set; }
}
