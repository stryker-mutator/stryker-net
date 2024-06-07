using System.Collections.Generic;
using System.Drawing;

namespace TargetProject.Constructs;

public class Csharp9
{
    // patern matching
    public bool IsLetter(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';

    public bool IsLetterOrSeparator(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '.' or ',';

    // null check
    public bool GetLength(string? text) => text is not null;

    // target type new expressions
    public void TargetTypeNewExpressions()
    {
        List<string> list = new() { "one", "two", "three" };
        Dictionary<int, string> dictionary = new() { [1] = "one", [2] = "two", [3] = "three" };
    }

    // with expressions
    public void WithExpressions()
    {
        var point = new Point(3, 4);
        var newPoint = point with { X = 5 };
    }

    // init only setters
    public class ExamplePoint
    {
        public int X { get; init; }
        public int Y { get; init; }

        public ExamplePoint(int x, int y) => (X, Y) = (x, y);
    }

    // logical patterns
    string WaterState(int tempInFahrenheit) =>
    tempInFahrenheit switch
    {
        (> 32) and (< 212) => "liquid",
        < 32 => "solid",
        > 212 => "gas",
        32 => "solid/liquid transition",
        212 => "liquid / gas transition",
    };

    public record Order(int Items, decimal Cost);

    public decimal CalculateDiscount(Order order) =>
    order switch
    {
        { Items: > 10, Cost: > 1000.00m } => 0.10m,
        { Items: > 5, Cost: > 500.00m } => 0.05m,
        { Cost: > 250.00m } => 0.02m,
        null => throw new ArgumentNullException(nameof(order), "Can't calculate discount on null order"),
        var someObject => 0m
    };

    // declaration patterns
    public void DeclarationPatterns()
    {
        var y = new object();
        var x = y is object car ? car.ToString() : "null";
    }
}
