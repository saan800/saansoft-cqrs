namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Commands;

public class DeleteTodoListCommand : Command
{
    public DeleteTodoListCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public DeleteTodoListCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid ListKey { get; set; }
}
