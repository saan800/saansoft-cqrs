using SaanSoft.Cqrs.Common.Handlers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class InMemoryQueryBusTests : TestSetup
{
    public class Constructor : InMemoryQueryBusTests
    {
        [Fact]
        public void Cant_create_with_null_serviceProvider()
        {
            Action act = () => new InMemoryQueryBus(null, IdGenerator);

            act.Should()
                .Throw<ArgumentNullException>()
                .Where(x => x.ParamName == "serviceProvider");
        }

        [Fact]
        public void Can_not_create_with_null_IdGenerator()
        {
            Action act = () => new InMemoryQueryBus(GetServiceProvider(), null);

            act.Should()
                .Throw<ArgumentNullException>()
                .Where(x => x.ParamName == "idGenerator");
        }
    }

    public class FetchAsync : InMemoryQueryBusTests
    {
        [Theory]
        [InlineAutoData]
        public async Task FetchAsync_handler_exists_in_serviceProvider(string data)
        {
            var handler = A.Fake<IBaseQueryHandler<MyQuery, MyQueryResponse>>();
            A.CallTo(() => handler.HandleAsync(A<MyQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(new MyQueryResponse(data));

            ServiceCollection.AddScoped<IBaseQueryHandler<MyQuery, MyQueryResponse>>(_ => handler);

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
            var handler1 = A.Fake<IBaseQueryHandler<MyQuery, MyQueryResponse>>();
            var handler2 = A.Fake<IBaseQueryHandler<MyQuery, MyQueryResponse>>();

            ServiceCollection.AddScoped<IBaseQueryHandler<MyQuery, MyQueryResponse>>(_ => handler1);
            ServiceCollection.AddScoped<IBaseQueryHandler<MyQuery, MyQueryResponse>>(_ => handler2);

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
