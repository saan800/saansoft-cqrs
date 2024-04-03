using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Tests.Cqrs.Utilities;

public class GenericUtilsTests
{
    [Fact]
    public void IsNullOrDefault_Nullable_Guid_without_value_should_return_true()
    {
        Guid? guidValue = null;
        GenericUtils.IsNullOrDefault(guidValue).Should().Be(true);
    }

    [Theory]
    [AutoFakeData]
    public void IsNullOrDefault_Nullable_Guid_with_value_should_return_false(Guid? value)
    {
        GenericUtils.IsNullOrDefault(value).Should().Be(false);
    }

    [Fact]
    public void IsNullOrDefault_Guid_with_default_value_should_return_true()
    {
        Guid guidValue = default;
        GenericUtils.IsNullOrDefault(guidValue).Should().BeTrue();
    }

    [Theory]
    [AutoFakeData]
    public void IsNullOrDefault_Guid_with_value_should_return_false(Guid value)
    {
        GenericUtils.IsNullOrDefault(value).Should().BeFalse();
    }
}
