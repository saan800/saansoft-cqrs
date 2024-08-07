namespace SaanSoft.Cqrs.Decorator.Store;

/// <summary>
/// Add the publisher to the command's metadata.
///
/// Should be used in conjunction with <see cref="StoreCommandDecorator"/>
/// </summary>
/// <param name="next"></param>
// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public class StoreCommandPublisherDecorator(ICommandBus next) :
      BaseStoreMessagePublisherDecorator,
      ICommandBus
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await AddPublisherToMetadataAsync<ICommandBus>(command);
        await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        await AddPublisherToMetadataAsync<ICommandBus>(typedCommand);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await AddPublisherToMetadataAsync<ICommandBus>(command);
        await next.QueueAsync(command, cancellationToken);
    }
}
