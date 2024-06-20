using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Cqrs.Messages;

namespace SaanSoft.Cqrs.Bus;

public class InMemoryQueryBus(IServiceProvider serviceProvider, ILogger logger)
    : InMemoryQueryBus<Guid>(serviceProvider, logger);

public abstract class InMemoryQueryBus<TMessageId>(IServiceProvider serviceProvider, ILogger logger) :
    IQueryPublisher<TMessageId>,
    IQuerySubscriber<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task<TResponse> QueryAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>, IMessage<TMessageId>
        where TResponse : IQueryResponse
    {
        // get subscriber via ServiceProvider so it runs through any decorators
        var subscriber = ServiceProvider.GetRequiredService<IQuerySubscriber<TMessageId>>();
        return await subscriber.RunAsync(query, cancellationToken);
    }

    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>, IMessage<TMessageId>
        where TResponse : IQueryResponse
    {
        var handlers = ServiceProvider.GetServices<IQueryHandler<TQuery, TResponse>>().ToList();
        switch (handlers.Count)
        {
            case 1:
                var handler = handlers.Single();
                Logger.LogInformation("Running query handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, typeof(TQuery).FullName);
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
