using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus;

public class InMemoryQueryBus(IServiceProvider serviceProvider) :
    IQueryBus,
    IQuerySubscriptionBus
{
    // ReSharper disable MemberCanBePrivate.Global
    protected readonly IServiceProvider ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    // ReSharper restore MemberCanBePrivate.Global

    public async Task<TResponse> FetchAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query,
        CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        var typedQuery = (TQuery)query;
        if (GenericUtils.IsNullOrDefault(typedQuery.Id)) typedQuery.Id = Guid.NewGuid();

        var subscriptionBus = GetSubscriptionBus();
        return await subscriptionBus.RunAsync(typedQuery, cancellationToken);
    }

    /// <summary>
    /// Get subscription bus via ServiceProvider so it runs through any decorators
    /// </summary>
    /// <returns></returns>
    protected virtual IQuerySubscriptionBus GetSubscriptionBus()
        => ServiceProvider.GetRequiredService<IQuerySubscriptionBus>();

    public async Task<TResponse> RunAsync<TQuery, TResponse>(IQuery<TQuery, TResponse> query, CancellationToken cancellationToken = default)
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        var handler = GetHandler<TQuery, TResponse>();
        var typedQuery = (TQuery)query;
        return await handler.HandleAsync(typedQuery, cancellationToken);
    }

    public IQueryHandler<TQuery, TResponse> GetHandler<TQuery, TResponse>()
        where TQuery : class, IQuery<TQuery, TResponse>
    {
        var handlers = ServiceProvider.GetServices<IQueryHandler<TQuery, TResponse>>().ToList();
        switch (handlers.Count)
        {
            case 1:
                return handlers.Single();
            case 0:
                throw new InvalidOperationException($"No handler for type '{typeof(IQueryHandler<TQuery, TResponse>)}' has been registered.");
            default:
                {
                    var typeNames = handlers.Select(handler => handler.GetType().GetTypeFullName()).ToList();
                    throw new InvalidOperationException($"Only one handler for type '{typeof(IQueryHandler<TQuery, TResponse>)}' can be registered. Currently have {typeNames.Count} registered: {string.Join("; ", typeNames)}");
                }
        }
    }
}
