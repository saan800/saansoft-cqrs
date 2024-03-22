namespace SaanSoft.Cqrs.Core;

public class Class1
{
    public Class1()
    {
    }

    public void Method1()
    {
        Console.WriteLine("Hello World");

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