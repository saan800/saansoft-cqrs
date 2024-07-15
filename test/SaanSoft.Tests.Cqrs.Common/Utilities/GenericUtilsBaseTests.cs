using FluentAssertions;

namespace SaanSoft.Tests.Cqrs.Common.Utilities;

public class GenericUtilsBaseTests
{
    public class IsNullOrDefault
    {
        public abstract class MessageId<TMessageId> where TMessageId : struct
        {
            protected abstract TMessageId NewId();

            [Fact]
            public void Nullable_without_value_should_return_true()
            {
                TMessageId? value = null;
                GenericUtils.IsNullOrDefault(value).Should().Be(true);
            }

            [Fact]
            public void Nullable_with_value_should_return_false()
            {
                TMessageId? value = NewId();
                GenericUtils.IsNullOrDefault(value).Should().Be(false);
            }

            [Fact]
            public void Default_value_should_return_true()
            {
                TMessageId value = default;
                GenericUtils.IsNullOrDefault(value).Should().BeTrue();
            }

            [Fact]
            public void Value_should_return_false()
            {
                TMessageId value = NewId();
                GenericUtils.IsNullOrDefault(value).Should().BeFalse();
            }
        }
    }
}
