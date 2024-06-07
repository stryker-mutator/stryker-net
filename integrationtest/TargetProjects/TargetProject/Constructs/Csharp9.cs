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

    // pattern matching lists

    public class Car
    {
        public int Passengers { get; set; }
    }

    public class DeliveryTruck
    {
        public int GrossWeightClass { get; set; }
    }

    public class Taxi
    {
        public int Fares { get; set; }
    }

    public class Bus
    {
        public int Capacity { get; set; }
        public int Riders { get; set; }
    }

    public void PatternMatching()
    {
        var vehicles = new object[] { new Car { Passengers = 2 }, new DeliveryTruck { GrossWeightClass = 4 }, new Taxi { Fares = 4 }, new Bus { Capacity = 30, Riders = 15 } };

        foreach (var vehicle in vehicles)
        {
            Console.WriteLine(vehicle switch
            {
                Car { Passengers: 1 } => "A car with one passenger",
                Car { Passengers: 2 } => "A car with two passengers",
                DeliveryTruck t => $"A delivery truck with gross weight class {t.GrossWeightClass}",
                Taxi t => $"A taxi with {t.Fares} fares",
                Bus { Capacity: var c, Riders: var r } when r > c => $"A bus with {r} riders, over capacity by {r - c}",
                Bus { Capacity: var c, Riders: var r } => $"A bus with {r} riders",
                _ => "Some other vehicle"
            });
        }
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

    // list patterns
    public void ListPatterns()
    {
        decimal balance = 0m;
        string[][] transactions = new[]
        {
            new[] { "1", "DEPOSIT", "100.00" },
            new[] { "2", "WITHDRAWAL", "20.00" },
            new[] { "3", "INTEREST", "0.50" },
            new[] { "4", "FEE", "1.00" },
        };
        foreach (string[] transaction in transactions)
        {
            balance += transaction switch
            {
            [_, "DEPOSIT", _, var amount] => decimal.Parse(amount),
            [_, "WITHDRAWAL", .., var amount] => -decimal.Parse(amount),
            [_, "INTEREST", var amount] => decimal.Parse(amount),
            [_, "FEE", var fee] => -decimal.Parse(fee),
                _ => throw new InvalidOperationException($"Record {string.Join(", ", transaction)} is not in the expected format!"),
            };
            Console.WriteLine($"Record: {string.Join(", ", transaction)}, New balance: {balance:C}");
        }
    }

    // declaration patterns
    public void DeclarationPatterns()
    {
        var y = new object();
        var x = y is Car car ? car.ToString() : "null";
    }
}
