using SaanSoft.Cqrs.Examples.Shared.Todo.Entities;

namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Queries;

public class GetTodoListQuery : Query<GetTodoListQuery, TodoList>
{
    public GetTodoListQuery(Guid listKey, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        ListKey = listKey;
    }

    public GetTodoListQuery(Guid listKey, IMessage triggeredByMessage) : base(triggeredByMessage)
    {
        ListKey = listKey;
    }

    public Guid ListKey { get; set; }
}
