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
}
