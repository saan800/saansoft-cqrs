namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Commands;

public class DeleteTodoItemCommand : Command
{
    public DeleteTodoItemCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public DeleteTodoItemCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid ListKey { get; set; }

    public Guid ItemKey { get; set; }
}
