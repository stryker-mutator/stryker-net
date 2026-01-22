using Xunit;
using TargetProject.StrykerFeatures;

namespace TargetProject.XUnit.MTP;

public class SampleTests
{
    [Theory]
    [InlineData(29, false)]
    [InlineData(31, true)]
    public void TestAgeExplicit(int age, bool expired)
    {
        var sut = new KilledMutants { Age = age };
        Assert.True(expired == sut.IsExpiredBool());
    }
}

