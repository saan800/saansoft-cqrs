namespace SaanSoft.Cqrs.Decorator;

/// <summary>
/// Empty interface that all publisher decorators should inherit
/// </summary>
/// <remarks>
/// Useful for when you need to specifically include or exclude subscriber decorators
/// </remarks>
public interface ISubscriberDecorator : IDecorator
{
}
