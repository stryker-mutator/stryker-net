using TargetProject.StrykerFeatures;

namespace NetCoreTestProject.MSTest.MTP;

[TestClass]
public class SampleTests
{
    [TestMethod]
    [DataRow(29, false)]
    [DataRow(31, true)]
    public void TestAgeExplicit(int age, bool expired)
    {
        var sut = new KilledMutants { Age = age };

        var result = sut.IsExpiredBool();

        Assert.IsTrue(expired == result);
    }

    [TestMethod]
    public void TestTimeout()
    {
        var sut = new TargetProject.StrykerFeatures.Timeout();

        sut.SomeLoop();
    }
}
