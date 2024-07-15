namespace SaanSoft.Cqrs.Core.Bus;

public interface IBaseCommandSubscriptionBus<TMessageId> :
    IBaseBus
    where TMessageId : struct
{
    /// <summary>
    /// Runs a command and wait for it to finish running.
    ///
    /// Any issues in the command could result in an exception being thrown.
    ///
    /// Commands will not be run in replay mode.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>;

    /// <summary>
    /// Runs a command and return the response.
    ///
    /// Any issues in the command could result in an exception being thrown.
    ///
    /// Commands will not be run in replay mode.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    Task<TResponse> RunAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>;

    /// <summary>
    /// Get the handler for the command.
    ///
    /// Should have exactly one command handler.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <returns></returns>
    IBaseCommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : class, IBaseCommand<TMessageId>;

    /// <summary>
    /// Get the handler for the command.
    ///
    /// Should have exactly one command handler.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    IBaseCommandHandler<TCommand, TResponse> GetHandler<TCommand, TResponse>()
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>;
}
