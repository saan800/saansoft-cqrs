namespace SaanSoft.Tests.Cqrs.Core;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Passing()
    {
        Assert.Pass();
    }



    [Test]
    public void Failing()
    {
        throw new Exception("This test failed!");
    }
}

