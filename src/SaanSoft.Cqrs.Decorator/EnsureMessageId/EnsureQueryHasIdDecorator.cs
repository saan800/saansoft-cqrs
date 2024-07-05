namespace SaanSoft.Cqrs.Decorator.EnsureMessageId;

/// <summary>
/// Ensure that the Query has the Id field populated with a non-null and non-default value
/// </summary>
/// <param name="idGenerator"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class EnsureQueryHasIdDecorator<TMessageId>(IIdGenerator<TMessageId> idGenerator, IQueryBus<TMessageId> next)
    : IQueryBusDecorator<TMessageId>
    where TMessageId : struct
{
    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        var typedQuery = (TQuery)query;
        if (GenericUtils.IsNullOrDefault(typedQuery.Id)) typedQuery.Id = idGenerator.NewId();
        return await next.FetchAsync(typedQuery, cancellationToken);
    }
}
