using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Transport;

/// <summary>
/// Default: use in-memory if a handler is registered; otherwise external if registered.
/// </summary>
public sealed class RoutingStrategy(IServiceRegistry serviceRegistry) : IRoutingStrategy
{
    /// <summary>
    /// If IExternalMessageTransport is not registered and doing InMemory execution only, this will:
    /// - Throw an exception if multiple handlers for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; are
    ///   found in the serviceRegistry.
    /// - Throw an exception if no handlers for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; are not found
    ///   in the serviceRegistry.
    /// - Return false if a single handler for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; is found in
    ///   the serviceRegistry.
    /// - Return false for IEvent, regardless of the number of handlers in the serviceRegistry.
    ///
    /// If IExternalMessageTransport is registered, this will:
    /// - Throw an exception if multiple handlers for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; are
    ///   found in the serviceRegistry.
    /// - Return true if no handlers for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; are not found in the
    ///   serviceRegistry.
    /// - Return false if a single handler for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; is found in
    ///   the serviceRegistry.
    /// - Return true for IEvent, regardless of the number of handlers in the serviceRegistry (assures events are
    ///   handled by all subscribers, and in published order).
    /// </summary>
    /// <exception cref="ApplicationException">
    /// Any critical issues which arise during routing decision making.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Pass in a message that is not a command/event/query (ie probably inherited directory from IMessage)
    /// </exception>
    public bool IsExternalMessage<TMessage>(TMessage message) where TMessage : IMessage
    {
        var hasExternal = serviceRegistry.ResolveService<IExternalMessageTransport>() != null;
        var messageType = message.GetType();
        if (Implements(messageType, typeof(IEvent)))
        {
            // always return true if hasExternal=true - assures events are handled by all subscribers
            // (in memory and external), and in message published order
            // always return false if hasExternal=false - events can have 0-n handlers
            return hasExternal;
        }
        if (ImplementsOpenGeneric(messageType, typeof(ICommand<>)))
        {
            var any = serviceRegistry.HasCommandResultHandler(messageType);
            if (any) return false;
            return hasExternal
                ? true
                : throw new ApplicationException($"Could not find a handler for {messageType.GetTypeFullName()}");
        }
        if (Implements(messageType, typeof(ICommand)))
        {
            var any = serviceRegistry.HasCommandHandler(messageType);

            if (any) return false;
            return hasExternal
                ? true
                : throw new ApplicationException($"Could not find a handler for {messageType.GetTypeFullName()}");
        }
        if (ImplementsOpenGeneric(messageType, typeof(IQuery<>)))
        {
            var any = serviceRegistry.HasQueryHandler(messageType);

            if (any) return false;
            return hasExternal
                ? true
                : throw new ApplicationException($"Could not find a handler for {messageType.GetTypeFullName()}");
        }
        if (Implements(messageType, typeof(IMessage)))
        {
            throw new NotSupportedException($"{messageType.GetTypeFullName()} directly implements {nameof(IMessage)}. Messages must use ICommand, IEvent or IQuery");
        }
        throw new NotSupportedException($"Unknown message type: {messageType.GetTypeFullName()}");
    }

    /// <summary>
    /// Check if 'type' implements the provided interface 'iFace'.
    /// </summary>
    private static bool Implements(Type type, Type iFace) => iFace.IsAssignableFrom(type);

    /// <summary>
    /// Check if 'type' implements the open generic interface 'openGeneric'.
    /// </summary>
    private static bool ImplementsOpenGeneric(Type type, Type openGeneric) =>
        type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == openGeneric);
}
