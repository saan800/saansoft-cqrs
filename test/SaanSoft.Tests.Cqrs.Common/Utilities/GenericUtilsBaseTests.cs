using FluentAssertions;

namespace SaanSoft.Tests.Cqrs.Common.Utilities;

public class GenericUtilsBaseTests
{
    public abstract class IsNullOrDefault<TValue> where TValue : struct
    {
        protected abstract TValue NewId();

        [Fact]
        public void Nullable_without_value_should_return_true()
        {
            TValue? value = null;
            GenericUtils.IsNullOrDefault(value).Should().Be(true);
        }

        [Fact]
        public void Nullable_with_value_should_return_false()
        {
            TValue? value = NewId();
            GenericUtils.IsNullOrDefault(value).Should().Be(false);
        }

        [Fact]
        public void Default_value_should_return_true()
        {
            TValue value = default;
            GenericUtils.IsNullOrDefault(value).Should().BeTrue();
        }

        [Fact]
        public void Value_should_return_false()
        {
            TValue value = NewId();
            GenericUtils.IsNullOrDefault(value).Should().BeFalse();
        }
    }
}
