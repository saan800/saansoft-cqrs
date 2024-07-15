namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoListCreatedEvent : Event
{
    public TodoListCreatedEvent(Guid listKey, string? correlationId = null, string? authenticatedId = null) :
        base(listKey, correlationId, authenticatedId)
    {
    }

    public TodoListCreatedEvent(Guid listKey, IMessage triggeredByMessage) : base(listKey, triggeredByMessage)
    {
    }

    public string Title { get; set; }
}
