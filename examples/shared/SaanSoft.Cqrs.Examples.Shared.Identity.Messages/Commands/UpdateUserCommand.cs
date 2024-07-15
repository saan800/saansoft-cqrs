namespace SaanSoft.Cqrs.Examples.Shared.Identity.Messages.Commands;

public class UpdateUserCommand : Command
{
    public UpdateUserCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public UpdateUserCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid UserKey { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? Biography { get; set; }
}
