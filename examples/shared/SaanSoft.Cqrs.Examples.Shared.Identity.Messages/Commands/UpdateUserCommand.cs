namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;

public class UpdateUserCommand : Command
{
    public UpdateUserCommand(Guid? userKey = null, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        if (userKey.HasValue) UserKey = userKey.Value;
    }

    public UpdateUserCommand(Guid userKey, IMessage triggeredByMessage)
        : base(triggeredByMessage)
    {
        UserKey = userKey;
    }

    public required Guid UserKey { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? Biography { get; set; }
}
