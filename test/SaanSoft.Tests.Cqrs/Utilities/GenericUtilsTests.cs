namespace SaanSoft.Tests.Cqrs.Utilities;

public class GenericUtilsTests
{
    public class IsNullOrDefault
    {
        public class GuidType : GenericUtilsBaseTests.IsNullOrDefault.MessageId<Guid>
        {
            protected override Guid NewId() => Guid.NewGuid();
        }

        public class IntType : GenericUtilsBaseTests.IsNullOrDefault.MessageId<int>
        {
            protected override int NewId()
            {
                var random = new Random();
                return random.Next(1, int.MaxValue);
            }
        }
    }
}
