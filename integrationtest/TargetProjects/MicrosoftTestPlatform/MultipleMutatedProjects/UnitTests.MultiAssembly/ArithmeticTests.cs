using Calculator;
using Xunit;

namespace UnitTests.MultiAssembly;

/// <summary>
/// Everything goes through <see cref="Arithmetic"/>: the tests never name the lower assembly, whose
/// mutants are covered only because the upper one calls into it.
/// </summary>
public class ArithmeticTests
{
    [Fact]
    public void Add_ShouldSumBothOperands()
    {
        Assert.Equal(7, Arithmetic.Add(3, 4));
    }

    [Fact]
    public void IsPositive_ShouldTellPositiveFromZero()
    {
        Assert.True(Arithmetic.IsPositive(1));
        Assert.False(Arithmetic.IsPositive(0));
    }

    [Fact]
    public void Clamp_ShouldCapAtMaximum()
    {
        Assert.Equal(5, Arithmetic.Clamp(9, 5));
        Assert.Equal(2, Arithmetic.Clamp(2, 5));
    }
}
