namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Commands;

public class RenameTodoListCommand : Command
{
    public RenameTodoListCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public RenameTodoListCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid ListKey { get; set; }

    public string Title { get; set; }
}
