namespace SaanSoft.Cqrs.Middleware;

public interface ISubscriberMiddleware<TMessage> where TMessage : IMessage
{
    Task InvokeAsync(HandlerContext context, Func<Task> next, CancellationToken ct);
}
