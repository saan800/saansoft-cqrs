using SaanSoft.Cqrs.Core.Messages;
using SaanSoft.Cqrs.GuidIds.Utilities;

namespace SaanSoft.Tests.Cqrs.Common;

public abstract class TestSetup
{
    protected readonly ILogger Logger;
    protected readonly IIdGenerator IdGenerator;
    protected readonly ServiceCollection ServiceCollection;

    protected TestSetup()
    {
        Logger = A.Fake<ILogger>();
        IdGenerator = new GuidIdGenerator();

        ServiceCollection = new ServiceCollection();
        ServiceCollection.AddScoped<ILogger>(_ => Logger);
        ServiceCollection.AddScoped<IIdGenerator>(_ => IdGenerator);

        ServiceCollection.AddScoped<ICommandSubscriptionBus, InMemoryCommandBus>();
        ServiceCollection.AddScoped<IEventSubscriptionBus, InMemoryEventBus>();
        ServiceCollection.AddScoped<IQuerySubscriptionBus, InMemoryQueryBus>();

        ServiceCollection.AddScoped<IIdGenerator, GuidIdGenerator>();
    }

    private IServiceProvider? _serviceProvider = null;
    protected IServiceProvider GetServiceProvider()
        => _serviceProvider ??= ServiceCollection.BuildServiceProvider();

    protected InMemoryCommandBus InMemoryCommandBus => new(GetServiceProvider(), IdGenerator);

    protected InMemoryEventBus InMemoryEventBus => new(GetServiceProvider(), IdGenerator);

    protected InMemoryQueryBus InMemoryQueryBus => new(GetServiceProvider(), IdGenerator);

    protected void AddCommandHandlerException<TCommand>()
        where TCommand : IBaseCommand
    {
        var handler = A.Fake<ICommandHandler<TCommand>>();
        A.CallTo(() => handler.HandleAsync(A<TCommand>.Ignored, A<CancellationToken>.Ignored))
            .ThrowsAsync(new Exception("it went wrong"));
        ServiceCollection.AddScoped<ICommandHandler<TCommand>>(_ => handler);
    }

    protected void AddCommandHandlerException<TCommand, TResponse>()
        where TCommand : IBaseCommand<TCommand, TResponse>
    {
        var handler = A.Fake<ICommandHandler<TCommand, TResponse>>();
        A.CallTo(() => handler.HandleAsync(A<TCommand>.Ignored, A<CancellationToken>.Ignored))
            .ThrowsAsync(new Exception("it went wrong"));
        ServiceCollection.AddScoped<ICommandHandler<TCommand, TResponse>>(_ => handler);
    }

    protected void AddEventHandlerException<TEvent>()
        where TEvent : IBaseEvent
    {
        var handler = A.Fake<IEventHandler<TEvent>>();
        A.CallTo(() => handler.HandleAsync(A<TEvent>.Ignored, A<CancellationToken>.Ignored))
            .ThrowsAsync(new Exception("it went wrong"));
        ServiceCollection.AddScoped<IEventHandler<TEvent>>(_ => handler);
    }

    protected void AddQueryHandlerException<TQuery, TResponse>()
        where TQuery : IBaseQuery<TQuery, TResponse>
    {
        var handler = A.Fake<IQueryHandler<TQuery, TResponse>>();
        A.CallTo(() => handler.HandleAsync(A<TQuery>.Ignored, A<CancellationToken>.Ignored))
            .ThrowsAsync(new Exception("it went wrong"));
        ServiceCollection.AddScoped<IQueryHandler<TQuery, TResponse>>(_ => handler);
    }
}
