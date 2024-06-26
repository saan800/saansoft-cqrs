namespace SaanSoft.Cqrs.Utilities;

public interface IIdGenerator<out TMessageId> where TMessageId : struct
{
    TMessageId NewId();
}
