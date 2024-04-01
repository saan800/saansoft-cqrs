using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.Bus;
using SaanSoft.Cqrs.Handler;
using SaanSoft.Tests.Cqrs.TestHelpers;

namespace SaanSoft.Tests.Cqrs.Bus;

public class LocalQueryBusTests
{
    [Fact]
    public async Task QueryAsync_handler_exists_in_serviceProvider()
    {
        var handler = A.Fake<IQueryHandler<GuidQuery, QueryResult>>();
        A.CallTo(() => handler.HandleAsync(A<GuidQuery>.Ignored, A<CancellationToken>.Ignored))
            .Returns(new QueryResult());

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<IQueryHandler<GuidQuery, QueryResult>>(_ => handler);

        var sut = new LocalQueryBus(serviceCollection.BuildServiceProvider());
        var result = await sut.QueryAsync<GuidQuery, QueryResult>(new GuidQuery());
        result.IsSuccess.Should().BeTrue();

        A.CallTo(() => handler.HandleAsync(A<GuidQuery>.That.IsNotNull(), A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public async Task QueryAsync_no_handler_in_serviceProvider_should_throw_exception()
    {
        var serviceCollection = new ServiceCollection();

        var sut = new LocalQueryBus(serviceCollection.BuildServiceProvider());

        await sut.Invoking(y => y.QueryAsync<GuidQuery, QueryResult>(new GuidQuery()))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"No service for type '{typeof(IQueryHandler<GuidQuery, QueryResult>)}' has been registered.");
    }
}
