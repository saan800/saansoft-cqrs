using Microsoft.Extensions.DependencyInjection;
using SaanSoft.Cqrs.DependencyInjection.ServiceProvider;
using SaanSoft.Cqrs.Handlers;

namespace SaanSoft.Tests.Cqrs.DependencyInjection.ServiceProvider;

public class ServiceProviderRegistryTests
{
    public class HasCommandHandler
    {
        [Fact]
        public void ShouldReturnTrue_WhenHandlerIsRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<ICommandHandler<MyCommand>, CommandHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.HasCommandHandler<MyCommand>();

            result.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalse_WhenNoHandlerRegistered()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            var result = registry.HasCommandHandler<MyCommand>();

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldThrow_WhenTypeIsNotCommand()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            Action act = () => registry.HasCommandHandler<MyEvent>(); // MyEvent is not a command

            act.Should().Throw<ApplicationException>();
        }

        [Fact]
        public void ShouldThrow_WhenMultipleHandlersRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<ICommandHandler<MyCommand>, CommandHandler>();
            services.AddTransient<ICommandHandler<MyCommand>, CommandHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            Action act = () => registry.HasCommandHandler<MyCommand>();

            act.Should().Throw<ApplicationException>()
                .WithMessage("*Multiple handlers*");
        }
    }

    public class HasCommandWithResponseHandler
    {
        [Fact]
        public void ShouldReturnTrue_WhenHandlerIsRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<ICommandHandler<MyCommandWithResponse, string?>, CommandHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.HasCommandWithResponseHandler<MyCommandWithResponse>();

            result.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalse_WhenNoHandlerRegistered()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            var result = registry.HasCommandWithResponseHandler<MyCommandWithResponse>();

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldThrow_WhenTypeIsNotCommand()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            Action act = () => registry.HasCommandWithResponseHandler<MyEvent>(); // MyEvent is not a command

            act.Should().Throw<ApplicationException>();
        }

        [Fact]
        public void ShouldThrow_WhenMultipleHandlersRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<ICommandHandler<MyCommandWithResponse, string?>, CommandHandler>();
            services.AddTransient<ICommandHandler<MyCommandWithResponse, string?>, CommandHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            Action act = () => registry.HasCommandWithResponseHandler<MyCommandWithResponse>();

            act.Should().Throw<ApplicationException>()
                .WithMessage("*Multiple handlers*");
        }
    }

    public class HasQueryHandler
    {
        [Fact]
        public void ShouldReturnTrue_WhenHandlerIsRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<IQueryHandler<MyQuery, MyQueryResponse?>, QueryHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.HasQueryHandler<MyQuery>();

            result.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalse_WhenNoHandlerRegistered()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            var result = registry.HasQueryHandler<MyQuery>();

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldThrow_WhenTypeIsNotQuery()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            Action act = () => registry.HasQueryHandler<MyEvent>(); // MyEvent is not a query

            act.Should().Throw<ApplicationException>();
        }

        [Fact]
        public void ShouldThrow_WhenMultipleHandlersRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<IQueryHandler<MyQuery, MyQueryResponse?>, QueryHandler>();
            services.AddTransient<IQueryHandler<MyQuery, MyQueryResponse?>, QueryHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            Action act = () => registry.HasQueryHandler<MyQuery>();

            act.Should().Throw<ApplicationException>()
                .WithMessage("*Multiple handlers*");
        }
    }

    public class HasEventHandlers
    {
        [Fact]
        public void ShouldReturnTrue_WhenOneHandlerIsRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<IEventHandler<MyEvent>, EventsHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.HasEventHandlers<MyEvent>();

            result.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrue_WhenMultipleHandlersAreRegistered()
        {
            var services = new ServiceCollection();
            services.AddTransient<IEventHandler<MyEvent>, EventsHandler>();
            services.AddTransient<IEventHandler<MyEvent>, EventsHandler>();
            services.AddTransient<IEventHandler<MyEvent>, EventsHandler>();
            var provider = services.BuildServiceProvider();

            var registry = new ServiceProviderRegistry(provider);

            var result = registry.HasEventHandlers<MyEvent>();

            result.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalse_WhenNoHandlerRegistered()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            var result = registry.HasEventHandlers<MyEvent>();

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldThrow_WhenTypeIsNotEvent()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var registry = new ServiceProviderRegistry(provider);

            Action act = () => registry.HasEventHandlers<MyQuery>(); // MyQuery is not an event

            act.Should().Throw<ApplicationException>();
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
