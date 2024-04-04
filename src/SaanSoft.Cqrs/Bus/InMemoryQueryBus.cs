using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class InMemoryQueryBus(IServiceProvider serviceProvider, ILogger logger, QueryBusOptions? options = null)
    : InMemoryQueryBus<Guid>(serviceProvider, logger, options);

public abstract class InMemoryQueryBus<TMessageId> :
    IQueryBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly QueryBusOptions Options;
    protected readonly LogLevel LogLevel;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ILogger Logger;
    // ReSharper restore MemberCanBePrivate.Global

    protected InMemoryQueryBus(IServiceProvider serviceProvider, ILogger logger, QueryBusOptions? options = null)
    {
        Options = options ?? new QueryBusOptions();
        LogLevel = Options.LogLevel;

        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> QueryAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>
        where TResponse : IQueryResponse
    {
        var handlers = ServiceProvider.GetServices<IQueryHandler<TQuery, TResponse>>().ToList();
        switch (handlers.Count)
        {
            case 1:
                var handler = handlers.Single();
                if (Logger.IsEnabled(LogLevel))
                {
                    Logger.Log(LogLevel, "Running query handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, typeof(TQuery).FullName);
                }
                return await handler.HandleAsync(query, cancellationToken);
            case 0:
                throw new InvalidOperationException($"No service for type '{typeof(IQueryHandler<TQuery, TResponse>)}' has been registered.");
            default:
                {
                    var typeNames = handlers.Select(x => x.GetType().FullName).ToList();
                    throw new InvalidOperationException($"Only one service for type '{typeof(IQueryHandler<TQuery, TResponse>)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }
}
