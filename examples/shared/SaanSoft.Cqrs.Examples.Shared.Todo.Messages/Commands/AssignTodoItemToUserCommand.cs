namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Commands;

public class AssignTodoItemToUserCommand : Command
{
    public AssignTodoItemToUserCommand(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId) { }

    public AssignTodoItemToUserCommand(IMessage triggeredByMessage)
        : base(triggeredByMessage) { }

    public Guid ListKey { get; set; }

    public Guid ItemKey { get; set; }

    public Guid UserKey { get; set; }

}
