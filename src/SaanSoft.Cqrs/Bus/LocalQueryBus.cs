using Microsoft.Extensions.DependencyInjection;

using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class LocalQueryBus(IServiceProvider serviceProvider)
    : LocalQueryBus<Guid>(serviceProvider);

public abstract class LocalQueryBus<TMessageId>(IServiceProvider serviceProvider)
    : IQueryBus<TMessageId>
{
    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TMessageId, TQuery, TResult>
        where TResult : IQueryResult
    {
        return await serviceProvider
            .GetRequiredService<IQueryHandler<TQuery, TResult>>()
            .HandleAsync(query, cancellationToken);
    }
}
