namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Queries;

public class GetAllTodoListsQuery : Query<GetAllTodoListsQuery, List<TodoListSummary>>
{
    public GetAllTodoListsQuery(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    public GetAllTodoListsQuery(IMessage triggeredByMessage) : base(triggeredByMessage)
    {
    }
}
