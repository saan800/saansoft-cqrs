namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the command bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
public class LoggerScopeCommandBusDecorator(ILogger logger, ICommandBus next) : ICommandBus
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        using (logger.BeginScope(command.BuildLoggingScopeData()))
        {
            logger.LogInformation("Executing the command");
            await next.ExecuteAsync(command, cancellationToken);
        }
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(ICommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand<TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        using (logger.BeginScope(typedCommand.BuildLoggingScopeData()))
        {
            logger.LogInformation("Executing the command");
            return await next.ExecuteAsync(typedCommand, cancellationToken);
        }
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        using (logger.BeginScope(command.BuildLoggingScopeData()))
        {
            logger.LogInformation("Queueing the command");
            await next.QueueAsync(command, cancellationToken);
        }
    }
}
