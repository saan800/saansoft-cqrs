using System.Diagnostics;
using SaanSoft.Cqrs.Messages;
using SaanSoft.Cqrs.Store;

namespace SaanSoft.Cqrs.Bus.Decorator;

public class StoreCommandPublisherDecorator(ICommandPublisherStore store, ICommandPublisher<Guid> next)
    : StoreCommandPublisherDecorator<Guid>(store, next);

public class StoreCommandPublisherDecorator<TMessageId>(
    ICommandPublisherStore store,
    ICommandPublisher<TMessageId> next)
    : ICommandPublisher<TMessageId> where TMessageId : struct
{
    public async Task<CommandResponse> ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        await StorePublisher<TCommand>(cancellationToken);
        return await next.ExecuteAsync(command, cancellationToken);
    }

    public async Task QueueAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TMessageId>
    {
        await StorePublisher<TCommand>(cancellationToken);
        await next.QueueAsync(command, cancellationToken);
    }

    private async Task StorePublisher<TCommand>(CancellationToken cancellationToken)
    {
        var commandType = typeof(TCommand);
        if (commandType == null) return;

        var callerClassType = new StackTrace().GetFrames()
            .Where(f => !string.IsNullOrWhiteSpace(f.GetMethod()?.DeclaringType?.Namespace))
            .Where(f => f.GetMethod()!.DeclaringType.IsClass)
            .Where(f => f.GetMethod()!.DeclaringType.IsVisible)
            .Where(f => !f.GetMethod()!.DeclaringType.Namespace.StartsWith("System"))
            .FirstOrDefault(f => f.GetMethod()!.DeclaringType.GetInterface(typeof(ICommandPublisher<TMessageId>).Name) == null)
            ?.GetMethod()
            ?.DeclaringType;

        var commandTypeName = commandType.FullName ?? commandType.Name;
        var publishedByName = callerClassType?.FullName ?? callerClassType?.Name ?? "Unknown";
        await store.UpsertPublisherAsync(commandTypeName, publishedByName, cancellationToken);
    }
}
