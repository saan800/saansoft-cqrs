namespace SaanSoft.Tests.Cqrs.Shared;

public class FakeTest
{
    /// <summary>
    /// Keeps .Net happy to have one test in the project
    /// </summary>
    [Fact]
    public void ATest()
    {
        (1 + 2).Should().Be(3);
    }
}
