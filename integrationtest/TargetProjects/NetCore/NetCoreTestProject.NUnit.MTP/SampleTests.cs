using NUnit.Framework;
using TargetProject.StrykerFeatures;

namespace TargetProject.NUnit.MTP;

[TestFixture]
public class SampleTests
{
    [TestCase(29, false)]
    [TestCase(31, true)]
    public void TestAgeExplicit(int age, bool expired)
    {
        var sut = new KilledMutants { Age = age };

        var result = sut.IsExpiredBool();

        Assert.That(expired == result, Is.True);
    }
}

