using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

/// <summary>
/// Add the publisher to the event's metadata.
///
/// Should be used in conjunction with <see cref="StoreEventDecorator"/>
/// </summary>
/// <param name="next"></param>
public class StoreEventPublisherDecorator(IEventBus next)
    : StoreEventPublisherDecorator<Guid>(next),
      IEventBus;
