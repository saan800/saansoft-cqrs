using SaanSoft.Cqrs.GuidIds.Decorator.EnsureMessageId;

namespace SaanSoft.Tests.Cqrs.Decorator.EnsureMessageId;

public class EnsureEventHasIdDecoratorTests : EventBusTestSetup
{
    protected override IEventBus SutPublisherDecorator =>
        new EnsureEventHasIdDecorator(IdGenerator, InMemoryEventBus);

    public class QueueAsyncTests : EnsureEventHasIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_id_if_default()
        {
            var id = Guid.Empty;
            var evt = new MyEvent(Guid.NewGuid()) { Id = id };
            await SutPublisherDecorator.QueueAsync(evt);

            evt.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Should_not_change_id_if_already_set()
        {
            var id = Guid.NewGuid();
            var evt = new MyEvent(Guid.NewGuid()) { Id = id };
            await SutPublisherDecorator.QueueAsync(evt);

            evt.Id.Should().NotBe(Guid.Empty);
            evt.Id.Should().Be(id);
        }
    }

    public class QueueManyAsyncTests : EnsureEventHasIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_ids_if_default()
        {
            var id = Guid.Empty;
            var evt1 = new MyEvent(Guid.NewGuid()) { Id = id };
            var evt2 = new MyEvent(Guid.NewGuid()) { Id = id };
            await SutPublisherDecorator.QueueManyAsync([evt1, evt2]);

            evt1.Id.Should().NotBe(Guid.Empty);
            evt2.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Should_not_change_id_if_already_set()
        {
            var id = Guid.NewGuid();
            var evt1 = new MyEvent(Guid.NewGuid()) { Id = id };
            var evt2 = new MyEvent(Guid.NewGuid()) { Id = id };
            await SutPublisherDecorator.QueueManyAsync([evt1, evt2]);

            evt1.Id.Should().NotBe(Guid.Empty);
            evt1.Id.Should().Be(id);

            evt2.Id.Should().NotBe(Guid.Empty);
            evt2.Id.Should().Be(id);
        }
    }
}
