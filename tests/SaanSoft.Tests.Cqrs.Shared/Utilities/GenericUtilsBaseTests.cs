using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Tests.Cqrs.Shared.Utilities;

public class GenericUtilsBaseTests
{
    public class IsNullOrDefault
    {
        public abstract class ForType<T> where T : struct
        {
            protected abstract T NewId();

            [Fact]
            public void Nullable_without_value_should_return_true()
            {
                T? value = null;
                GenericUtils.IsNullOrDefault(value).Should().Be(true);
            }

            [Fact]
            public void Nullable_with_value_should_return_false()
            {
                T? value = NewId();
                GenericUtils.IsNullOrDefault(value).Should().Be(false);
            }

            [Fact]
            public void Default_value_should_return_true()
            {
                T value = default;
                GenericUtils.IsNullOrDefault(value).Should().BeTrue();
            }

            [Fact]
            public void Value_should_return_false()
            {
                T value = NewId();
                GenericUtils.IsNullOrDefault(value).Should().BeFalse();
            }
        }
    }
}
