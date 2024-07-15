using SaanSoft.Cqrs.Common.Handlers;
using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.Store;

public abstract class StoreCommandHandlerDecorator<TMessageId>(ICommandHandlerRepository<TMessageId> repository, IBaseCommandSubscriptionBus<TMessageId> next)
    : BaseStoreMessageHandlerDecorator<TMessageId>(repository),
      IBaseCommandSubscriptionBus<TMessageId>
    where TMessageId : struct
{
    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        var handler = GetHandler<TCommand>();
        try
        {
            await next.RunAsync(command, cancellationToken);
            await Repository.UpsertHandlerAsync(command.Id, handler.GetType(), null, cancellationToken);
        }
        catch (Exception exception)
        {
            await Repository.UpsertHandlerAsync(command.Id, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public async Task<TResponse> RunAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
    {
        var handler = GetHandler<TCommand, TResponse>();
        var typedCommand = (TCommand)command;
        try
        {
            var response = await next.RunAsync(command, cancellationToken);
            await Repository.UpsertHandlerAsync(typedCommand.Id, handler.GetType(), null, cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            await Repository.UpsertHandlerAsync(typedCommand.Id, handler.GetType(), exception, cancellationToken);
            throw;
        }
    }

    public IBaseCommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : class, IBaseCommand<TMessageId>
        => next.GetHandler<TCommand>();

    public IBaseCommandHandler<TCommand, TResponse> GetHandler<TCommand, TResponse>()
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
        => next.GetHandler<TCommand, TResponse>();
}
