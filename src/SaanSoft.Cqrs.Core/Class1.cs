namespace SaanSoft.Cqrs.Core;

public class Class1
{
    public Class1()
    {
    }

    public void Mthod1()
    {
        var temp = "warning";
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
