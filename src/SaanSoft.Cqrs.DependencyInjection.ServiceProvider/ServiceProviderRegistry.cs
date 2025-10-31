using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.DependencyInjection.ServiceProvider;

// TODO: add helper to add default services and config?

/// <summary>
/// Minimal service registry backed by IServiceProvider.
/// </summary>
public sealed class ServiceProviderRegistry(IServiceProvider serviceProvider) : IServiceRegistry
{
    private readonly ConcurrentDictionary<Type, bool> _hasHandler = [];

    public bool HasCommandHandler<TCommand>() where TCommand : IMessage
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider2 = serviceCollection.BuildServiceProvider();

        var messageType = typeof(TCommand);
        if (_hasHandler.TryGetValue(messageType, out var hasHandler)) return hasHandler;

        if (!messageType.IsCommand())
        {
            throw new ApplicationException(
                $"{messageType.GetTypeFullName()} does not implement {nameof(ICommand)}");
        }

        // Construct ICommandHandler<TCommand> at runtime
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(messageType);

        var response = HasSingleHandler(handlerType);
        _hasHandler.TryAdd(messageType, response);
        return response;
    }

    public bool HasCommandWithResponseHandler<TCommand>() where TCommand : IMessage
    {
        var messageType = typeof(TCommand);
        if (_hasHandler.TryGetValue(messageType, out var hasHandler)) return hasHandler;

        var i = messageType.GetInterfaces().FirstOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(ICommand<>)
            );
        if (!messageType.IsCommandWithResponse() || i == null)
        {
            throw new ApplicationException(
                $"{messageType.GetTypeFullName()} does not implement {nameof(ICommand)} with return type"); ;
        }

        // Construct ICommandHandler<TCommand, TResponse> at runtime
        var responseType = i.GetGenericArguments()[0];
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(messageType, responseType);

        var response = HasSingleHandler(handlerType);
        _hasHandler.TryAdd(messageType, response);
        return response;
    }

    public bool HasQueryHandler<TQuery>() where TQuery : IMessage
    {
        var messageType = typeof(TQuery);
        if (_hasHandler.TryGetValue(messageType, out var hasHandler)) return hasHandler;

        var i = messageType.GetInterfaces().FirstOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IQuery<>)
            );
        if (!messageType.IsQuery() || i == null)
        {
            throw new ApplicationException($"{messageType.GetTypeFullName()} does not implement IQuery");
        }

        // Construct IQueryHandler<TQuery, TResponse> at runtime
        var responseType = i.GetGenericArguments()[0];
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(messageType, responseType);

        var response = HasSingleHandler(handlerType);
        _hasHandler.TryAdd(messageType, response);
        return response;
    }

    public bool HasEventHandlers<TEvent>() where TEvent : IMessage
    {
        var messageType = typeof(TEvent);
        if (_hasHandler.TryGetValue(messageType, out var hasHandler)) return hasHandler;

        if (!messageType.IsEvent())
        {
            throw new ApplicationException(
                $"{messageType.GetTypeFullName()} does not implement {nameof(IEvent)}");
        }

        // Construct IEventHandler<TEvent> at runtime
        var handlerType = typeof(IEventHandler<>).MakeGenericType(messageType);

        var handlers = serviceProvider.GetServices(handlerType);
        var response = handlers.Any();
        _hasHandler.TryAdd(messageType, response);
        return response;
    }

    public T? ResolveService<T>() => serviceProvider.GetService<T>();

    public T ResolveRequiredService<T>() where T : notnull => serviceProvider.GetRequiredService<T>();

    public IEnumerable<T> ResolveServices<T>() => serviceProvider.GetServices<T>();

    /// <summary>
    /// Check if there is an instance of the handler type
    /// Having more than one handler instance throws the exception ApplicationException
    /// </summary>
    private bool HasSingleHandler(Type handlerType)
    {
        var resolved = serviceProvider.GetServices(handlerType);
        if (resolved.Count() > 1)
        {
            throw new ApplicationException($"Multiple handlers found for {handlerType.GetTypeFullName()}");
        }
        return resolved.Count() == 1;
    }
}
