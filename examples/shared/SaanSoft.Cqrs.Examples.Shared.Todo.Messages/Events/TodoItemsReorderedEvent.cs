namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoItemsReorderedEvent : Event
{
    public TodoItemsReorderedEvent(Guid listKey, string? correlationId = null, string? authenticatedId = null) :
        base(listKey, correlationId, authenticatedId)
    {
    }

    public TodoItemsReorderedEvent(Guid listKey, IMessage triggeredByMessage) :
        base(listKey, triggeredByMessage)
    {
    }

    public Guid ListKey => Key;

    public List<Guid> ItemKeysInOrder { get; set; }
}
