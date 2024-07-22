namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Commands;

public class AddTodoItemCommand : Command
{
    public AddTodoItemCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public AddTodoItemCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid ListKey { get; set; }

    public Guid ItemKey { get; set; }

    public string Title { get; set; }
}
