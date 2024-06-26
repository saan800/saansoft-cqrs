namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryQueryBusTests : TestSetup
{
    public class Constructor : InMemoryQueryBusTests
    {
        [Fact]
        public void Cant_create_with_null_serviceProvider()
        {
            Action act = () => new InMemoryQueryBus(null, IdGenerator, Logger);

            act.Should()
                .Throw<ArgumentNullException>()
                .Where(x => x.ParamName == "serviceProvider");
        }

        [Fact]
        public void Can_not_create_with_null_IdGenerator()
        {
            Action act = () => new InMemoryQueryBus(GetServiceProvider(), null, Logger);

            act.Should()
                .Throw<ArgumentNullException>()
                .Where(x => x.ParamName == "idGenerator");
        }

        [Fact]
        public void Cant_create_with_null_logger()
        {
            Action act = () => new InMemoryQueryBus(GetServiceProvider(), IdGenerator, null);

            act.Should()
                .Throw<ArgumentNullException>()
                .Where(x => x.ParamName == "logger");
        }
    }


    public class FetchAsync : InMemoryQueryBusTests
    {
        [Theory]
        [InlineAutoData]
        public async Task FetchAsync_handler_exists_in_serviceProvider(string data)
        {
            var handler = A.Fake<IQueryHandler<MyQuery, MyQueryResponse>>();
            A.CallTo(() => handler.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(new MyQueryResponse(data));

            ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>>(_ => handler);

            var result = await InMemoryQueryBus.FetchAsync(new MyQuery());
            result.Should().NotBeNull();
            result.Message.Should().Be(data);

            A.CallTo(() => handler.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public async Task FetchAsync_no_handler_in_serviceProvider_should_throw_error()
        {
            await InMemoryQueryBus.Invoking(y => y.FetchAsync(new MyQuery()))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x =>
                    x.Message.StartsWith("No handler for type") &&
                    x.Message.EndsWith("has been registered.")
                );
        }

        [Fact]
        public async Task FetchAsync_multiple_handlers_exists_in_serviceProvider_should_throw_error()
        {
            var handler1 = A.Fake<IQueryHandler<MyQuery, MyQueryResponse>>();
            var handler2 = A.Fake<IQueryHandler<MyQuery, MyQueryResponse>>();

            ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>>(_ => handler1);
            ServiceCollection.AddScoped<IQueryHandler<MyQuery, MyQueryResponse>>(_ => handler2);

            await InMemoryQueryBus.Invoking(y => y.FetchAsync(new MyQuery()))
                .Should().ThrowAsync<InvalidOperationException>()
                .Where(x =>
                    x.Message.StartsWith("Only one handler for type") &&
                    x.Message.Contains("can be registered")
                );

            A.CallTo(() => handler1.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => handler2.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored)).MustNotHaveHappened();
        }
    }
}
