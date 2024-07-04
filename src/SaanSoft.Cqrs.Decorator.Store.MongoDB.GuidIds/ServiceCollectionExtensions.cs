using Microsoft.Extensions.DependencyInjection;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB.GuidIds;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Guid implementations of the command, event and query repositories
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void AddGuidRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCommandRepository();
        serviceCollection.AddEventRepository();
        serviceCollection.AddQueryRepository();
    }

    public static void AddCommandRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandRepository, CommandRepository>();
        serviceCollection.AddScoped<ICommandRepository<Guid>, CommandRepository>();

        serviceCollection.AddScoped<ICommandHandlerRepository, CommandRepository>();
        serviceCollection.AddScoped<ICommandHandlerRepository<Guid>, CommandRepository>();
    }

    public static void AddEventRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEventRepository, EventRepository>();
        serviceCollection.AddScoped<IEventRepository<Guid>, EventRepository>();
        serviceCollection.AddScoped<IEventRepository<Guid, Guid>, EventRepository>();

        serviceCollection.AddScoped<IEventHandlerRepository, EventRepository>();
        serviceCollection.AddScoped<IEventHandlerRepository<Guid>, EventRepository>();
    }

    public static void AddEventRepository<TEntityKey>(this IServiceCollection serviceCollection)
        where TEntityKey : struct
    {
        serviceCollection.AddScoped<IEventRepository<TEntityKey>, EventRepository<TEntityKey>>();
        serviceCollection.AddScoped<IEventRepository<Guid, TEntityKey>, EventRepository<TEntityKey>>();

        serviceCollection.AddScoped<IEventHandlerRepository, EventRepository<TEntityKey>>();
        serviceCollection.AddScoped<IEventHandlerRepository<Guid>, EventRepository<TEntityKey>>();
    }

    public static void AddQueryRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryRepository, QueryRepository>();
        serviceCollection.AddScoped<IQueryRepository<Guid>, QueryRepository>();

        serviceCollection.AddScoped<IQueryHandlerRepository, QueryRepository>();
        serviceCollection.AddScoped<IQueryHandlerRepository<Guid>, QueryRepository>();
    }
}
