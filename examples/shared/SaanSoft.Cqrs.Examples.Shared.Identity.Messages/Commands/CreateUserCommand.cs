namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;

public class CreateUserCommand : Command
{
    protected CreateUserCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    protected CreateUserCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid UserKey { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
}
