namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoListDeletedEvent : Event
{
    public TodoListDeletedEvent(Guid listKey, string? correlationId = null, string? authenticatedId = null) :
        base(listKey, correlationId, authenticatedId)
    {
    }

    public TodoListDeletedEvent(Guid listKey, IMessage triggeredByMessage) : base(listKey, triggeredByMessage)
    {
    }
}
