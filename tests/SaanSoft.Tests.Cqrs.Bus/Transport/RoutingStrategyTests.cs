using SaanSoft.Cqrs.Bus.Transport;
using SaanSoft.Cqrs.DependencyInjection;
using SaanSoft.Cqrs.Transport;

namespace SaanSoft.Tests.Cqrs.Bus.Transport;

public class RoutingStrategyTests
{
    // public class LocalOnly
    // {
    //     private readonly IServiceRegistry _serviceRegistry;
    //     private readonly RoutingStrategy _sut;

    //     public LocalOnly()
    //     {
    //         _serviceRegistry = A.Fake<IServiceRegistry>();
    //         A.CallTo(() => _serviceRegistry.ResolveService<ExternalMessageProcessor>()).Returns(null);
    //         A.CallTo(() => _serviceRegistry.ResolveService<LocalMessageRouter>()).Returns(
    //             A.Fake<LocalMessageRouter>()
    //         );


    //         _sut = new RoutingStrategy(_serviceRegistry);
    //     }

    //     [Fact]
    //     public void Command_WithLocalHandler_ShouldUseLocalRouter()
    //     {
    //         // Arrange
    //         _serviceRegistry.RegisterSingleton<ICommandHandler<MyCommand>, MyCommandHandler>();

    //         // Act
    //         var router = _sut.GetMessageRouter<MyCommand>();

    //         // Assert
    //         router.Should().BeOfType<LocalMessageRouter>();
    //     }
    // }
}


// public class RoutingStrategyTests
// {
//     /// <summary>
//     /// If IExternalMessageBroker is not registered and doing Local execution only, this will:
//     /// - Throw an exception if multiple handlers for ICommand, ICommand&lt;TResponse&gt;, IQuery&lt;TResponse&gt;
//           are found in the serviceRegistry.
//     /// - Throw an exception if no handlers for ICommand, ICommand&lt;TResponse&gt;, IQuery&lt;TResponse&gt; are not
//           found in the serviceRegistry.
//     /// - Return false if a single handler for ICommand, ICommand&lt;TResponse&gt;, IQuery&lt;TResponse&gt; is found
//           in the serviceRegistry.
//     /// - Return false for IEvent, regardless of the number of handlers in the serviceRegistry.
//     /// </summary>
//     public class LocalOnly
//     {
//         protected readonly IServiceRegistry _serviceRegistry;
//         protected readonly RoutingStrategy _routingStrategy;

//         public LocalOnly()
//         {
//             _serviceRegistry = A.Fake<IServiceRegistry>();
//             A.CallTo(() => _serviceRegistry.ResolveService<IExternalMessageBroker>()).Returns(null);

//             _routingStrategy = new RoutingStrategy(_serviceRegistry);
//         }

//         [Fact]
//         public void Unknown_message_type_throws_exception()
//         {
//             _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<AMessage>())
//                 .Should().Throw<NotSupportedException>();
//         }

//         public class Commands : LocalOnly
//         {
//             [Fact]
//             public void Zero_handlers_should_throw_exception()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandHandler<MyCommand>()).Returns(false);

//                 _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<MyCommand>())
//                     .Should().Throw<ApplicationException>();
//             }

//             [Fact]
//             public void One_handler_should_return_false()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandHandler<MyCommand>()).Returns(true);

//                 var result = _routingStrategy.IsExternalMessage<MyCommand>();

//                 result.Should().BeFalse();
//             }

//             [Fact]
//             public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandHandler<MyCommand>()).Throws<Exception>();

//                 _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<MyCommand>())
//                     .Should().Throw<Exception>();
//             }
//         }

//         public class CommandWithResponse : LocalOnly
//         {
//             [Fact]
//             public void Zero_handlers_should_throw_exception()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandWithResponseHandler<MyCommandWithResponse>())
//                     .Returns(false);

//                 _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<MyCommandWithResponse>())
//                     .Should().Throw<ApplicationException>();
//             }

//             [Fact]
//             public void One_handler_should_return_false()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandWithResponseHandler<MyCommandWithResponse>())
//                     .Returns(true);

//                 var result = _routingStrategy.IsExternalMessage<MyCommandWithResponse>();

//                 result.Should().BeFalse();
//             }

//             [Fact]
//             public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandWithResponseHandler<MyCommandWithResponse>())
//                     .Throws<Exception>();

//                 _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<MyCommandWithResponse>())
//                     .Should().Throw<Exception>();
//             }
//         }

//         public class Queries : LocalOnly
//         {
//             [Fact]
//             public void Zero_handlers_should_throw_exception()
//             {
//                 A.CallTo(() => _serviceRegistry.HasQueryHandler<MyQuery>()).Returns(false);

//                 _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<MyQuery>())
//                     .Should().Throw<ApplicationException>();
//             }

//             [Fact]
//             public void One_handler_should_return_false()
//             {
//                 A.CallTo(() => _serviceRegistry.HasQueryHandler<MyQuery>()).Returns(true);

//                 var result = _routingStrategy.IsExternalMessage<MyQuery>();

//                 result.Should().BeFalse();
//             }

//             [Fact]
//             public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
//             {
//                 A.CallTo(() => _serviceRegistry.HasQueryHandler<MyQuery>()).Throws<Exception>();

//                 _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<MyQuery>())
//                     .Should().Throw<Exception>();
//             }
//         }

//         public class Events : LocalOnly
//         {
//             [Fact]
//             public void Zero_handlers_should_return_false()
//             {
//                 A.CallTo(() => _serviceRegistry.HasEventHandlers<MyEvent>()).Returns(false);

