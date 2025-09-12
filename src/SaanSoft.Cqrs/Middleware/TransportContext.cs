using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Transport;

namespace SaanSoft.Cqrs.Middleware;

public sealed record TransportContext(
    MessageEnvelope Envelope,
    IExternalTransportOptions Options,
    IServiceRegistry ServiceRegistry
);
