using Microsoft.Extensions.DependencyInjection;

namespace SaanSoft.Cqrs.Decorator.Store.MongoDB;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Guid implementations of the command, event and query repositories
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCommandRepository();
        serviceCollection.AddEventRepository();
        serviceCollection.AddQueryRepository();
    }

    public static void AddCommandRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICommandRepository, CommandRepository>();

        serviceCollection.AddScoped<ICommandHandlerRepository, CommandRepository>();
    }

    public static void AddEventRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEventRepository, EventRepository>();
        serviceCollection.AddScoped<IEventRepository<Guid>, EventRepository>();

        serviceCollection.AddScoped<IEventHandlerRepository, EventRepository>();
    }

    public static void AddEventRepository<TEntityKey>(this IServiceCollection serviceCollection)
        where TEntityKey : struct
    {
        serviceCollection.AddScoped<IEventRepository<TEntityKey>, EventRepository<TEntityKey>>();

        serviceCollection.AddScoped<IEventHandlerRepository, EventRepository<TEntityKey>>();
    }

    public static void AddQueryRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryRepository, QueryRepository>();

        serviceCollection.AddScoped<IQueryHandlerRepository, QueryRepository>();
    }
}
