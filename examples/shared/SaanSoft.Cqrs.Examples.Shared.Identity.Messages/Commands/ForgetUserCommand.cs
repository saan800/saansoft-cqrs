namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;

/// <summary>
/// Many countries have "Right to be forgotten" laws around personal data
/// </summary>
public class ForgetUserCommand : Command
{
    public ForgetUserCommand(Guid? userKey = null, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        if (userKey.HasValue) UserKey = userKey.Value;
    }

    public ForgetUserCommand(Guid userKey, IMessage triggeredByMessage)
        : base(triggeredByMessage)
    {
        UserKey = userKey;
    }

    public required Guid UserKey { get; set; }
}
