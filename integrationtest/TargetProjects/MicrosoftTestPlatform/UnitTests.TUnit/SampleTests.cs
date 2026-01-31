using TUnit.Core;
using TargetProject.StrykerFeatures;

namespace NetCoreTestProject.TUnit;

public class SampleTests
{
    [Test]
    [Arguments(29, false)]
    [Arguments(31, true)]
    public async Task TestAgeExplicit(int age, bool expired)
    {
        var sut = new KilledMutants { Age = age };

        var result = sut.IsExpiredBool();

        await Assert.That(result == expired).IsTrue();
    }
}

