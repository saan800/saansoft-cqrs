namespace SaanSoft.Tests.Cqrs.Decorator.EnsureCorrelationId;

public class EnsureQueryHasCorrelationIdDecoratorTests : QueryBusDecoratorTestSetup
{
    protected override IQueryBusDecorator SutPublisherDecorator =>
        new EnsureQueryHasCorrelationIdDecorator(InMemoryQueryBus);

    public class FetchAsyncTests : EnsureQueryHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_correlationId_if_not_set()
        {
            var query = new MyQuery { Message = "hello" };
            await SutPublisherDecorator.FetchAsync(query);

            query.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_correlationId_if_empty(string? emptyCorrelationId)
        {
            var query = new MyQuery { Message = "hello", CorrelationId = emptyCorrelationId };
            await SutPublisherDecorator.FetchAsync(query);

            query.CorrelationId.Should().NotBeNullOrWhiteSpace();
            query.CorrelationId.Should().NotBe(emptyCorrelationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_not_change_correlationId_if_already_set(string correlationId)
        {
            var query = new MyQuery { Message = "hello", CorrelationId = correlationId };
            await SutPublisherDecorator.FetchAsync(query);

            query.CorrelationId.Should().Be(correlationId);
        }
    }
}
