using ConsoleApp1;

namespace TestProject1;

public class CalculatorTests
{
    [Fact]
    public void Add_ReturnsCorrectSum()
    {
        var calculator = new Calculator();
        var result = calculator.Add(2, 3);
        Assert.Equal(5, result);
    }

    [Fact]
    public void Subtract_ReturnsCorrectDifference()
    {
        var calculator = new Calculator();
        var result = calculator.Subtract(5, 3);
        Assert.Equal(2, result);
    }

    [Fact]
    public void Multiply_ReturnsCorrectProduct()
    {
        var calculator = new Calculator();
        var result = calculator.Multiply(2, 3);
        Assert.Equal(6, result);
    }

    [Fact]
    public void Divide_ReturnsCorrectQuotient()
    {
        var calculator = new Calculator();
        var result = calculator.Divide(6, 3);
        Assert.Equal(2, result);
    }
}