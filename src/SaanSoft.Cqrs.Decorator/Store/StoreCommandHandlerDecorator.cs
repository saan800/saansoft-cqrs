namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreCommandHandlerDecorator(ICommandHandlerRepository<Guid> repository, ICommandSubscriptionBus<Guid> next)
    : StoreCommandHandlerDecorator<Guid>(repository, next);

public abstract class StoreCommandHandlerDecorator<TMessageId>(ICommandHandlerRepository<TMessageId> repository, ICommandSubscriptionBus<TMessageId> next)
    : BaseStoreMessageHandlerDecorator<TMessageId, ICommand<TMessageId>>(repository),
      ICommandSubscriptionBusDecorator<TMessageId>
    where TMessageId : struct
{
    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        var handler = GetHandler<TCommand>();
        try
        {
            await next.RunAsync(command, cancellationToken);
            await Repository.UpsertHandlerAsync(command, handler.GetType(), null, cancellationToken);
        }
        catch (Exception exception)
        {
            await Repository.UpsertHandlerAsync(command, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public async Task<TResponse> RunAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
    {
        var handler = GetHandler<TCommand, TResponse>();
        var typedCommand = (TCommand)command;
        try
        {
            var response = await next.RunAsync(command, cancellationToken);
            await Repository.UpsertHandlerAsync(typedCommand, handler.GetType(), null, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            await Repository.UpsertHandlerAsync(typedCommand, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public ICommandHandler<TCommand> GetHandler<TCommand>() where TCommand : ICommand<TMessageId>
        => next.GetHandler<TCommand>();

    public ICommandHandler<TCommand, TResponse> GetHandler<TCommand, TResponse>()
        where TCommand : ICommand<TCommand, TResponse>, ICommand<TMessageId, TCommand, TResponse>
        => next.GetHandler<TCommand, TResponse>();
}
