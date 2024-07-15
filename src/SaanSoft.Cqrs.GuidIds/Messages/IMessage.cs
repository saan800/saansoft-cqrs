using SaanSoft.Cqrs.Common.Messages;
using SaanSoft.Cqrs.Core.Messages;

namespace SaanSoft.Cqrs.GuidIds.Messages;

public interface IMessage : IBaseMessage<Guid>
{
}
