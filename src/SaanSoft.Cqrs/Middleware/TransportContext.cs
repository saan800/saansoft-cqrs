using SaanSoft.Cqrs.DependencyInjection;

namespace SaanSoft.Cqrs.Middleware;

public sealed record TransportContext(
    MessageEnvelope Envelope,
    IServiceRegistry ServiceRegistry
);
