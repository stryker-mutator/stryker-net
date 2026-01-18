using Xunit;

namespace ExampleLibrary.Tests;

public class ExampleTests
{
    [Fact]
    public void TestSayHello()
    {
        var example = new Example();
        var result = example.SayHello();
        Assert.Equal("Hello World!", result);
    }
}
