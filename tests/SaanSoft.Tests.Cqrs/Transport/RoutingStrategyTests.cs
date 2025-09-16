using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Transport;

namespace SaanSoft.Tests.Cqrs.Transport;

public class RoutingStrategyTests
{
    /// <summary>
    /// If IExternalMessageTransport is not registered and doing InMemory execution only, this will:
    /// - Throw an exception if multiple handlers for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; are
    ///   found in the serviceRegistry.
    /// - Throw an exception if no handlers for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; are not found
    ///   in the serviceRegistry.
    /// - Return false if a single handler for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; is found in
    ///   the serviceRegistry.
    /// - Return false for IEvent, regardless of the number of handlers in the serviceRegistry.
    /// </summary>
    public class InMemoryOnly
    {
        protected readonly IServiceRegistry _serviceRegistry;
        protected readonly RoutingStrategy _routingStrategy;

        public InMemoryOnly()
        {
            _serviceRegistry = A.Fake<IServiceRegistry>();
            A.CallTo(() => _serviceRegistry.ResolveService<IExternalMessageTransport>()).Returns(null);

            _routingStrategy = new RoutingStrategy(_serviceRegistry);
        }

        [Fact]
        public void Unknown_message_type_throws_exception()
        {
            _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new AMessage()))
                .Should().Throw<NotSupportedException>();
        }

        public class Commands : InMemoryOnly
        {
            [Fact]
            public void Zero_handlers_should_throw_exception()
            {
                A.CallTo(() => _serviceRegistry.HasCommandHandler(A<Type>.Ignored)).Returns(false);

                _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new MyCommand()))
                    .Should().Throw<ApplicationException>();
            }

            [Fact]
            public void One_handler_should_return_false()
            {
                A.CallTo(() => _serviceRegistry.HasCommandHandler(A<Type>.Ignored)).Returns(true);

                var result = _routingStrategy.IsExternalMessage(new MyCommand());

                result.Should().BeFalse();
            }

            [Fact]
            public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
            {
                A.CallTo(() => _serviceRegistry.HasCommandHandler(A<Type>.Ignored)).Throws<Exception>();

                _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new MyCommand()))
                    .Should().Throw<Exception>();
            }
        }

        public class CommandWithResults : InMemoryOnly
        {
            [Fact]
            public void Zero_handlers_should_throw_exception()
            {
                A.CallTo(() => _serviceRegistry.HasCommandResultHandler(A<Type>.Ignored)).Returns(false);

                _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new MyCommandWithResult()))
                    .Should().Throw<ApplicationException>();
            }

            [Fact]
            public void One_handler_should_return_false()
            {
                A.CallTo(() => _serviceRegistry.HasCommandResultHandler(A<Type>.Ignored)).Returns(true);

                var result = _routingStrategy.IsExternalMessage(new MyCommandWithResult());

                result.Should().BeFalse();
            }

            [Fact]
            public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
            {
                A.CallTo(() => _serviceRegistry.HasCommandResultHandler(A<Type>.Ignored)).Throws<Exception>();

                _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new MyCommandWithResult()))
                    .Should().Throw<Exception>();
            }
        }

        public class Queries : InMemoryOnly
        {
            [Fact]
            public void Zero_handlers_should_throw_exception()
            {
                A.CallTo(() => _serviceRegistry.HasQueryHandler(A<Type>.Ignored)).Returns(false);

                _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new MyQuery()))
                    .Should().Throw<ApplicationException>();
            }

            [Fact]
            public void One_handler_should_return_false()
            {
                A.CallTo(() => _serviceRegistry.HasQueryHandler(A<Type>.Ignored)).Returns(true);

                var result = _routingStrategy.IsExternalMessage(new MyQuery());

                result.Should().BeFalse();
            }

            [Fact]
            public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
            {
                A.CallTo(() => _serviceRegistry.HasQueryHandler(A<Type>.Ignored)).Throws<Exception>();

                _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new MyQuery()))
                    .Should().Throw<Exception>();
            }
        }

        public class Events : InMemoryOnly
        {
            [Fact]
            public void Zero_handlers_should_return_false()
            {
                A.CallTo(() => _serviceRegistry.HasEventHandlers(A<Type>.Ignored)).Returns(false);

                var result = _routingStrategy.IsExternalMessage(new MyEvent(Guid.NewGuid()));

                result.Should().BeFalse();
            }

            [Fact]
            public void One_or_more_handlers_should_return_false()
            {
                A.CallTo(() => _serviceRegistry.HasEventHandlers(A<Type>.Ignored)).Returns(true);

                var result = _routingStrategy.IsExternalMessage(new MyEvent(Guid.NewGuid()));

                result.Should().BeFalse();
            }
        }
    }

    /// <summary>
    /// If IExternalMessageTransport is registered, this will:
    /// - Throw an exception if multiple handlers for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; are
    ///   found in the serviceRegistry.
    /// - Return true if no handlers for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; are not found in the
    ///   serviceRegistry.
    /// - Return false if a single handler for ICommand, ICommand&lt;TResult&gt;, IQuery&lt;TResult&gt; is found in
    ///   the serviceRegistry.
    /// - Return true for IEvent, regardless of the number of handlers in the serviceRegistry (assures events are
    ///   handled by all subscribers, and in published order).
    /// </summary>
    public class ExternalMessageTransport
    {
        protected readonly IServiceRegistry _serviceRegistry;
        protected readonly RoutingStrategy _routingStrategy;

        public ExternalMessageTransport()
        {
            _serviceRegistry = A.Fake<IServiceRegistry>();
            A.CallTo(() => _serviceRegistry.ResolveService<IExternalMessageTransport>())
                .Returns(A.Fake<IExternalMessageTransport>());

            _routingStrategy = new RoutingStrategy(_serviceRegistry);
        }


        [Fact]
        public void Unknown_message_type_throws_exception()
        {
            _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new AMessage()))
                .Should().Throw<NotSupportedException>();
        }

        public class Commands : ExternalMessageTransport
        {
            [Fact]
            public void Zero_handlers_should_return_true()
            {
                A.CallTo(() => _serviceRegistry.HasCommandHandler(A<Type>.Ignored)).Returns(false);

                var result = _routingStrategy.IsExternalMessage(new MyCommand());

                result.Should().BeTrue();
            }

            [Fact]
            public void One_handler_should_return_false()
            {
                A.CallTo(() => _serviceRegistry.HasCommandHandler(A<Type>.Ignored)).Returns(true);

                var result = _routingStrategy.IsExternalMessage(new MyCommand());

                result.Should().BeFalse();
            }

            [Fact]
            public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
            {
                A.CallTo(() => _serviceRegistry.HasCommandHandler(A<Type>.Ignored)).Throws<Exception>();

                _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new MyCommand()))
                    .Should().Throw<Exception>();
            }
        }

        public class CommandWithResults : ExternalMessageTransport
        {
            [Fact]
            public void Zero_handlers_should_return_true()
            {
                A.CallTo(() => _serviceRegistry.HasCommandResultHandler(A<Type>.Ignored)).Returns(false);

                var result = _routingStrategy.IsExternalMessage(new MyCommandWithResult());

                result.Should().BeTrue();
            }

            [Fact]
            public void One_handler_should_return_false()
            {
                A.CallTo(() => _serviceRegistry.HasCommandResultHandler(A<Type>.Ignored)).Returns(true);

                var result = _routingStrategy.IsExternalMessage(new MyCommandWithResult());

                result.Should().BeFalse();
            }

            [Fact]
            public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
            {
                A.CallTo(() => _serviceRegistry.HasCommandResultHandler(A<Type>.Ignored)).Throws<Exception>();

                _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new MyCommandWithResult()))
                    .Should().Throw<Exception>();
            }
        }

        public class Queries : ExternalMessageTransport
        {
            [Fact]
            public void Zero_handlers_should_return_true()
            {
                A.CallTo(() => _serviceRegistry.HasQueryHandler(A<Type>.Ignored)).Returns(false);

                var result = _routingStrategy.IsExternalMessage(new MyQuery());

                result.Should().BeTrue();
            }

            [Fact]
            public void One_handler_should_return_false()
            {
                A.CallTo(() => _serviceRegistry.HasQueryHandler(A<Type>.Ignored)).Returns(true);

                var result = _routingStrategy.IsExternalMessage(new MyQuery());

                result.Should().BeFalse();
            }

            [Fact]
            public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
            {
                A.CallTo(() => _serviceRegistry.HasQueryHandler(A<Type>.Ignored)).Throws<Exception>();

                _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage(new MyQuery()))
                    .Should().Throw<Exception>();
            }
        }

        public class Events : ExternalMessageTransport
        {
            [Fact]
            public void Zero_handlers_should_return_true()
            {
                A.CallTo(() => _serviceRegistry.HasEventHandlers(A<Type>.Ignored)).Returns(false);

                var result = _routingStrategy.IsExternalMessage(new MyEvent(Guid.NewGuid()));

                result.Should().BeTrue();
            }

            [Fact]
            public void One_or_more_handlers_should_return_true()
            {
                A.CallTo(() => _serviceRegistry.HasEventHandlers(A<Type>.Ignored)).Returns(true);

                var result = _routingStrategy.IsExternalMessage(new MyEvent(Guid.NewGuid()));

                result.Should().BeTrue();
            }
        }
    }
}
