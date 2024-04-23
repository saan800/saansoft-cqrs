using Microsoft.Extensions.DependencyInjection;

namespace SaanSoft.Cqrs.Store.MongoDB;

// ReSharper disable MemberCanBePrivate.Global
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Guid implementations of the command, event and query stores
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void AddAllStores(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCommandStore();
        serviceCollection.AddEventStore();
        serviceCollection.AddQueryStore();
    }

    /// <summary>
    /// Adds the provided type implementations of the command, event and query stores
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void AddAllStores<TMessageId, TEntityKey>(this IServiceCollection serviceCollection)
        where TMessageId : struct
        where TEntityKey : struct
    {
        serviceCollection.AddCommandStore<TMessageId>();
        serviceCollection.AddEventStore<TMessageId, TEntityKey>();
        serviceCollection.AddQueryStore<TMessageId>();
    }

    public static void AddCommandStore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandStore<Guid>, CommandStore>();
    }

    public static void AddCommandStore<TMessageId>(this IServiceCollection serviceCollection)
        where TMessageId : struct
    {
        serviceCollection.AddScoped<ICommandStore<TMessageId>, CommandStore<TMessageId>>();
    }

    public static void AddEventStore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEventStore<Guid, Guid>, EventStore>();
    }

    public static void AddEventStore<TMessageId, TEntityKey>(this IServiceCollection serviceCollection)
        where TMessageId : struct
        where TEntityKey : struct
    {
        serviceCollection.AddScoped<IEventStore<TMessageId, TEntityKey>, EventStore<TMessageId, TEntityKey>>();
    }

    public static void AddQueryStore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryStore<Guid>, QueryStore>();
    }

    public static void AddQueryStore<TMessageId>(this IServiceCollection serviceCollection)
        where TMessageId : struct
    {
        serviceCollection.AddScoped<IQueryStore<TMessageId>, QueryStore<TMessageId>>();
    }
}
