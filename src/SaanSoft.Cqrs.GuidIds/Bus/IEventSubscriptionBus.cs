using SaanSoft.Cqrs.Core.Bus;

namespace SaanSoft.Cqrs.GuidIds.Bus;

public interface IEventSubscriptionBus : IBaseEventSubscriptionBus<Guid>
{
}
