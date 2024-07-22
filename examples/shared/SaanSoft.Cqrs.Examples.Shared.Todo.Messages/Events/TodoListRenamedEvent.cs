namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoListRenamedEvent : Event
{
    public TodoListRenamedEvent(Guid listKey, string? correlationId = null, string? authenticatedId = null) :
        base(listKey, correlationId, authenticatedId)
    {
    }

    public TodoListRenamedEvent(Guid listKey, IMessage triggeredByMessage) : base(listKey, triggeredByMessage)
    {
    }

    public string Title { get; set; }
}
