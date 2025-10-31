using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.Bus.Transport;

public interface IMessageRouter
{
    Task ExecuteAsync<TCommand>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand;

    Task<TResponse> ExecuteAsync<TCommand, TResponse>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand<TResponse>;

    Task SendAsync<TCommand>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand;

    Task<TResponse> QueryAsync<TQuery, TResponse>(MessageEnvelope envelope, CancellationToken ct)
        where TQuery : IQuery<TResponse>;

    Task PublishManyAsync<TEvent>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TEvent : IEvent;
}
