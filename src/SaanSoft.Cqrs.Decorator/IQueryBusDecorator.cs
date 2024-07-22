using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Cqrs.Decorator;

/// <summary>
/// Empty interface that all query bus decorators should inherit
/// </summary>
/// <remarks>
/// Useful for when you need to specifically include or exclude query bus decorators
/// (e.g. BaseStoreMessagePublisherDecorator needs to filter our all decorators)
/// </remarks>
public interface IQueryBusDecorator :
    IMessageBusDecorator,
    IQueryBus
{
}
