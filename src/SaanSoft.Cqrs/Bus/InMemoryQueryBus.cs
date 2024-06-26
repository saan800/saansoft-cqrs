using Microsoft.Extensions.DependencyInjection;

namespace SaanSoft.Cqrs.Bus;

public abstract class InMemoryQueryBus<TMessageId>(IServiceProvider serviceProvider, IIdGenerator<TMessageId> idGenerator, ILogger logger) :
    IQueryBus<TMessageId>,
    IQuerySubscriptionBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    protected readonly IIdGenerator<TMessageId> IdGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        var typedQuery = (TQuery)query;
        if (GenericUtils.IsNullOrDefault(typedQuery.Id)) typedQuery.Id = IdGenerator.NewId();

        // get subscription bus via ServiceProvider so it runs through any decorators
        var subscriptionBus = ServiceProvider.GetRequiredService<IQuerySubscriptionBus<TMessageId>>();
        return await subscriptionBus.RunAsync(typedQuery, cancellationToken);
    }

    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        var handler = GetHandler<TQuery, TResponse>();
        Logger.LogInformation("Running query handler '{HandlerType}' for '{MessageType}'", handler.GetType().FullName, query.TypeFullName);
        return await handler.HandleAsync((TQuery)query, cancellationToken);
    }

    public IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : IQuery<TQuery, TResponse>, IQuery<TMessageId>, IMessage<TMessageId>
    {
        var handlers = ServiceProvider.GetServices<IQueryHandler<TQuery, TResponse>>().ToList();
        switch (handlers.Count)
        {
            case 1:
                return handlers.Single();
            case 0:
                throw new InvalidOperationException($"No service for type '{typeof(IQueryHandler<TQuery, TResponse>)}' has been registered.");
            default:
                {
                    var typeNames = handlers.Select(handler => handler.GetType().FullName ?? handler.GetType().Name).ToList();
                    throw new InvalidOperationException($"Only one service for type '{typeof(IQueryHandler<TQuery, TResponse>)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }
}
