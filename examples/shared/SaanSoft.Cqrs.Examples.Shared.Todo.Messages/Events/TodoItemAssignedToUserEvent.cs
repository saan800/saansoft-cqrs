namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoItemAssignedToUserEvent : Event
{
    public TodoItemAssignedToUserEvent(Guid listKey, string? correlationId = null, string? authenticatedId = null) :
        base(listKey, correlationId, authenticatedId)
    {
    }

    public TodoItemAssignedToUserEvent(Guid listKey, IMessage triggeredByMessage) :
        base(listKey, triggeredByMessage)
    {
    }

    public Guid ListKey => Key;

    public Guid ItemKey { get; set; }

    public Guid UserKey { get; set; }
}
