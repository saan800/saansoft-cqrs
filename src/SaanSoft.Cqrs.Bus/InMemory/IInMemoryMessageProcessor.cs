using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.Bus.InMemory;

public interface IInMemoryMessageProcessor
{
    Task HandleCommandEnvelopeAsync<TCommand>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand;

    Task<TResult> HandleCommandEnvelopeAsync<TCommand, TResult>(MessageEnvelope envelope, CancellationToken ct)
        where TCommand : ICommand<TResult>;

    Task<TResult> HandleQueryEnvelopeAsync<TQuery, TResult>(MessageEnvelope envelope, CancellationToken ct)
        where TQuery : IQuery<TResult>;

    Task HandleEventEnvelopesAsync<TEvent>(MessageEnvelope[] envelopes, CancellationToken ct)
        where TEvent : IEvent;
}
