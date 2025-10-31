namespace SaanSoft.Cqrs.DependencyInjection;

/// <summary>
/// Registry for checking if handlers are registered for given message types, and getting instances of types.
///
/// Can be implemented using any IoC container (eg .net core's default IServiceProvider, or others)
/// or a custom registry.
/// </summary>
public interface IServiceRegistry
{
    /// <summary>
    /// Checks if a command handler is registered in the current application for the given ICommand type.
    /// Throws ApplicationException if more than one handler for the command.
    /// </summary>
    /// <exception cref="ApplicationException" />
    bool HasCommandHandler<TCommand>() where TCommand : IMessage;

    /// <summary>
    /// Checks if a command handler is registered in the current application for the given ICommand&lt;TResponse&gt; type.
    /// Throws ApplicationException if more than one handler for the command.
    /// </summary>
    /// <exception cref="ApplicationException" />
    bool HasCommandWithResponseHandler<TCommand>() where TCommand : IMessage;

    /// <summary>
    /// Checks if a query handler is registered in the current application for the given IQuery&lt;TResponse&gt; type.
    /// Throws ApplicationException if more than one handler for the query.
    /// </summary>
    /// <exception cref="ApplicationException" />
    bool HasQueryHandler<TQuery>() where TQuery : IMessage;

    /// <summary>
    /// Checks if any event handlers are registered in the current application for the given IEvent type.
    /// </summary>
    /// <returns>
    /// false: no handlers for the event type
    /// true: one or more handlers for the event type
    /// </returns>
    bool HasEventHandlers<TEvent>() where TEvent : IMessage;

    /// <summary>
    /// Resolve a service (if any) of the given type from the underlying container.
    /// </summary>
    T? ResolveService<T>();

    /// <summary>
    /// Resolve a service of the given type from the underlying container.
    /// If no service is found, and exception is thrown.
    /// </summary>
    T ResolveRequiredService<T>() where T : notnull;

    /// <summary>
    /// Resolve 0-n instances for the given type.
    /// </summary>
    /// <remarks>
    /// Useful for finding potentially multiple handlers of events
    /// </remarks>
    IEnumerable<T> ResolveServices<T>();
}
