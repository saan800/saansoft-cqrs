namespace SaanSoft.Cqrs.Decorator;

/// <summary>
/// Empty interface that all decorators should inherit
/// </summary>
/// <remarks>
/// Useful for when you need to specifically include or exclude decorators
/// (e.g. BaseStoreMessagePublisherDecorator needs to filter our all decorators)
/// </remarks>
public interface IDecorator
{
}
