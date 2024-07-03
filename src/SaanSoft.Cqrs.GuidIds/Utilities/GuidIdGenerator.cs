namespace SaanSoft.Cqrs.GuidIds.Utilities;

public class GuidIdGenerator : IIdGenerator
{
    public Guid NewId() => New;

    public static Guid New => Guid.NewGuid();
}
