namespace SaanSoft.Tests.Cqrs.Utilities;

public class GenericUtilsTests
{
    public class IsNullOrDefault
    {
        public class GuidType : GenericUtilsBaseTests.IsNullOrDefault<Guid>
        {
            protected override Guid NewId() => Guid.NewGuid();
        }

        public class IntType : GenericUtilsBaseTests.IsNullOrDefault<int>
        {
            protected override int NewId()
            {
                var random = new Random();
                return random.Next(1, int.MaxValue);
            }
        }
    }
}
