using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Cqrs.Decorator.GuidIds.Utilities;

public class GuidIdGenerator : IIdGenerator<Guid>
{
    public Guid NewId() => New;

    public static Guid New => Guid.NewGuid();
}
