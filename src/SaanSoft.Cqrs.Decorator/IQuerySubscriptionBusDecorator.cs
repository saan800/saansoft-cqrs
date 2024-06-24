using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Cqrs.Decorator;

/// <summary>
/// Empty interface that all query subscription bus decorators should inherit.
///
/// You should never directly inherit from this interface
/// </summary>
/// <remarks>
/// Useful for when you need to specifically include or exclude subscription bus decorators
/// (e.g. BaseStoreMessagePublisherDecorator needs to filter our all decorators)
/// </remarks>
public interface IQuerySubscriptionBusDecorator<TMessageId> :
    IMessageSubscriptionBus,
    IQuerySubscriptionBus<TMessageId> where TMessageId : struct
{
}
