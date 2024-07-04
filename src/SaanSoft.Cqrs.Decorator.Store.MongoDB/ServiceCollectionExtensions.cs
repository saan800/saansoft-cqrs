using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.GuidIds.Decorator.Store;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

// ReSharper disable MemberCanBePrivate.Global
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Guid implementations of the command, event and query stores
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void AddGuidStores(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCommandStore();
        serviceCollection.AddEventStore();
        serviceCollection.AddQueryStore();
    }

    /// <summary>
    /// Adds the provided type implementations of the command, event and query stores
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void AddGuidStores<TEntityKey>(this IServiceCollection serviceCollection)
        where TEntityKey : struct
    {
        serviceCollection.AddCommandStore();
        serviceCollection.AddQueryStore();

        if (typeof(TEntityKey) == typeof(Guid))
        {
            serviceCollection.AddEventStore();
        }
        else
        {
            serviceCollection.AddEventStore<TEntityKey>();
        }
    }

    public static void AddCommandStore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandRepository<Guid>, CommandRepository>();
    }

    public static void AddEventStore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEventRepository<Guid, Guid>, EventRepository>();
    }

    public static void AddEventStore<TEntityKey>(this IServiceCollection serviceCollection)
        where TEntityKey : struct
    {
        serviceCollection.AddScoped<IEventRepository<Guid, TEntityKey>, EventRepository<TEntityKey>>();
    }

    public static void AddQueryStore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryRepository<Guid>, QueryRepository>();
    }
}
