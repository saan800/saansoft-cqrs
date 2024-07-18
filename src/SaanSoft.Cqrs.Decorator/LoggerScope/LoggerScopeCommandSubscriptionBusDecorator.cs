namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the command subscription bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
public class LoggerScopeCommandSubscriptionBusDecorator(ILogger logger, ICommandSubscriptionBus next) :
    ICommandSubscriptionBusDecorator
{
    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        var handler = GetHandler<TCommand>();
        using (logger.BeginScope(command.BuildLoggingScopeData(handler.GetType())))
        {
            logger.LogInformation("Running command handler");
            await next.RunAsync(command, cancellationToken);
        }
    }

    public async Task<TResponse> RunAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default) where TCommand : class, ICommand<TCommand, TResponse>
    {
        var handler = GetHandler<TCommand, TResponse>();
        var typedCommand = (TCommand)command;
        using (logger.BeginScope(typedCommand.BuildLoggingScopeData(handler.GetType())))
        {
            logger.LogInformation("Running command handler");
            return await next.RunAsync(typedCommand, cancellationToken);
        }
    }

    public ICommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : class, ICommand
        => next.GetHandler<TCommand>();

    public ICommandHandler<TCommand, TResponse> GetHandler<TCommand, TResponse>()
        where TCommand : class, ICommand<TCommand, TResponse>
        => next.GetHandler<TCommand, TResponse>();
}
