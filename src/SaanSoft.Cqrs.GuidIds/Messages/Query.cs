using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public abstract class Query<TQuery, TResponse> :
    Query<Guid, TQuery, TResponse>,
    IMessage
    where TQuery : IQuery<Guid, TQuery, TResponse>
{
    protected Query(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected Query(IMessage triggeredByMessage) : base(triggeredByMessage)
    {
    }
}
