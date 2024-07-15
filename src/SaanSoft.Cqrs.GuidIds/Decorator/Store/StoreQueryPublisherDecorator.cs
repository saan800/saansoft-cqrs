using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

/// <summary>
/// Add the publisher to the query's metadata.
///
/// Should be used in conjunction with <see cref="StoreQueryDecorator"/>
/// </summary>
/// <param name="next"></param>
public class StoreQueryPublisherDecorator(IQueryBus next)
    : StoreQueryPublisherDecorator<Guid>(next),
      IQueryBus;
