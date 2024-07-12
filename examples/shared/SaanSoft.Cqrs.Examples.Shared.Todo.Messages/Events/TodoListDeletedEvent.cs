using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoListDeletedEvent : Event
{
    public TodoListDeletedEvent(Guid key, string? correlationId = null, string? authenticatedId = null) :
        base(key, correlationId, authenticatedId)
    {
    }

    public TodoListDeletedEvent(Guid key, IMessage<Guid> triggeredByMessage) : base(key, triggeredByMessage)
    {
    }
}
