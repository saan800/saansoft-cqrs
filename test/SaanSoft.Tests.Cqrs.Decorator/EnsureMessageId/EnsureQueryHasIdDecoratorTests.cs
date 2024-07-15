using SaanSoft.Cqrs.GuidIds.Decorator.EnsureMessageId;

namespace SaanSoft.Tests.Cqrs.Decorator.EnsureMessageId;

public class EnsureQueryHasIdDecoratorTests : QueryBusTestSetup
{
    protected override IQueryBus SutPublisherDecorator =>
        new EnsureQueryHasIdDecorator(IdGenerator, InMemoryQueryBus);

    public class FetchAsyncTests : EnsureQueryHasIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_id_if_default()
        {
            var id = Guid.Empty;
            var query = new MyQuery { Id = id, Message = "hello" };
            await SutPublisherDecorator.FetchAsync(query);

            query.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Should_not_change_id_if_already_set()
        {
            var id = Guid.NewGuid();
            var query = new MyQuery { Id = id, Message = "hello" };
            await SutPublisherDecorator.FetchAsync(query);

            query.Id.Should().NotBe(Guid.Empty);
            query.Id.Should().Be(id);
        }
    }
}
