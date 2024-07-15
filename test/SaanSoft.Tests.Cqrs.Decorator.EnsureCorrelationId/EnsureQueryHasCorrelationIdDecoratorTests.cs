using SaanSoft.Cqrs.GuidIds.Bus;

namespace SaanSoft.Tests.Cqrs.Decorator.EnsureCorrelationId;

public class EnsureQueryHasCorrelationIdDecoratorTests : QueryBusTestSetup
{
    protected override IQueryBus SutPublisherDecorator =>
        new EnsureQueryHasCorrelationIdDecorator(InMemoryQueryBus);

    public class FetchAsyncTests : EnsureQueryHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_correlationId_if_not_set()
        {
            var query = new MyQuery { Message = "hello" };
            await SutPublisherDecorator.FetchAsync(query);

            query.Metadata.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_correlationId_if_empty(string? emptyCorrelationId)
        {
            var query = new MyQuery { Message = "hello", Metadata = { CorrelationId = emptyCorrelationId } };
            await SutPublisherDecorator.FetchAsync(query);

            query.Metadata.CorrelationId.Should().NotBeNullOrWhiteSpace();
            query.Metadata.CorrelationId.Should().NotBe(emptyCorrelationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_not_change_correlationId_if_already_set(string correlationId)
        {
            var query = new MyQuery { Message = "hello", Metadata = { CorrelationId = correlationId } };
            await SutPublisherDecorator.FetchAsync(query);

            query.Metadata.CorrelationId.Should().Be(correlationId);
        }
    }
}
