namespace SaanSoft.Cqrs.Decorator;

/// <summary>
/// Empty interface that all event subscription bus decorators should inherit.
///
/// You should never directly inherit from this interface
/// </summary>
/// <remarks>
/// Useful for when you need to specifically include or exclude subscription bus decorators
/// (e.g. BaseStoreMessagePublisherDecorator needs to filter our all decorators)
/// </remarks>
public interface IEventSubscriptionBusDecorator<TMessageId> :
    IMessageSubscriptionBus,
    IEventSubscriptionBus<TMessageId> where TMessageId : struct
{
}
