namespace SaanSoft.Cqrs.Middleware;

public interface IPublisherMiddleware<TMessage> where TMessage : IMessage
{
    Task InvokeAsync(PublishContext context, Func<Task> next, CancellationToken ct);
}
