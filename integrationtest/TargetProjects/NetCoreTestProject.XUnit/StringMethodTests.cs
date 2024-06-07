using Xunit;

namespace ExampleProject.XUnit;

public class StringMethodTests
{
    [Fact]
    public void ExampleChain()
    {
        var stringMethods = new StringMethods();
        Assert.Equal('S', stringMethods.ExampleChain());
    }

    [Fact]
    public void ExampleChain2()
    {
        var stringMethods = new StringMethods();
        Assert.Equal('S', stringMethods.ExampleChain2());
    }
}
