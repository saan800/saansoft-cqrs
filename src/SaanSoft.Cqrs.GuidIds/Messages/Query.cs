using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public abstract class Query<TQuery, TResponse> : Query<Guid, TQuery, TResponse>
    where TQuery : IQuery<Guid, TQuery, TResponse>
{
    protected Query(Guid? id = null, string? correlationId = null, string? authenticatedId = null)
        : base(id, correlationId, authenticatedId)
    {
    }

    protected Query(IMessage<Guid> triggeredByMessage) : base(triggeredByMessage)
    {
    }
}
