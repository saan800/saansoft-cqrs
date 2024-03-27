namespace SaanSoft.Cqrs.Core;

public class Class1
{
    public Class1()
    {
    }

    /// <summary>
    /// Receive recieve
    /// </summary>
    public void Mthod1()
    {
        var temp = "wrning";
        Console.WriteLine(temp);

        var testWarningsAsErrors = new TestClass
        {
            Name = "Bob"
        };

    }

    public class TestClass
    {
        public required string Name { get; set; }
    }
}
