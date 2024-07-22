namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoItemDeletedEvent : Event
{
    public TodoItemDeletedEvent(Guid listKey, string? correlationId = null, string? authenticatedId = null) :
        base(listKey, correlationId, authenticatedId)
    {
    }

    public TodoItemDeletedEvent(Guid listKey, IMessage triggeredByMessage) :
        base(listKey, triggeredByMessage)
    {
    }

    public Guid ListKey => Key;

    public Guid ItemKey { get; set; }
}
