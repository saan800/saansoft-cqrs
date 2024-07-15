namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Commands;

public class ReorderTodoItemsCommand : Command
{
    public ReorderTodoItemsCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public ReorderTodoItemsCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid ListKey { get; set; }

    public List<Guid> ItemKeysInOrder { get; set; }
}
