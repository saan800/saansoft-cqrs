using SaanSoft.Cqrs.Common.Handlers;
using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.Decorator.LoggerScope;

/// <summary>
/// Add ILogger.BeginScope structured log format to the command subscription bus
/// </summary>
/// <param name="logger"></param>
/// <param name="next"></param>
/// <typeparam name="TMessageId"></typeparam>
public abstract class LoggerScopeCommandSubscriptionBus<TMessageId>(ILogger logger, IBaseCommandSubscriptionBus<TMessageId> next) :
    IBaseCommandSubscriptionBus<TMessageId>
    where TMessageId : struct
{
    public async Task RunAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : class, IBaseCommand<TMessageId>
    {
        var handler = GetHandler<TCommand>();
        using (logger.BeginScope(command.BuildLoggingScopeData(handler.GetType())))
        {
            logger.LogInformation("Running command handler");
            await next.RunAsync(command, cancellationToken);
        }
    }

    public async Task<TResponse> RunAsync<TCommand, TResponse>(IBaseCommand<TCommand, TResponse> command, CancellationToken cancellationToken = default) where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
    {
        var handler = GetHandler<TCommand, TResponse>();
        var typedCommand = (TCommand)command;
        using (logger.BeginScope(typedCommand.BuildLoggingScopeData(handler.GetType())))
        {
            logger.LogInformation("Running command handler");
            return await next.RunAsync(typedCommand, cancellationToken);
        }
    }

    public IBaseCommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : class, IBaseCommand<TMessageId>
        => next.GetHandler<TCommand>();

    public IBaseCommandHandler<TCommand, TResponse> GetHandler<TCommand, TResponse>()
        where TCommand : class, IBaseCommand<TCommand, TResponse>, IBaseCommand<TMessageId, TCommand, TResponse>
        => next.GetHandler<TCommand, TResponse>();
}
