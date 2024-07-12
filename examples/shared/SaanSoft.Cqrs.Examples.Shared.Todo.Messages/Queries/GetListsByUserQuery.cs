using SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Models;

namespace SaanSoft.Cqrs.Examples.Shared.Todo.Messages.Queries;

public class GetListsByUserQuery : Query<GetListsByUserQuery, List<TodoListSummary>>
{
    public GetListsByUserQuery(Guid userKey, string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
        UserKey = userKey;
    }

    public GetListsByUserQuery(Guid userKey, IMessage triggeredByMessage) : base(triggeredByMessage)
    {
        UserKey = userKey;
    }

    public Guid UserKey { get; set; }
}
