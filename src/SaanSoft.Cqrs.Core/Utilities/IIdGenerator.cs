namespace SaanSoft.Cqrs.Core.Utilities;

public interface IIdGenerator<out TMessageId> where TMessageId : struct
{
    TMessageId NewId();
}
