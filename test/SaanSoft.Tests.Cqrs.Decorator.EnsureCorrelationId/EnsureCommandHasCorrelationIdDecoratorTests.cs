using SaanSoft.Cqrs.GuidIds.Bus;

namespace SaanSoft.Tests.Cqrs.Decorator.EnsureCorrelationId;

public class EnsureCommandHasCorrelationIdDecoratorTests : CommandBusTestSetup
{
    protected override ICommandBus SutPublisherDecorator =>
        new EnsureCommandHasCorrelationIdDecorator(InMemoryCommandBus);

    public class ExecuteAsyncTests : EnsureCommandHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_correlationId_if_not_set()
        {
            var command = new MyCommand();
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Metadata.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_correlationId_if_empty(string? emptyCorrelationId)
        {
            var command = new MyCommand { Metadata = { CorrelationId = emptyCorrelationId } };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Metadata.CorrelationId.Should().NotBeNullOrWhiteSpace();
            command.Metadata.CorrelationId.Should().NotBe(emptyCorrelationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_not_change_correlationId_if_already_set(string correlationId)
        {
            var command = new MyCommand { Metadata = { CorrelationId = correlationId } };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Metadata.CorrelationId.Should().Be(correlationId);
        }
    }

    public class ExecuteAsyncWithResponseTests : EnsureCommandHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_correlationId_if_not_set()
        {
            var command = new MyCommandWithResponse { Message = "hello" };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Metadata.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_correlationId_if_empty(string? emptyCorrelationId)
        {
            var command = new MyCommandWithResponse { Message = "hello", Metadata = { CorrelationId = emptyCorrelationId } };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Metadata.CorrelationId.Should().NotBeNullOrWhiteSpace();
            command.Metadata.CorrelationId.Should().NotBe(emptyCorrelationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_not_change_correlationId_if_already_set(string correlationId)
        {
            var command = new MyCommandWithResponse { Message = "hello", Metadata = { CorrelationId = correlationId } };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.Metadata.CorrelationId.Should().Be(correlationId);
        }
    }

    public class QueueAsyncTests : EnsureCommandHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_correlationId_if_not_set()
        {
            var command = new MyCommand();
            await SutPublisherDecorator.QueueAsync(command);

            command.Metadata.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_correlationId_if_empty(string? emptyCorrelationId)
        {
            var command = new MyCommand { Metadata = { CorrelationId = emptyCorrelationId } };
            await SutPublisherDecorator.QueueAsync(command);

            command.Metadata.CorrelationId.Should().NotBeNullOrWhiteSpace();
            command.Metadata.CorrelationId.Should().NotBe(emptyCorrelationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_not_change_correlationId_if_already_set(string correlationId)
        {
            var command = new MyCommand { Metadata = { CorrelationId = correlationId } };
            await SutPublisherDecorator.QueueAsync(command);

            command.Metadata.CorrelationId.Should().Be(correlationId);
        }
    }
}
