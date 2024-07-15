using SaanSoft.Cqrs.Common.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public interface IMessage : IBaseMessage<Guid>
{
}
