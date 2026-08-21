using Xunit;

namespace NetCoreTestProject.XUnit;
public class VariousTests
{
    [Fact]
    public void AssertShouldKillMutation()
    {
        var target = new TargetProject.StrykerFeatures.UseAssert();
        target.IncrementCounter();
        // no assert needed, Debug.Assert will throw if counter is less than 0
    }

    [Fact]
    public void GetHelloUtf8_ShouldReturnHello()
    {
        var result = TargetProject.Constructs.CSharp11.GetHelloUtf8();
        var expected = "Hello"u8.ToArray();
        Assert.Equal(expected, result.ToArray());
    }
}
