using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Handlers;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.DependencyInjection.ServiceProvider;

/// <summary>
/// Minimal service registry backed by IServiceProvider.
/// </summary>
public sealed class ServiceProviderRegistry(IServiceProvider serviceProvider) : IServiceRegistry
{
    private readonly ConcurrentDictionary<Type, int> _numHandlers = [];

    public int GetMessageHandlerCount<TMessage>() where TMessage : IMessage
    {
        var messageType = typeof(TMessage);
        if (_numHandlers.TryGetValue(messageType, out var numHandlers)) return numHandlers;

        // Construct IHandleMessage<TMessage> type at runtime
        var handlerType = typeof(IHandleMessage<>).MakeGenericType(messageType);

        numHandlers = CountServices(handlerType);
        _numHandlers.TryAdd(messageType, numHandlers);
        return numHandlers;

    }

    public int GetMessageHandlerWithResponseCount<TMessage>() where TMessage : IMessage
    {
        var messageType = typeof(TMessage);
        if (_numHandlers.TryGetValue(messageType, out var numHandlers)) return numHandlers;

        var i = messageType.GetInterfaces().FirstOrDefault(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IMessage<>)
            );
        if ((!messageType.IsMessageWithResponse()) || i == null)
        {
            throw new ApplicationException(
                $"{messageType.GetTypeFullName()} does not implement ICommand<TResponse> or IQuery<TResponse>"); ;
        }

        // Construct IHandleMessage<TMessage, TResponse> type at runtime
        var responseType = i.GetGenericArguments()[0];
        var handlerType = typeof(IHandleMessage<,>).MakeGenericType(messageType, responseType);

        numHandlers = CountServices(handlerType);
        _numHandlers.TryAdd(messageType, numHandlers);
        return numHandlers;
    }

    public T? ResolveService<T>() where T : notnull => serviceProvider.GetService<T>();

    public T ResolveRequiredService<T>() where T : notnull => serviceProvider.GetRequiredService<T>();

    public IEnumerable<T> ResolveServices<T>() where T : notnull => serviceProvider.GetServices<T>();

    /// <summary>
    /// Get the number of services registered for the type
    /// </summary>
    private int CountServices(Type serviceType)
    {
        var resolved = serviceProvider.GetServices(serviceType)?.Where(x => x != null);
        return resolved?.Count() ?? 0;
    }
}
