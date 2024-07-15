using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.EnsureCorrelationId;

/// <summary>
/// Ensure that the Command has the CorrelationId field populated with a non-null and non-default value
/// </summary>
/// <param name="providers"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class EnsureCommandHasCorrelationIdDecorator<TMessageId>(IEnumerable<ICorrelationIdProvider> providers, IBaseCommandBus<TMessageId> next)
    : IBaseCommandBus<TMessageId>
    where TMessageId : struct
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        command.Metadata.CorrelationId = providers.EnsureCorrelationId(command.Metadata.CorrelationId);
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
    {
        command.Metadata.CorrelationId = providers.EnsureCorrelationId(command.Metadata.CorrelationId);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        command.Metadata.CorrelationId = providers.EnsureCorrelationId(command.Metadata.CorrelationId);
        await next.QueueAsync(command, cancellationToken);
    }
}
