using SaanSoft.Cqrs.Bus;

namespace SaanSoft.Cqrs.GuidIds.Bus;

public interface ICommandBus : ICommandBus<Guid>
{
}
