using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Bus.Transport;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Transport;

namespace SaanSoft.Cqrs.DependencyInjection.ServiceProvider;

// TODO: tests

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Setup the default services and options for SaanSoft.Cqrs. Configures everything required to create and
    /// handle messages in the current application (ie runs in memory)
    /// </summary>
    /// <remarks>
    /// Requires ILogger&lt;TClass&gt; to be available from the IServiceProvider
    /// </remarks>
    public static IServiceCollection AddCqrs(this IServiceCollection services, AddCqrsOptions? options = null)
    {
        options ??= new AddCqrsOptions();
        var lifetimes = options.ServiceLifetimes;

        services.AddServiceWithLifetime<IServiceRegistry, ServiceProviderRegistry>(lifetimes.ServiceRegistry);
        services.AddServiceWithLifetime<RoutingStrategy>(lifetimes.RoutingStrategy);

        services.AddServiceWithLifetime(lifetimes.LocalMessageRouter, provider => new LocalMessageRouter(
                provider.GetRequiredService<ILogger<LocalMessageRouter>>(),
                provider.GetRequiredService<IServiceRegistry>(),
                provider.GetService<ILocalMessageRouterOptions>(),
                options.HandlerMiddlewares));

        services.AddServiceWithLifetime<IMessageBus>(lifetimes.MesageBus, provider => new MessageBus(
                provider.GetRequiredService<ILogger<MessageBus>>(),
                provider.GetRequiredService<RoutingStrategy>(),
                options.PublisherMiddlewares));

        return services;
    }

    /// <summary>
    /// Options used to configure the LocalMessageRouter for in memory processing of messages
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="LocalMessageRouterOptions"/> if not configured
    /// </remarks>
    public static IServiceCollection WithLocalMessageRouterOptions(
        this IServiceCollection services, ILocalMessageRouterOptions options)
        => services.AddSingleton(_ => options);

    /// <summary>
    /// Options used to configure the LocalMessageRouter for handling of messages
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="LocalMessageRouterOptions"/> if not configured
    /// </remarks>
    public static IServiceCollection WithLocalMessageRouterOptions<TOptions>(this IServiceCollection services)
        where TOptions : class, ILocalMessageRouterOptions
        => services.AddSingleton<ILocalMessageRouterOptions, TOptions>();

    /// <summary>
    /// Options used to configure the ExternalMessageRouter for sending messages to the IExternalMessageProvider
    /// (if configured)
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="DefaultExternalMessageRouterOptions"/> if not configured
    /// </remarks>
    public static IServiceCollection WithExternalMessageRouterOptions<TOptions>(this IServiceCollection services)
        where TOptions : class, IDefaultExternalMessageRouterOptions
        => services.AddSingleton<IDefaultExternalMessageRouterOptions, TOptions>();

    /// <summary>
    /// Add an external pub/sub message provider (eg Azure Service Bus, AWS SNS/SQS, RabbitMq)
    /// </summary>
    public static IServiceCollection WithExternalMessageProvider<TMessageProvider>(
        this IServiceCollection services, WithExternalMessageProviderOptions? options = null)
        where TMessageProvider : class, IExternalMessageProvider
    {
        options ??= new WithExternalMessageProviderOptions();
        var lifetimes = options.ServiceLifetimes;

        // TODO: external subscriber??

        services.AddServiceWithLifetime<IExternalMessageProvider, TMessageProvider>(lifetimes.ExternalMessageProvider);
        services.AddServiceWithLifetime(lifetimes.ExternalMessageRouter, provider =>
        {
            var routerOptions = options.DefaultExternalMessageRouterOptions
                ?? provider.GetService<IDefaultExternalMessageRouterOptions>();

            return new ExternalMessageRouter(
                provider.GetRequiredService<ILogger<ExternalMessageRouter>>(),
                provider.GetRequiredService<IExternalMessageProvider>(),
                routerOptions,
                options.PublisherMiddlewares);
        });

        return services;
    }

    /// <summary>
    /// Add all <see cref="IHandleMessage{TMessage}"/> and <see cref="IHandleMessage{TMessage, TResponse}"/> in
    /// the provided assembly
    /// </summary>
    public static IServiceCollection WithMessageHandlersFromAssembly(
        this IServiceCollection services, Assembly assembly)
    {
        // TODO:

        return services;
    }

    private static IServiceCollection AddServiceWithLifetime<TService>(
        this IServiceCollection services, ServiceLifetime lifetime)
        where TService : class
    {
        if (lifetime == ServiceLifetime.Transient)
        {
            services.AddTransient<TService>();
        }
        else if (lifetime == ServiceLifetime.Scoped)
        {
            services.AddScoped<TService>();
        }
        else if (lifetime == ServiceLifetime.Singleton)
        {
            services.AddSingleton<TService>();
        }
        return services;
    }

    private static IServiceCollection AddServiceWithLifetime<TInterface, TImplementation>(
        this IServiceCollection services, ServiceLifetime lifetime)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        if (lifetime == ServiceLifetime.Transient)
        {
            services.AddTransient<TInterface, TImplementation>();
        }
        else if (lifetime == ServiceLifetime.Scoped)
        {
            services.AddScoped<TInterface, TImplementation>();
        }
        else if (lifetime == ServiceLifetime.Singleton)
        {
            services.AddSingleton<TInterface, TImplementation>();
        }
        return services;
    }

    public static IServiceCollection AddServiceWithLifetime<TInterface>(
        this IServiceCollection services, ServiceLifetime lifetime, Func<IServiceProvider, TInterface> func)
        where TInterface : class
    {

        if (lifetime == ServiceLifetime.Transient)
        {
            services.AddTransient(func);
        }
        else if (lifetime == ServiceLifetime.Scoped)
        {
            services.AddScoped(func);
        }
        else if (lifetime == ServiceLifetime.Singleton)
        {
            services.AddSingleton(func);
        }
        return services;
    }
}
