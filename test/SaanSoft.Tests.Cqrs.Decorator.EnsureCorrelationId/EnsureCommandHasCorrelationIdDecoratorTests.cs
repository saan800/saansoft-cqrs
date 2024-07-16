namespace SaanSoft.Tests.Cqrs.Decorator.EnsureCorrelationId;

public class EnsureCommandHasCorrelationIdDecoratorTests : CommandBusDecoratorTestSetup
{
    protected override ICommandBusDecorator SutPublisherDecorator =>
        new EnsureCommandHasCorrelationIdDecorator(InMemoryCommandBus);

    public class ExecuteAsyncTests : EnsureCommandHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_correlationId_if_not_set()
        {
            var command = new MyCommand();
            await SutPublisherDecorator.ExecuteAsync(command);

            command.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_correlationId_if_empty(string? emptyCorrelationId)
        {
            var command = new MyCommand { CorrelationId = emptyCorrelationId };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.CorrelationId.Should().NotBeNullOrWhiteSpace();
            command.CorrelationId.Should().NotBe(emptyCorrelationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_not_change_correlationId_if_already_set(string correlationId)
        {
            var command = new MyCommand { CorrelationId = correlationId };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.CorrelationId.Should().Be(correlationId);
        }
    }

    public class ExecuteAsyncWithResponseTests : EnsureCommandHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_correlationId_if_not_set()
        {
            var command = new MyCommandWithResponse { Message = "hello" };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_correlationId_if_empty(string? emptyCorrelationId)
        {
            var command = new MyCommandWithResponse { Message = "hello", CorrelationId = emptyCorrelationId };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.CorrelationId.Should().NotBeNullOrWhiteSpace();
            command.CorrelationId.Should().NotBe(emptyCorrelationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_not_change_correlationId_if_already_set(string correlationId)
        {
            var command = new MyCommandWithResponse { Message = "hello", CorrelationId = correlationId };
            await SutPublisherDecorator.ExecuteAsync(command);

            command.CorrelationId.Should().Be(correlationId);
        }
    }

    public class QueueAsyncTests : EnsureCommandHasCorrelationIdDecoratorTests
    {
        [Fact]
        public async Task Should_generate_correlationId_if_not_set()
        {
            var command = new MyCommand();
            await SutPublisherDecorator.QueueAsync(command);

            command.CorrelationId.Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task Should_generate_correlationId_if_empty(string? emptyCorrelationId)
        {
            var command = new MyCommand { CorrelationId = emptyCorrelationId };
            await SutPublisherDecorator.QueueAsync(command);

            command.CorrelationId.Should().NotBeNullOrWhiteSpace();
            command.CorrelationId.Should().NotBe(emptyCorrelationId);
        }

        [Theory]
        [AutoFakeData]
        public async Task Should_not_change_correlationId_if_already_set(string correlationId)
        {
            var command = new MyCommand { CorrelationId = correlationId };
            await SutPublisherDecorator.QueueAsync(command);

            command.CorrelationId.Should().Be(correlationId);
        }
    }
}
