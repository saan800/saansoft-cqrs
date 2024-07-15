namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoItemStatusUpdatedEvent : Event
{
    public TodoItemStatusUpdatedEvent(Guid listKey, string? correlationId = null, string? authenticatedId = null) :
        base(listKey, correlationId, authenticatedId)
    {
    }

    public TodoItemStatusUpdatedEvent(Guid listKey, IMessage triggeredByMessage) :
        base(listKey, triggeredByMessage)
    {
    }

    public Guid ListKey => Key;

    public Guid ItemKey { get; set; }

    public TodoStatus Status { get; set; }
}
