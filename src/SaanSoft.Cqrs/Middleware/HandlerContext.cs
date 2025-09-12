using SaanSoft.Cqrs.DependencyInjection;

namespace SaanSoft.Cqrs.Middleware;

public sealed record HandlerContext(
    MessageEnvelope Envelope,
    Type HandlerType,
    IServiceRegistry ServiceRegistry
);
