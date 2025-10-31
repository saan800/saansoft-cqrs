using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.Bus.Utilities;

// TODO: test

public static class MiddlewareExtensions
{
    public static Task InvokeAsync<TMessage>(
        this IMiddleware[] allMiddlewares, MessageEnvelope envelope, CancellationToken ct)
        where TMessage : IMessage
        => InvokeAsync<TMessage>(allMiddlewares, envelope, () => Task.CompletedTask, ct);

    public static Task InvokeAsync<TMessage>(
        this IMiddleware[] allMiddlewares, MessageEnvelope envelope, Func<Task> terminal, CancellationToken ct)
        where TMessage : IMessage
    {
        var validMiddlewares = allMiddlewares
            .Where(m => m.IsValidForMessage<TMessage>())
            .Select(mw => (
                Func<Func<Task>, Func<Task>>)(next => () => mw.InvokeAsync<TMessage>(envelope, next, ct)
            ));

        var next = terminal;
        foreach (var middleware in validMiddlewares)
        {
            next = middleware(next);
        }
        return next();
    }
}
