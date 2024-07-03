using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.Common.TestHandlers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryCommandBusTests : TestSetup
{
    public class Constructor : InMemoryCommandBusTests
    {
        [Fact]
        public void Can_not_create_with_null_serviceProvider()
        {
            Action act = () => new InMemoryCommandBus(null, IdGenerator, Logger);

            act.Should()
                .Throw<ArgumentNullException>()
                .Where(x => x.ParamName == "serviceProvider");
        }

        [Fact]
        public void Can_not_create_with_null_IdGenerator()
        {
            Action act = () => new InMemoryCommandBus(GetServiceProvider(), null, Logger);

            act.Should()
                .Throw<ArgumentNullException>()
                .Where(x => x.ParamName == "idGenerator");
        }

        [Fact]
        public void Can_not_create_with_null_logger()
        {
            Action act = () => new InMemoryCommandBus(GetServiceProvider(), IdGenerator, null);

            act.Should()
                .Throw<ArgumentNullException>()
                .Where(x => x.ParamName == "logger");
        }
    }

    public class ExecuteAsync : InMemoryCommandBusTests
    {
        [Fact]
        public async Task ExecuteAsync_handler_exists_in_serviceProvider()
        {
            var handler = A.Fake<ICommandHandler<MyCommand>>();

            ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler);

            await InMemoryCommandBus.ExecuteAsync(new MyCommand());

            A.CallTo(() => handler.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task ExecuteAsync_no_handler_in_serviceProvider_should_throw_error()
        {
            await InMemoryCommandBus.Invoking(y => y.ExecuteAsync(new MyCommand()))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x =>
                    x.Message.StartsWith("No handler for type") &&
                    x.Message.EndsWith("has been registered.")
                );
        }

        [Fact]
        public async Task ExecuteAsync_multiple_handlers_exists_in_serviceProvider_should_throw_error()
        {
            var handler1 = A.Fake<ICommandHandler<MyCommand>>();
            var handler2 = A.Fake<ICommandHandler<MyCommand>>();

            ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler1);
            ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler2);

            await InMemoryCommandBus.Invoking(y => y.ExecuteAsync(new MyCommand()))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x =>
                    x.Message.StartsWith("Only one handler for type") &&
                    x.Message.Contains("can be registered")
                );

            A.CallTo(() => handler1.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => handler2.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }
    }

    public class ExecuteAsyncWithResponse : InMemoryCommandBusTests
    {
        [Theory]
        [InlineAutoData]
        public async Task ExecuteAsync_handler_exists_in_serviceProvider(string response)
        {
            ServiceCollection.AddScoped<ICommandHandler<MyCommandWithResponse, string>, CommandHandler>();

            var result = await InMemoryCommandBus.ExecuteAsync(new MyCommandWithResponse { Message = response });
            result.Should().Be(response);
        }

        [Theory]
        [InlineAutoData]
        public async Task ExecuteAsync_no_handler_in_serviceProvider_should_throw_error(string response)
        {
            await InMemoryCommandBus.Invoking(y => y.ExecuteAsync(new MyCommandWithResponse { Message = response }))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x =>
                    x.Message.StartsWith("No handler for type") &&
                    x.Message.EndsWith("has been registered.")
                );
        }

        [Theory]
        [InlineAutoData]
        public async Task ExecuteAsync_multiple_handlers_exists_in_serviceProvider_should_throw_error(string response)
        {
            var handler1 = A.Fake<ICommandHandler<MyCommandWithResponse, string>>();
            var handler2 = A.Fake<ICommandHandler<MyCommandWithResponse, string>>();

            ServiceCollection.AddScoped<ICommandHandler<MyCommandWithResponse, string>>(_ => handler1);
            ServiceCollection.AddScoped<ICommandHandler<MyCommandWithResponse, string>>(_ => handler2);

            await InMemoryCommandBus.Invoking(y => y.ExecuteAsync(new MyCommandWithResponse { Message = response }))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x =>
                    x.Message.StartsWith("Only one handler for type") &&
                    x.Message.Contains("can be registered")
                );

            A.CallTo(() => handler1.HandleAsync(A<MyCommandWithResponse>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => handler2.HandleAsync(A<MyCommandWithResponse>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }
    }

    public class QueueAsync : InMemoryCommandBusTests
    {
        [Fact]
        public async Task QueueAsync_handler_exists_in_serviceProvider()
        {
            var handler = A.Fake<ICommandHandler<MyCommand>>();

            ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler);

            await InMemoryCommandBus.QueueAsync(new MyCommand());

            A.CallTo(() => handler.HandleAsync(A<MyCommand>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public async Task QueueAsync_no_handler_in_serviceProvider_should_throw_error()
        {
            await InMemoryCommandBus.Invoking(y => y.RunAsync(new NoHandlerCommand()))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x => x.Message.Contains("No handler for type"));
        }

        [Fact]
        public async Task QueueAsync_multiple_handlers_in_serviceProvider_should_throw_error()
        {
            var handler1 = A.Fake<ICommandHandler<MyCommand>>();
            var handler2 = A.Fake<ICommandHandler<MyCommand>>();

            ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler1);
            ServiceCollection.AddScoped<ICommandHandler<MyCommand>>(_ => handler2);

            await InMemoryCommandBus.Invoking(y => y.QueueAsync(new MyCommand()))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x => x.Message.StartsWith("Only one handler for type"));

            A.CallTo(() => handler1.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => handler2.HandleAsync(A<MyCommand>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }
    }
}
