using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaanSoft.Cqrs.Handlers;

namespace SaanSoft.Tests.Cqrs.Shared;

// TODO: this?

public class TestSetup
{
    protected readonly ILogger Logger;
    protected readonly ServiceCollection ServiceCollection;

    protected TestSetup()
    {
        Logger = A.Fake<ILogger>();
        var temp = A.Fake<ILoggerFactory>();

        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddScoped<ILogger>(_ => Logger);

        // TODO: extensions for finding and registering command, event and query handlers in assembly

        //         ServiceCollection.AddScoped<ICommandSubscriptionBus, LocalCommandBus>();
        //         ServiceCollection.AddScoped<IEventSubscriptionBus, LocalEventBus>();
        //         ServiceCollection.AddScoped<IQuerySubscriptionBus, LocalQueryBus>();
        //     }

        //     private IServiceProvider? _serviceProvider = null;
        //     protected IServiceProvider GetServiceProvider()
        //         => _serviceProvider ??= ServiceCollection.BuildServiceProvider();

        //     protected LocalCommandBus LocalCommandBus => new(GetServiceProvider());

        //     protected LocalEventBus LocalEventBus => new(GetServiceProvider());

        //     protected LocalQueryBus LocalQueryBus => new(GetServiceProvider());

        //     protected void AddCommandHandlerException<TCommand>()
        //         where TCommand : ICommand
        //     {
        //         var handler = A.Fake<ICommandHandler<TCommand>>();
        //         A.CallTo(() => handler.HandleAsync(A<TCommand>.Ignored, A<CancellationToken>.Ignored))
        //             .ThrowsAsync(new Exception("it went wrong"));
        //         ServiceCollection.AddScoped<ICommandHandler<TCommand>>(_ => handler);
        //     }

        //     protected void AddCommandHandlerException<TCommand, TResponse>()
        //         where TCommand : ICommand<TCommand, TResponse>
        //     {
        //         var handler = A.Fake<ICommandHandler<TCommand, TResponse>>();
        //         A.CallTo(() => handler.HandleAsync(A<TCommand>.Ignored, A<CancellationToken>.Ignored))
        //             .ThrowsAsync(new Exception("it went wrong"));
        //         ServiceCollection.AddScoped<ICommandHandler<TCommand, TResponse>>(_ => handler);
        //     }

        //     protected void AddEventHandlerException<TEvent>()
        //         where TEvent : IEvent
        //     {
        //         var handler = A.Fake<IEventHandler<TEvent>>();
        //         A.CallTo(() => handler.HandleAsync(A<TEvent>.Ignored, A<CancellationToken>.Ignored))
        //             .ThrowsAsync(new Exception("it went wrong"));
        //         ServiceCollection.AddScoped<IEventHandler<TEvent>>(_ => handler);
        //     }

        //     protected void AddQueryHandlerException<TQuery, TResponse>()
        //         where TQuery : IQuery<TQuery, TResponse>
        //     {
        //         var handler = A.Fake<IQueryHandler<TQuery, TResponse>>();
        //         A.CallTo(() => handler.HandleAsync(A<TQuery>.Ignored, A<CancellationToken>.Ignored))
        //             .ThrowsAsync(new Exception("it went wrong"));
        //         ServiceCollection.AddScoped<IQueryHandler<TQuery, TResponse>>(_ => handler);
        //     }
    }
}
