namespace SaanSoft.Cqrs.Decorator;

/// <summary>
/// Empty interface that all event bus decorators should inherit
/// </summary>
/// <remarks>
/// Useful for when you need to specifically include or exclude event bus decorators
/// (e.g. BaseStoreMessagePublisherDecorator needs to filter our all decorators)
/// </remarks>
public interface IEventBusDecorator<TMessageId> :
    IMessageBusDecorator,
    IEventBus<TMessageId> where TMessageId : struct
{
}
