namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

/// <summary>
/// Ensure that the Command has the CorrelationId field populated with a non-null and non-default value
/// </summary>
/// <param name="providers"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class EnsureCommandHasCorrelationIdDecorator<TMessageId>(IEnumerable<ICorrelationIdProvider> providers, ICommandBus<TMessageId> next)
    : ICommandBusDecorator<TMessageId>
    where TMessageId : struct
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TMessageId>
    {
        command.CorrelationId = providers.EnsureCorrelationId(command.CorrelationId);
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
    {
        command.CorrelationId = providers.EnsureCorrelationId(command.CorrelationId);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TMessageId>
    {
        command.CorrelationId = providers.EnsureCorrelationId(command.CorrelationId);
        await next.QueueAsync(command, cancellationToken);
    }
}
