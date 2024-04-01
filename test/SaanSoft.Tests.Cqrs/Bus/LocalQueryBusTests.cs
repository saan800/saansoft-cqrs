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
        var serviceCollection = new ServiceCollection();
        // TODO: add all from assembly
        serviceCollection.AddScoped<IQueryHandler<GuidQuery, QueryResult>, GuidQueryHandler>();

        var sut = new LocalQueryBus(serviceCollection.BuildServiceProvider());
        var result = await sut.QueryAsync<GuidQuery, QueryResult>(new GuidQuery());
        result.IsSuccess.Should().BeTrue();
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
