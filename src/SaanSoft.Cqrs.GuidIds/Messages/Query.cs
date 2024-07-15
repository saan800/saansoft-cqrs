using SaanSoft.Cqrs.Common.Messages;
using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public abstract class Query<TQuery, TResponse> :
    BaseQuery<Guid, TQuery, TResponse>,
    IMessage
    where TQuery : IBaseQuery<Guid, TQuery, TResponse>
{
    protected Query(string? correlationId = null, string? authenticatedId = null)
        : base(correlationId, authenticatedId)
    {
    }

    protected Query(IMessage triggeredByMessage) : base(triggeredByMessage)
    {
    }
}
