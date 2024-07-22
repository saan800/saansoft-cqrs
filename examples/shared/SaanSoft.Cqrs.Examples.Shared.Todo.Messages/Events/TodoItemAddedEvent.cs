namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoItemAddedEvent : Event
{
    public TodoItemAddedEvent(Guid listKey, string? correlationId = null, string? authenticatedId = null) :
        base(listKey, correlationId, authenticatedId)
    {
    }

    public TodoItemAddedEvent(Guid listKey, IMessage triggeredByMessage) :
        base(listKey, triggeredByMessage)
    {
    }

    public Guid ListKey => Key;

    public Guid ItemKey { get; set; }

    public string Title { get; set; }
}