//                 var result = _routingStrategy.IsExternalMessage<MyEvent>();

//                 result.Should().BeFalse();
//             }

//             [Fact]
//             public void One_or_more_handlers_should_return_false()
//             {
//                 A.CallTo(() => _serviceRegistry.HasEventHandlers<MyEvent>()).Returns(true);

//                 var result = _routingStrategy.IsExternalMessage<MyEvent>();

//                 result.Should().BeFalse();
//             }
//         }
//     }

//     /// <summary>
//     /// If IExternalMessageBroker is registered, this will:
//     /// - Throw an exception if multiple handlers for ICommand, ICommand&lt;TResponse&gt;, IQuery&lt;TResponse&gt
//     ///   are found in the serviceRegistry.
//     /// - Return true if no handlers for ICommand, ICommand&lt;TResponse&gt;, IQuery&lt;TResponse&gt; are not 
//     ///   found in theserviceRegistry.
//     /// - Return false if a single handler for ICommand, ICommand&lt;TResponse&gt;, IQuery&lt;TResponse&gt; is 
//     ///   found in the serviceRegistry.
//     /// - Return true for IEvent, regardless of the number of handlers in the serviceRegistry (assures events are
//     ///   handled by all subscribers, and in published order).
//     /// </summary>
//     public class ExternalMessageBroker
//     {
//         protected readonly IServiceRegistry _serviceRegistry;
//         protected readonly RoutingStrategy _routingStrategy;

//         public ExternalMessageBroker()
//         {
//             _serviceRegistry = A.Fake<IServiceRegistry>();
//             A.CallTo(() => _serviceRegistry.ResolveService<IExternalMessageBroker>())
//                 .Returns(A.Fake<IExternalMessageBroker>());

//             _routingStrategy = new RoutingStrategy(_serviceRegistry);
//         }


//         [Fact]
//         public void Unknown_message_type_throws_exception()
//         {
//             _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<AMessage>())
//                 .Should().Throw<NotSupportedException>();
//         }

//         public class Commands : ExternalMessageBroker
//         {
//             [Fact]
//             public void Zero_handlers_should_return_true()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandHandler<MyCommand>()).Returns(false);

//                 var result = _routingStrategy.IsExternalMessage<MyCommand>();

//                 result.Should().BeTrue();
//             }

//             [Fact]
//             public void One_handler_should_return_false()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandHandler<MyCommand>()).Returns(true);

//                 var result = _routingStrategy.IsExternalMessage<MyCommand>();

//                 result.Should().BeFalse();
//             }

//             [Fact]
//             public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandHandler<MyCommand>()).Throws<Exception>();

//                 _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<MyCommand>())
//                     .Should().Throw<Exception>();
//             }
//         }

//         public class CommandWithResponse : ExternalMessageBroker
//         {
//             [Fact]
//             public void Zero_handlers_should_return_true()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandWithResponseHandler<MyCommandWithResponse>())
//                     .Returns(false);

//                 var result = _routingStrategy.IsExternalMessage<MyCommandWithResponse>();

//                 result.Should().BeTrue();
//             }

//             [Fact]
//             public void One_handler_should_return_false()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandWithResponseHandler<MyCommandWithResponse>())
//                     .Returns(true);

//                 var result = _routingStrategy.IsExternalMessage<MyCommandWithResponse>();

//                 result.Should().BeFalse();
//             }

//             [Fact]
//             public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
//             {
//                 A.CallTo(() => _serviceRegistry.HasCommandWithResponseHandler<MyCommandWithResponse>())
//                     .Throws<Exception>();

//                 _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<MyCommandWithResponse>())
//                     .Should().Throw<Exception>();
//             }
//         }

//         public class Queries : ExternalMessageBroker
//         {
//             [Fact]
//             public void Zero_handlers_should_return_true()
//             {
//                 A.CallTo(() => _serviceRegistry.HasQueryHandler<MyQuery>()).Returns(false);

//                 var result = _routingStrategy.IsExternalMessage<MyQuery>();

//                 result.Should().BeTrue();
//             }

//             [Fact]
//             public void One_handler_should_return_false()
//             {
//                 A.CallTo(() => _serviceRegistry.HasQueryHandler<MyQuery>()).Returns(true);

//                 var result = _routingStrategy.IsExternalMessage<MyQuery>();

//                 result.Should().BeFalse();
//             }

//             [Fact]
//             public void Multiple_handlers_ServiceRegistry_throws_exception_that_should_bubble_up()
//             {
//                 A.CallTo(() => _serviceRegistry.HasQueryHandler<MyQuery>()).Throws<Exception>();

//                 _routingStrategy.Invoking(y => _routingStrategy.IsExternalMessage<MyQuery>())
//                     .Should().Throw<Exception>();
//             }
//         }

//         public class Events : ExternalMessageBroker
//         {
//             [Fact]
//             public void Zero_handlers_should_return_true()
//             {
//                 A.CallTo(() => _serviceRegistry.HasEventHandlers<MyEvent>()).Returns(false);

//                 var result = _routingStrategy.IsExternalMessage<MyEvent>();

//                 result.Should().BeTrue();
//             }

//             [Fact]
//             public void One_or_more_handlers_should_return_true()
//             {
//                 A.CallTo(() => _serviceRegistry.HasEventHandlers<MyEvent>()).Returns(true);

//                 var result = _routingStrategy.IsExternalMessage<MyEvent>();

//                 result.Should().BeTrue();
//             }
//         }
//     }
// }
