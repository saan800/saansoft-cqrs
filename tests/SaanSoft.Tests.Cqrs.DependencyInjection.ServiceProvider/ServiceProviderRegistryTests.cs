using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.DependencyInjection.ServiceProvider;
using SaanSoft.Cqrs.Handlers;

namespace SaanSoft.Tests.Cqrs.DependencyInjection.ServiceProvider;

public class ServiceProviderRegistryTests
{
    public class GetMessageHandlerCount
    {
        [Fact]
        public void ShouldReturnOne_WhenHandlerIsRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<IHandleMessage<MyCommand>, CommandHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.GetMessageHandlerCount<MyCommand>();

            result.Should().Be(1);
        }

        [Fact]
        public void ShouldReturnZero_WhenNoHandlerRegistered()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            var result = registry.GetMessageHandlerCount<MyCommand>();

            result.Should().Be(0);
        }

        [Fact]
        public void ShouldReturnNumber_WhenMultipleHandlersRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<IHandleMessage<MyEvent>, EventsHandler>();
            services.AddTransient<IHandleMessage<MyEvent>, EventsHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.GetMessageHandlerCount<MyEvent>();

            result.Should().Be(2);
        }
    }

    public class GetMessageHandlerWithResponseCount
    {
        [Fact]
        public void ShouldReturnOne_WhenHandlerIsRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<IHandleMessage<MyCommandWithResponse, string?>, CommandHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.GetMessageHandlerWithResponseCount<MyCommandWithResponse>();
            result.Should().Be(1);
        }

        [Fact]
        public void ShouldReturnZero_WhenNoHandlerRegistered()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            var result = registry.GetMessageHandlerWithResponseCount<MyCommandWithResponse>();

            result.Should().Be(0);
        }

        [Fact]
        public void ShouldReturnCount_WhenMultipleHandlersRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<IHandleMessage<MyCommandWithResponse, string?>, CommandHandler>();
            services.AddTransient<IHandleMessage<MyCommandWithResponse, string?>, CommandHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.GetMessageHandlerWithResponseCount<MyCommandWithResponse>();

            result.Should().Be(2);
        }
    }

    public class ResolveServiceMethods
    {
        [Fact]
        public void ResolveService_ShouldReturnNull_WhenNotRegistered()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            var result = registry.ResolveService<string>();

            result.Should().BeNull();
        }

        [Fact]
        public void ResolveRequiredService_ShouldReturnService_WhenRegistered()
        {
            var services = new ServiceCollection();
            services.AddSingleton("hello");
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.ResolveRequiredService<string>();

            result.Should().Be("hello");
        }


        [Fact]
        public void ResolveRequiredService_ShouldThrow_WhenNotRegistered()
        {
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            Action act = () => registry.ResolveRequiredService<string>();

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ResolveServices_ShouldReturnMultipleServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton("a");
            services.AddSingleton("b");
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.ResolveServices<string>();

            result.Should().BeEquivalentTo(["a", "b"]);
        }

        [Fact]
        public void ResolveServices_ShouldReturnEmptyListIfNoneRegistered()
        {
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.ResolveServices<string>();

            result.Should().BeEmpty();
        }
    }
}
