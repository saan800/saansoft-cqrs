using SaanSoft.Cqrs.Handlers;

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
    bool HasCommandHandler(Type commandType);

    /// <summary>
    /// Checks if a command handler is registered in the current application for the given ICommand&lt;TResult&gt; type.
    /// Throws ApplicationException if more than one handler for the command.
    /// </summary>
    /// <exception cref="ApplicationException" />
    bool HasCommandResultHandler(Type commandType);

    /// <summary>
    /// Checks if a query handler is registered in the current application for the given IQuery&lt;TResult&gt; type.
    /// Throws ApplicationException if more than one handler for the query.
    /// </summary>
    /// <exception cref="ApplicationException" />
    bool HasQueryHandler(Type queryType);

    /// <summary>
    /// Checks if any event handlers are registered in the current application for the given IEvent type.
    /// </summary>
    /// <returns>
    /// false: no handlers for the event type
    /// true: one or more handlers for the event type
    /// </returns>
    bool HasEventHandlers(Type eventType);

    /// <summary>
    /// Resolve a single handler instance for the given handler type.
    /// </summary>
    /// <remarks>
    /// Throws <see cref="ApplicationException"/> if multiple handlers are found for the given type.
    /// Returns null if no handlers are found.
    /// </remarks>
    /// <exception cref="ApplicationException">Multiple handlers found for the given type.</exception>
    object? ResolveSingleHandler(Type handlerType);

    // TODO: ICommandHandler<TCommand> GetCommandHandler<TCommand>(TCommand command) where TCommand : ICommand;

    /// <summary>
    /// Resolve multiple handler instances for the given handler type.
    /// </summary>
    IEnumerable<object> ResolveMultipleHandlers(Type handlerType);

    /// <summary>
    /// Resolve a service (if any) of the given type from the underlying container.
    /// </summary>
    T? ResolveService<T>();

    /// <summary>
    /// Resolve a service of the given type from the underlying container.
    /// If no service is found, and exception is thrown.
    /// </summary>
    T ResolveRequiredService<T>() where T : notnull;
}
