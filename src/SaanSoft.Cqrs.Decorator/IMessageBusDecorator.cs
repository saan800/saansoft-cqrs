namespace SaanSoft.Cqrs.Decorator;

/// <summary>
/// Empty interface that all message bus decorators should inherit.
///
/// You shoudld not in
/// </summary>
/// <remarks>
/// Useful for when you need to specifically include or exclude publisher decorators
/// (e.g. BaseStoreMessagePublisherDecorator needs to filter our all decorators)
/// </remarks>
public interface IMessageBusDecorator : IDecorator
{
}
