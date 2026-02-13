using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Middleware;

namespace SaanSoft.Cqrs.DependencyInjection.ServiceProvider;

public sealed class AddCqrsOptions
{
    /// <summary>
    /// Middleware to run on messages when they are newly raised with IMessageBus
    /// </summary>
    public IReadOnlyCollection<IMiddleware> PublisherMiddlewares { get; set; } = [];

    /// <summary>
    /// Middleware to run on messages just before the message is handled
    /// </summary>
    public IReadOnlyCollection<IMiddleware> HandlerMiddlewares { get; set; } = [];

    public Lifetimes ServiceLifetimes { get; set; } = new Lifetimes();

    public class Lifetimes
    {
        /// <summary>
        /// Service lifetime of IMessageBus
        /// Defaults to Scoped
        /// </summary>
        public ServiceLifetime MesageBus { get; set; } = ServiceLifetime.Scoped;

        /// <summary>
        /// Service lifetime of LocalMessageRouter
        /// Defaults to Scoped
        /// </summary>
        public ServiceLifetime LocalMessageRouter { get; set; } = ServiceLifetime.Scoped;

        /// <summary>
        /// Service lifetime of IServiceRegistry
        /// Defaults to Scoped
        /// </summary>
        public ServiceLifetime ServiceRegistry { get; set; } = ServiceLifetime.Scoped;

        /// <summary>
        /// Service lifetime of RoutingStrategy
        /// Defaults to Scoped
        /// </summary>
        public ServiceLifetime RoutingStrategy { get; set; } = ServiceLifetime.Scoped;
    }
}
