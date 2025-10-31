using System.Runtime.CompilerServices;
using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.Bus;

public interface IMessageBus
{
    /// <summary>
    /// Execute a Command that does not expect a response, waits for completion
    ///
    /// Should mostly be used for commands that are handled by the same application instance.
    ///
    /// Exceptions that are thrown by the command handler will be propagated back to the caller.
    ///
    /// Commands that are handled asynchronously, e.g. via a queue or other out-of-process
    /// mechanism should use <see cref="SendAsync{TCommand}"/> instead.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <param name="callerFile">
    /// Used to populate the <see cref="MessageEnvelope.Publisher"/> at runtime. Parameter does not
    /// need to be supplied by caller, it should be populated automatically by
    /// System.Runtime.CompilerServices.CallerFilePath
    /// </param>
    Task ExecuteAsync<TCommand>(
        TCommand command,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TCommand : ICommand;

    /// <summary>
    /// Execute a Command that expects a response, waits for completion.
    ///
    /// Exceptions that are thrown by the command handler will be propagated back to the caller.
    ///
    /// This can be used for commands that are handled by either the same application instance or
    /// asynchronously via a queue or other out-of-process mechanism, but the caller will be waiting
    /// for the response.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse">Must match the TResponse of the TCommand</typeparam>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <param name="callerFile">
    /// Used to populate the <see cref="MessageEnvelope.Publisher"/> at runtime. Parameter does not
    /// need to be supplied by caller, it should be populated automatically by
    /// System.Runtime.CompilerServices.CallerFilePath
    /// </param>
    /// <returns></returns>
    Task<TResponse> ExecuteAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TCommand : ICommand<TResponse>;

    /// <summary>
    /// Send Command that's not expecting a response, fire-and-forget from caller perspective.
    ///
    /// Exceptions that are thrown by the command handler will not be propagated back to the caller.
    ///
    /// Should be used for commands that are handled asynchronously, e.g. via a queue
    /// or other out-of-process mechanism.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <param name="callerFile">
    /// Used to populate the <see cref="MessageEnvelope.Publisher"/> at runtime. Parameter does not
    /// need to be supplied by caller, it should be populated automatically by
    /// System.Runtime.CompilerServices.CallerFilePath
    /// </param>
    Task SendAsync<TCommand>(
        TCommand command,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TCommand : ICommand;

    /// <summary>
    /// Publish Event - fire and forget from caller perspective
    ///
    /// Exceptions that are thrown by event handlers will not be propagated back to the caller.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="evt"></param>
    /// <param name="ct"></param>
    /// <param name="callerFile">
    /// Used to populate the <see cref="MessageEnvelope.Publisher"/> at runtime. Parameter does not
    /// need to be supplied by caller, it should be populated automatically by
    /// System.Runtime.CompilerServices.CallerFilePath
    /// </param>
    Task PublishAsync<TEvent>(
        TEvent evt,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TEvent : IEvent;

    /// <summary>
    /// Publish many events of the same type - fire and forget from caller perspective
    ///
    /// Exceptions that are thrown by event handlers will not be propagated back to the caller.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="events"></param>
    /// <param name="ct"></param>
    /// <param name="callerFile">
    /// Used to populate the <see cref="MessageEnvelope.Publisher"/> at runtime. Parameter does not
    /// need to be supplied by caller, it should be populated automatically by
    /// System.Runtime.CompilerServices.CallerFilePath
    /// </param>
    Task PublishManyAsync<TEvent>(
        IReadOnlyCollection<TEvent> events,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TEvent : IEvent;

    /// <summary>
    /// Query - always wait for the response
    ///
    /// Exceptions that are thrown by the query handler will be propagated back to the caller.
    ///
    /// This can be used for queries that are handled by either the same application instance or
    /// asynchronously via a queue or other out-of-process mechanism, but the caller will be waiting
    /// for the response.
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="query"></param>
    /// <param name="ct"></param>
    /// <param name="callerFile">
    /// Used to populate the <see cref="MessageEnvelope.Publisher"/> at runtime. Parameter does not
    /// need to be supplied by caller, it should be populated automatically by
    /// System.Runtime.CompilerServices.CallerFilePath
    /// </param>
    Task<TResponse> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken ct = default,
        [CallerFilePath] string callerFile = "")
        where TQuery : IQuery<TResponse>;
}
