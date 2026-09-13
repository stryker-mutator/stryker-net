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

        var result = sut.IsExpiredBool();

        Assert.True(expired == result);
    }

    [Fact]
    public void TestExtraProjectLessons()
    {
        var sut = new global::ExtraProject.Teacher();

        Assert.Equal(0, sut.Lessons);

        sut.AddLesson();

        Assert.Equal(1, sut.Lessons);
    }
}

