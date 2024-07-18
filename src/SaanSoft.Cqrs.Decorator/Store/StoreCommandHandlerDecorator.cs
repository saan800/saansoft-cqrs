namespace SaanSoft.Cqrs.Decorator.Store;

public class StoreCommandHandlerDecorator(ICommandHandlerRepository repository, ICommandSubscriptionBus next)
    : BaseStoreMessageHandlerDecorator(repository),
      ICommandSubscriptionBusDecorator
{
    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
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

    public async Task<TResponse> RunAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>
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

    public ICommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : class, ICommand
        => next.GetHandler<TCommand>();

    public ICommandHandler<TCommand, TResponse> GetHandler<TCommand, TResponse>()
        where TCommand : class, ICommand<TCommand, TResponse>
        => next.GetHandler<TCommand, TResponse>();
}
