using SaanSoft.Cqrs.DependencyInjection;

namespace SaanSoft.Cqrs.Middleware;

public sealed record PublishContext(
    MessageEnvelope Envelope,
    IServiceRegistry ServiceRegistry
);
