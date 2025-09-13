namespace SaanSoft.Cqrs.Core.Bus;

public interface IMessageBus
{
    /// <summary>
    /// Execute a Command that does not expect a result, waits for completion
    ///
    /// Should mostly be used for commands that are handled by the same application instance.
    ///
    /// Exceptions that are thrown by the command handler will be propagated back to the caller.
    ///
    /// Commands that are handled asynchronously, e.g. via a queue or other out-of-process
    /// mechanism should use <see cref="SendAsync{TCommand}"/> instead.
    /// </summary>
    Task ExecuteAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand;

    /// <summary>
    /// Execute a Command that expects a result, waits for completion.
    ///
    /// Exceptions that are thrown by the command handler will be propagated back to the caller.
    ///
    /// This can be used for commands that are handled by either the same application instance or
    /// asynchronously via a queue or other out-of-process mechanism, but the caller will be waiting
    /// for the result.
    /// </summary>
    Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand<TResult>;

    /// <summary>
    /// Send Command that's not expecting a result, fire and forget from caller perspective.
    ///
    /// Exceptions that are thrown by the command handler will not be propagated back to the caller.
    ///
    /// Should be used for commands that are handled asynchronously, e.g. via a queue
    /// or other out-of-process mechanism.
    /// </summary>
    Task SendAsync<TCommand>(TCommand command, CancellationToken ct = default)
        where TCommand : ICommand;

    /// <summary>
    /// Publish Event - fire and forget from caller perspective
    ///
    /// Exceptions that are thrown by event handlers will not be propagated back to the caller.
    /// </summary>
    Task PublishAsync<TEvent>(TEvent evt, CancellationToken ct = default)
        where TEvent : IEvent;

    /// <summary>
    /// Publish many events of the same type - fire and forget from caller perspective
    ///
    /// Exceptions that are thrown by event handlers will not be propagated back to the caller.
    /// </summary>
    Task PublishManyAsync<TEvent>(IReadOnlyCollection<TEvent> events, CancellationToken ct = default)
        where TEvent : IEvent;

    /// <summary>
    /// Query - always wait for the result
    ///
    /// Exceptions that are thrown by the query handler will be propagated back to the caller.
    ///
    /// This can be used for queries that are handled by either the same application instance or
    /// asynchronously via a queue or other out-of-process mechanism, but the caller will be waiting
    /// for the result.
    /// </summary>
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default)
        where TQuery : IQuery<TResult>;
}
