using SaanSoft.Cqrs.Decorator.Store;

namespace SaanSoft.Cqrs.GuidIds.Decorator.Store;

/// <summary>
/// Add the publisher to the command's metadata.
///
/// Should be used in conjunction with <see cref="StoreCommandDecorator"/>
/// </summary>
/// <param name="next"></param>
public class StoreCommandPublisherDecorator(ICommandBus next)
    : StoreCommandPublisherDecorator<Guid>(next),
      ICommandBus;
