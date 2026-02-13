using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Middleware;
using SaanSoft.Cqrs.Transport;

namespace SaanSoft.Cqrs.DependencyInjection.ServiceProvider;

public class WithExternalMessageProviderOptions
{
    /// <summary>
    /// Middleware to run on messages just before they are sent to IExternalMessageProvider.
    /// These middleware run after <see cref="AddCqrsOptions.PublisherMiddlewares"/>
    /// </summary>
    /// <remarks>
    /// Consider if the PublisherMiddlewares needs pairing with SubscriberMiddlewares.
    /// eg If you have a large message body and serialise it to blob storage when publishing a message,
    /// then you would also need something to read the message body from blob storage and reconstruct
    /// the message when you subscribe to it, and before its handled.
    /// </remarks>
    public IReadOnlyCollection<IMiddleware> PublisherMiddlewares { get; set; } = [];

    /// <summary>
    /// Middleware to run on messages when they are fetched from the external message provider subscription
    /// These middleware run just before <see cref="AddCqrsOptions.HandlerMiddlewares"/>
    /// </summary>
    /// <remarks>
    /// Consider if the PublisherMiddlewares needs pairing with SubscriberMiddlewares.
    /// eg If you have a large message body and serialise it to blob storage when publishing a message,
    /// then you would also need something to read the message body from blob storage and reconstruct
    /// the message when you subscribe to it, and before its handled.
    /// </remarks>
    public IReadOnlyCollection<IMiddleware> SubscriberMiddlewares { get; set; } = [];

    /// <summary>
    /// Options used to configure the ExternalMessageRouter for sending messages to the IExternalMessageProvider.
    /// Can configure the options here, or provide using WithExternalMessageRouterOptions&lt;TOptions&gt;
    /// extension. Here takes precedence if both are configured.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="DefaultExternalMessageRouterOptions"/> if not configured in any way
    /// </remarks>
    public IDefaultExternalMessageRouterOptions? DefaultExternalMessageRouterOptions { get; set; }

    public Lifetimes ServiceLifetimes { get; set; } = new Lifetimes();

    public class Lifetimes
    {
        /// <summary>
        /// Service lifetime of IExternalMessageProvider
        /// Defaults to Scoped
        /// </summary>
        public ServiceLifetime ExternalMessageProvider { get; set; } = ServiceLifetime.Scoped;
        /// <summary>
        /// Service lifetime of ExternalMessageRouter
        /// Defaults to Scoped
        /// </summary>
        public ServiceLifetime ExternalMessageRouter { get; set; } = ServiceLifetime.Scoped;
    }
}
