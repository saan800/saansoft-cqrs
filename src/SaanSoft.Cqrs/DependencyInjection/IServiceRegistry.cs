namespace SaanSoft.Cqrs.DependencyInjection;

// TODO: add helper to add default services and config? eg serviceProvider.AddFromAssembly(...)

/// <summary>
/// Registry for checking if handlers are registered for given message types, and getting instances of types.
///
/// Can be implemented using any IoC container (eg .net core's default IServiceProvider, or others)
/// or a custom registry.
/// </summary>
public interface IServiceRegistry
{
    /// <summary>
    /// Checks if a message handler is registered in the current application for the given message type.
    /// Returns the number of handlers found
    /// </summary>
    /// <exception cref="ApplicationException" />
    int GetMessageHandlerCount<TMessage>() where TMessage : IMessage;

    /// <summary>
    /// Checks if a single message handler is registered in the current application for the given
    /// IMessage&lt;TResponse&gt;
    /// type.
    /// </summary>
    int GetMessageHandlerWithResponseCount<TMessage>() where TMessage : IMessage;

    /// <summary>
    /// Resolve a service (if any) of the given type from the underlying container.
    /// </summary>
    T? ResolveService<T>() where T : notnull;

    /// <summary>
    /// Resolve a service of the given type from the underlying container.
    /// If no service is found, and exception is thrown.
    /// </summary>
    T ResolveRequiredService<T>() where T : notnull;

    /// <summary>
    /// Resolve 0-n instances for the given type.
    /// </summary>
    IEnumerable<T> ResolveServices<T>() where T : notnull;
}
