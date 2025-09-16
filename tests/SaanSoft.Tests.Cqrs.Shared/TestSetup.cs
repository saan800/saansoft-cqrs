using SaanSoft.Cqrs.Handlers;

namespace SaanSoft.Tests.Cqrs.Shared;

// TODO: this?

// public abstract class TestSetup
// {
//     protected readonly ILogger Logger;
//     protected readonly ServiceCollection ServiceCollection;

//     protected TestSetup()
//     {
//         Logger = A.Fake<ILogger>();

//         ServiceCollection = new ServiceCollection();
//         ServiceCollection.AddScoped<ILogger>(_ => Logger);

//         ServiceCollection.AddScoped<ICommandSubscriptionBus, InMemoryCommandBus>();
//         ServiceCollection.AddScoped<IEventSubscriptionBus, InMemoryEventBus>();
//         ServiceCollection.AddScoped<IQuerySubscriptionBus, InMemoryQueryBus>();
//     }

//     private IServiceProvider? _serviceProvider = null;
//     protected IServiceProvider GetServiceProvider()
//         => _serviceProvider ??= ServiceCollection.BuildServiceProvider();

//     protected InMemoryCommandBus InMemoryCommandBus => new(GetServiceProvider());

//     protected InMemoryEventBus InMemoryEventBus => new(GetServiceProvider());

//     protected InMemoryQueryBus InMemoryQueryBus => new(GetServiceProvider());

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
// }
