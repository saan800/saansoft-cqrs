using SaanSoft.Cqrs.Utilities;

namespace SaanSoft.Tests.Cqrs.Utilities;

public class GenericUtilsTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("00000000-0000-0000-0000-000000000000", false)]
    [InlineData("3311621e-c1c5-47f0-8ceb-638833210fab", false)]
    public void IsNullOrDefault_Nullable_Guid(string? valueAsString, bool expectedResult)
    {
        Guid? guidValue = string.IsNullOrWhiteSpace(valueAsString) ? null : Guid.Parse(valueAsString);
        GenericUtils.IsNullOrDefault(guidValue).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("00000000-0000-0000-0000-000000000000", true)]
    [InlineData("3311621e-c1c5-47f0-8ceb-638833210fab", false)]
    public void IsNullOrDefault_Guid(string? valueAsString, bool expectedResult)
    {
        Guid guidValue = string.IsNullOrWhiteSpace(valueAsString) ? default : Guid.Parse(valueAsString);
        GenericUtils.IsNullOrDefault(guidValue).Should().Be(expectedResult);
    }
}
