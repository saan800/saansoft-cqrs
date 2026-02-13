using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Transport;
using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Bus.Transport;

/// <summary>
/// Default Routing Strategy: use local router if a handler is registered locally or
/// <see cref="IExternalMessageProvider"/> is not registered; otherwise use the external
/// router if message can't be handled in memory locally
/// </summary>
public sealed class RoutingStrategy(IServiceRegistry serviceRegistry)
{
    /// <summary>
    /// Decide if need to route the message externally or use local (default) transport,
    /// and then return the correct <see cref="IMessageRouter"/>
    /// </summary>
    /// <remarks>
    /// If IExternalMessageProvider is not registered and doing local execution only, this will:
    /// - Throw an exception if multiple handlers for ICommand, ICommand&lt;TResponse&gt;, IQuery&lt;TResponse&gt; are
    ///   found in the serviceRegistry.
    /// - Throw an exception if no handlers for ICommand, ICommand&lt;TResponse&gt;, IQuery&lt;TResponse&gt; are not
    ///   found in the serviceRegistry.
    /// - Return LocalMessageRouter if a single handler for ICommand, ICommand&lt;TResponse&gt;,
    ///   IQuery&lt;TResponse&gt; is found in the serviceRegistry.
    /// - Return LocalMessageRouter for IEvent, regardless of the number of handlers in the serviceRegistry.
    ///
    /// If IExternalMessageProvider is registered, this will:
    /// - Throw an exception if multiple handlers for ICommand, ICommand&lt;TResponse&gt;, IQuery&lt;TResponse&gt; are
    ///   found in the serviceRegistry.
    /// - Return ExternalMessageRouter if no handlers for ICommand, ICommand&lt;TResponse&gt;,
    ///   IQuery&lt;TResponse&gt; are not found in the serviceRegistry.
    /// - Return LocalMessageRouter if a single handler for ICommand, ICommand&lt;TResponse&gt;,
    ///   IQuery&lt;TResponse&gt; is found in the serviceRegistry.
    /// - Return ExternalMessageRouter for IEvent, regardless of the number of handlers in the serviceRegistry
    ///   (assures events are handled by all subscribers, and in published order).
    /// </remarks>
    /// <exception cref="ApplicationException">
    /// Any critical issues which arise during routing decision making.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Pass in a message that is not a command/event/query (ie probably inherited directly from IMessage)
    /// </exception>
    public IMessageRouter GetMessageRouter<TMessage>() where TMessage : IMessage
        => IsExternalMessage<TMessage>()
            ? serviceRegistry.ResolveRequiredService<ExternalMessageRouter>()
            : serviceRegistry.ResolveRequiredService<LocalMessageRouter>();

    private bool IsExternalMessage<TMessage>() where TMessage : IMessage
    {
        var hasExternal = serviceRegistry.ResolveService<IExternalMessageProvider>() != null;
        var messageType = typeof(TMessage);
        if (messageType.IsEvent())
        {
            // always return true if hasExternal=true - assures events are handled by all subscribers
            // (local and external), and in message published order
            // always return false if hasExternal=false - events can have 0-n handlers
            return hasExternal;
        }

        if (messageType.IsMessageWithResponse())
        {
            var numHandlers = serviceRegistry.GetMessageHandlerWithResponseCount<TMessage>();
            if (numHandlers > 1)
            {
                throw new ApplicationException($"Multiple handlers found for {messageType.GetTypeFullName()}");
            }
            if (numHandlers == 1) return false;

            return hasExternal
                ? true
                : throw new ApplicationException($"Could not find a handler for {messageType.GetTypeFullName()}");
        }

        if (messageType.IsCommand())
        {
            var numHandlers = serviceRegistry.GetMessageHandlerCount<TMessage>();
            if (numHandlers > 1)
            {
                throw new ApplicationException($"Multiple handlers found for {messageType.GetTypeFullName()}");
            }

            if (numHandlers == 1) return false;

            return hasExternal
                ? true
                : throw new ApplicationException($"Could not find a handler for {messageType.GetTypeFullName()}");
        }

        if (messageType.Implements(typeof(IMessage)))
        {
            throw new NotSupportedException(
                @$"{messageType.GetTypeFullName()} directly implements {nameof(IMessage)}. " +
                " Messages must implement ICommand, IEvent or IQuery");
        }
        throw new NotSupportedException($"Unknown message type: {messageType.GetTypeFullName()}");
    }
}
