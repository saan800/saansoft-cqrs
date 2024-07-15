using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Core.Utilities;

namespace SaanSoft.Cqrs.Core.Bus;

public abstract class BaseInMemoryQueryBus<TMessageId>(IServiceProvider serviceProvider, IIdGenerator<TMessageId> idGenerator) :
    IBaseQueryBus<TMessageId>,
    IBaseQuerySubscriptionBus<TMessageId>
    where TMessageId : struct
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    protected readonly IIdGenerator<TMessageId> IdGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IBaseQuery<TQuery, TResponse> query,
        CancellationToken cancellationToken = default)
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
    {
        var typedQuery = (TQuery)query;
        if (GenericUtils.IsNullOrDefault(typedQuery.Id)) typedQuery.Id = IdGenerator.NewId();

        var subscriptionBus = GetSubscriptionBus();
        return await subscriptionBus.RunAsync(typedQuery, cancellationToken);
    }

    /// <summary>
    /// Get subscription bus via ServiceProvider so it runs through any decorators
    /// </summary>
    /// <returns></returns>
    protected virtual IBaseQuerySubscriptionBus<TMessageId> GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IBaseQuerySubscriptionBus<TMessageId>>();

    public async Task<TResponse> RunAsync<TQuery, TResponse>(IBaseQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
    {
        var handler = GetHandler<TQuery, TResponse>();
        var typedQuery = (TQuery)query;
        return await handler.HandleAsync(typedQuery, cancellationToken);
    }

    public IBaseQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : class, IBaseQuery<TQuery, TResponse>, IBaseQuery<TMessageId>, IBaseMessage<TMessageId>
    {
        var handlers = ServiceProvider.GetServices<IBaseQueryHandler<TQuery, TResponse>>().ToList();
        switch (handlers.Count)
        {
            case 1:
                return handlers.Single();
            case 0:
                throw new InvalidOperationException($"No handler for type '{typeof(IBaseQueryHandler<TQuery, TResponse>)}' has been registered.");
            default:
                {
                    var typeNames = handlers.Select(handler => handler.GetType().GetTypeFullName()).ToList();
                    throw new InvalidOperationException($"Only one handler for type '{typeof(IBaseQueryHandler<TQuery, TResponse>)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }
}
