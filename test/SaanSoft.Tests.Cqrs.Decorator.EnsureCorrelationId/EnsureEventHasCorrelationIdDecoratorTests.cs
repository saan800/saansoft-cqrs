namespace SaanSoft.Tests.Cqrs.Decorator.EnsureCorrelationId;

public class EnsureEventHasCorrelationIdDecoratorTests : EventBusDecoratorTestSetup
{
    protected override IEventBus SutPublisherDecorator =>
        new EnsureEventHasCorrelationIdDecorator([], InMemoryEventBus);

    public class QueueAsyncTests : EnsureEventHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_correlationId_if_not_set()
        {
            var evt = new MyEvent(Guid.NewGuid());
            await SutPublisherDecorator.QueueAsync(evt);

            evt.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_correlationId_if_empty(string? emptyCorrelationId)
        {
            var evt = new MyEvent(Guid.NewGuid()) { CorrelationId = emptyCorrelationId };
            await SutPublisherDecorator.QueueAsync(evt);

            evt.CorrelationId.Should().NotBeNullOrWhiteSpace();
            evt.CorrelationId.Should().NotBe(emptyCorrelationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_not_change_correlationId_if_already_set(string correlationId)
        {
            var evt = new MyEvent(Guid.NewGuid()) { CorrelationId = correlationId };
            await SutPublisherDecorator.QueueAsync(evt);

            evt.CorrelationId.Should().Be(correlationId);
        }
    }

    public class QueueManyAsyncTests : EnsureEventHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_same_correlationId_if_not_set()
        {
            var evt1 = new MyEvent(Guid.NewGuid());
            var evt2 = new MyEvent(Guid.NewGuid());
            var evt3 = new MyEvent(Guid.NewGuid());
            await SutPublisherDecorator.QueueManyAsync([evt1, evt2, evt3]);

            evt1.CorrelationId.Should().NotBeNullOrWhiteSpace();
            var correlationIdResult = evt1.CorrelationId;
            evt2.CorrelationId.Should().Be(correlationIdResult);
            evt3.CorrelationId.Should().Be(correlationIdResult);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_same_correlationId_if_empty(string? emptyCorrelationId)
        {
            var evt1 = new MyEvent(Guid.NewGuid()) { CorrelationId = emptyCorrelationId };
            var evt2 = new MyEvent(Guid.NewGuid()) { CorrelationId = emptyCorrelationId };
            var evt3 = new MyEvent(Guid.NewGuid()) { CorrelationId = emptyCorrelationId };
            await SutPublisherDecorator.QueueManyAsync([evt1, evt2, evt3]);

            evt1.CorrelationId.Should().NotBeNullOrWhiteSpace();
            evt1.CorrelationId.Should().NotBe(emptyCorrelationId);

            var correlationIdResult = evt1.CorrelationId;
            evt2.CorrelationId.Should().Be(correlationIdResult);
            evt3.CorrelationId.Should().Be(correlationIdResult);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_use_existing_correlationId_for_all_events_if_exactly_one_distinct_already_set(string correlationId)
        {
            var evt1 = new MyEvent(Guid.NewGuid()) { CorrelationId = correlationId };
            var evt2 = new MyEvent(Guid.NewGuid()) { CorrelationId = correlationId };
            var evt3 = new MyEvent(Guid.NewGuid());
            var evt4 = new MyEvent(Guid.NewGuid());
            await SutPublisherDecorator.QueueManyAsync([evt1, evt2, evt3, evt4]);

            evt1.CorrelationId.Should().Be(correlationId);
            evt2.CorrelationId.Should().Be(correlationId);
            evt3.CorrelationId.Should().Be(correlationId);
            evt4.CorrelationId.Should().Be(correlationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_use_new_correlationId_for_unset_if_more_than_one_distinct_already_set(string correlationId1, string correlationId2)
        {
            var evt1 = new MyEvent(Guid.NewGuid()) { CorrelationId = correlationId1 };
            var evt2 = new MyEvent(Guid.NewGuid()) { CorrelationId = correlationId2 };
            var evt3 = new MyEvent(Guid.NewGuid());
            var evt4 = new MyEvent(Guid.NewGuid());
            await SutPublisherDecorator.QueueManyAsync([evt1, evt2, evt3, evt4]);

            // 1 & 2 were set, but to different values, should keep existing correlationIds
            evt1.CorrelationId.Should().Be(correlationId1);
            evt2.CorrelationId.Should().Be(correlationId2);

            // 3 and 4 weren't set, so they should have the same correlationId, but different to 1 & 2
            evt3.CorrelationId.Should().NotBeNullOrWhiteSpace();
            evt3.CorrelationId.Should().NotBe(correlationId1);
            evt3.CorrelationId.Should().NotBe(correlationId2);

            var correlationIdResult = evt3.CorrelationId;
            evt4.CorrelationId.Should().Be(correlationIdResult);
        }
    }
}
