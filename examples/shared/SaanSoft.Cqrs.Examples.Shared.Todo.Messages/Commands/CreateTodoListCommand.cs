namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Commands;

public class CreateTodoListCommand : Command
{
    public CreateTodoListCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public CreateTodoListCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid ListKey { get; set; }

    public string Title { get; set; }
}
