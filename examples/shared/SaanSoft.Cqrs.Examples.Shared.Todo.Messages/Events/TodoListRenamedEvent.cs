using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Events;

public class TodoListRenamedEvent : Event
{
    public TodoListRenamedEvent(Guid key, string? correlationId = null, string? authenticatedId = null) :
        base(key, correlationId, authenticatedId)
    {
    }

    public TodoListRenamedEvent(Guid key, IMessage<Guid> triggeredByMessage) : base(key, triggeredByMessage)
    {
    }

    public string Title { get; set; }
}
