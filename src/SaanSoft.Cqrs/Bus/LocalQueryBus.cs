using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class LocalQueryBus(IServiceProvider serviceProvider, ILogger logger, QueryBusOptions? options = null)
    : LocalQueryBus<Guid>(serviceProvider, logger, options);

public abstract class LocalQueryBus<TMessageId>
    : IQueryBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly QueryBusOptions Options;
    protected readonly LogLevel LogLevel;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger Logger;
    // ReSharper restore MemberCanBePrivate.Global

    protected LocalQueryBus(IServiceProvider serviceProvider, ILogger logger, QueryBusOptions? options = null)
    {
        Options = options ?? new QueryBusOptions();
        LogLevel = Options.LogLevel;

        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TMessageId, TQuery, TResult>
        where TResult : IQueryResult
    {
        var handlers = ServiceProvider.GetServices<IQueryHandler<TQuery, TResult>>()?.ToList() ?? [];
        switch (handlers.Count)
        {
            case 1:
                var handler = handlers.Single();
                Logger.Log(LogLevel, "Running query handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, typeof(TQuery).FullName);
                return await handlers.First().HandleAsync(query, cancellationToken);
            case 0:
                throw new InvalidOperationException($"No service for type '{typeof(IQueryHandler<TQuery, TResult>)}' has been registered.");
            default:
                {
                    var typeNames = handlers.Select(x => x.GetType().FullName).ToList();
                    throw new InvalidOperationException($"Only one service for type '{typeof(IQueryHandler<TQuery, TResult>)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }
}
