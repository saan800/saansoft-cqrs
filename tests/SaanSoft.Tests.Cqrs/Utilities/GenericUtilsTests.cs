using SaanSoft.Cqrs.Utilities;
using SaanSoft.Tests.Cqrs.Shared.Utilities;

namespace SaanSoft.Tests.Cqrs.Utilities;

public class GenericUtilsTests
{
    public class IsNullOrDefault
    {
        public class GuidType : GenericUtilsBaseTests.IsNullOrDefault.ForType<Guid>
        {
            protected override Guid NewValue() => Guid.NewGuid();
        }

        public class IntType : GenericUtilsBaseTests.IsNullOrDefault.ForType<int>
        {
            protected override int NewValue()
            {
                var random = new Random();
                return random.Next(1, int.MaxValue);
            }
        }

        public class DateTimeType : GenericUtilsBaseTests.IsNullOrDefault.ForType<DateTime>
        {
            protected override DateTime NewValue() => DateTime.UtcNow;


            [Fact]
            public void MinValue_should_return_true()
            {
                GenericUtils.IsNullOrDefault(DateTime.MinValue).Should().BeTrue();
            }
        }
    }
}
