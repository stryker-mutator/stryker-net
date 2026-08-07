using Calculator;
using Xunit;

namespace UnitTests.MultiAssembly;

/// <summary>
/// Exercises the lower assembly through the upper one only, the way a real test project reaches a
/// transitively referenced project.
/// </summary>
public class LabelDelegationTests
{
    [Fact]
    public void LabelBudget_ShouldMeasureLabelAndCapIt()
    {
        Assert.Equal(0, Arithmetic.LabelBudget(null, 5));
        Assert.Equal(3, Arithmetic.LabelBudget("abc", 5));
        Assert.Equal(5, Arithmetic.LabelBudget("abcdefgh", 5));
    }

    [Fact]
    public void LabelFits_ShouldCompareAgainstMaxLength()
    {
        Assert.True(Arithmetic.LabelFits("abc", 3));
        Assert.False(Arithmetic.LabelFits("abcd", 3));
    }

    [Fact]
    public void ShortLabel_ShouldKeepShortLabelsIntact()
    {
        Assert.Equal("abc", Arithmetic.ShortLabel("abc", 3));
        Assert.Equal("ab", Arithmetic.ShortLabel("abcd", 2));
    }
}
