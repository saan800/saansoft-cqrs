namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the command bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class LoggerScopeCommandBusDecorator<TMessageId>(ILogger logger, ICommandBus<TMessageId> next) :
    ICommandBusDecorator<TMessageId>
    where TMessageId : struct
{
    public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        using (logger.BeginScope(command.BuildLoggingScopeData()))
        {
            logger.LogInformation("Executing the command");
            await next.ExecuteAsync(command, cancellationToken);
        }
    }

    public async Task<TResponse> ExecuteAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
    {
        var typedCommand = (TCommand)command;
        using (logger.BeginScope(typedCommand.BuildLoggingScopeData()))
        {
            logger.LogInformation("Executing the command");
            return await next.ExecuteAsync(typedCommand, cancellationToken);
        }
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, IBaseCommand<TMessageId>
    {
        using (logger.BeginScope(command.BuildLoggingScopeData()))
        {
            logger.LogInformation("Queueing the command");
            await next.QueueAsync(command, cancellationToken);
        }
    }
}
