using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Decorator.EnsureMessageId;

/// <summary>
/// Ensure that the Command has the Id field populated with a non-null and non-default value
/// </summary>
/// <param name="idGenerator"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class EnsureCommandHasIdDecorator<TMessageId>(IIdGenerator<TMessageId> idGenerator, ICommandBus<TMessageId> next) :
    ICommandBusDecorator<TMessageId> where TMessageId : struct
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TMessageId>
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = idGenerator.NewId();
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        if (GenericUtils.IsNullOrDefault(typedCommand.Id)) typedCommand.Id = idGenerator.NewId();
        return await next.ExecuteAsync(typedCommand, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TMessageId>
    {
        if (GenericUtils.IsNullOrDefault(command.Id)) command.Id = idGenerator.NewId();
        await next.ExecuteAsync(command, cancellationToken);
    }
}
