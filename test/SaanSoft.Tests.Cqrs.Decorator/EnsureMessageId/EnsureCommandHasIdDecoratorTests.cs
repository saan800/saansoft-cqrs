using SaanSoft.Cqrs.GuidIds.Decorator.EnsureMessageId;

namespace SaanSoft.Tests.Cqrs.Decorator.EnsureMessageId;

public class EnsureCommandHasIdDecoratorTests : CommandBusDecoratorTestSetup
{
    protected override ICommandBusDecorator SutPublisherDecorator =>
        new EnsureCommandHasIdDecorator(IdGenerator, InMemoryCommandBus);

    public class ExecuteAsyncTests : EnsureCommandHasIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_id_if_default()
        {
            var id = Guid.Empty;
            var command = new MyCommand { Id = id };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Should_not_change_id_if_already_set()
        {
            var id = Guid.NewGuid();
            var command = new MyCommand { Id = id };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Id.Should().NotBe(Guid.Empty);
            command.Id.Should().Be(id);
        }
    }

    public class ExecuteAsyncWithResponseTests : EnsureCommandHasIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_id_if_default()
        {
            var id = Guid.Empty;
            var command = new MyCommandWithResponse { Id = id, Message = "hello" };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Should_not_change_id_if_already_set()
        {
            var id = Guid.NewGuid();
            var command = new MyCommandWithResponse { Id = id, Message = "hello" };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Id.Should().NotBe(Guid.Empty);
            command.Id.Should().Be(id);
        }
    }

    public class QueueAsyncTests : EnsureCommandHasIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_id_if_default()
        {
            var id = Guid.Empty;
            var command = new MyCommand { Id = id };
            await SutPublisherDecorator.QueueAsync(command);

            command.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public async Task Should_not_change_id_if_already_set()
        {
            var id = Guid.NewGuid();
            var command = new MyCommand { Id = id };
            await SutPublisherDecorator.QueueAsync(command);

            command.Id.Should().NotBe(Guid.Empty);
            command.Id.Should().Be(id);
        }
    }
}
