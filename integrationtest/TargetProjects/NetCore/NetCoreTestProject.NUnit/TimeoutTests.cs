using NUnit.Framework;
using TargetProject.StrykerFeatures;

namespace NetCoreTestProject.NUnit;

[TestFixture]
internal class TimeoutTests
{
    [Test]
    public void TestTimeout()
    {
        var target = new Timeout();
        target.SomeLoop();
    }
}