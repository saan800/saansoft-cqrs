namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Commands;

public class UpdateTodoItemStatusCommand : Command
{
    public UpdateTodoItemStatusCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public UpdateTodoItemStatusCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid ListKey { get; set; }

    public Guid ItemKey { get; set; }

    public TodoStatus Status { get; set; }
}
