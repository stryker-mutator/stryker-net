using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace TargetProject.Constructs;

public class Csharp11
{
    // genergic attributes
    public class GenericAttribute<T> : Attribute { }

    [GenericAttribute<string>()]
    public string Method() => default;

    // static virtual members in interfaces
    public interface IGetNext<T> where T : IGetNext<T>
    {
        static abstract T operator ++(T other);
    }

    // virtual members in structs
    public struct RepeatSequence : IGetNext<RepeatSequence>
    {
        private const char Ch = 'A';
        public string Text = new string(Ch, 1);

        public RepeatSequence() { }

        public static RepeatSequence operator ++(RepeatSequence other)
            => other with { Text = other.Text + Ch };

        public override string ToString() => Text;
    }

    public static void VitualMembers()
    {
        var str = new RepeatSequence();

        for (var i = 0; i < 10; i++)
            Console.WriteLine(str++);
    }

    // generic math
    public record Translation<T>(T XOffset, T YOffset) where T : IAdditionOperators<T, T, T>;

    public record Point<T>(T X, T Y) where T : IAdditionOperators<T, T, T>
    {
        public static Point<T> operator +(Point<T> left, Translation<T> right) =>
        left with { X = left.X + right.XOffset, Y = left.Y + right.YOffset };
    }

    public static string GenericMath()
    {
        var point = new Point<BigInteger>(1, 2);
        var translation = new Translation<BigInteger>(3, 4);

        var newPoint = point + translation;

        return $"({newPoint.X}, {newPoint.Y})";
    }

    // patern matching lists
    public static void PatternMatching()
    {
        int[] numbers = { 1, 2, 3 };

        Console.WriteLine(new[] { 1, 2, 3, 4, 5 } is [> 0, > 0, ..]);  // True
        Console.WriteLine(new[] { 1, 1 } is [_, _, ..]);  // True
        Console.WriteLine(new[] { 0, 1, 2, 3, 4 } is [> 0, > 0, ..]);  // False
        Console.WriteLine(new[] { 1 } is [1, 2, ..]);  // False

        Console.WriteLine(new[] { 1, 2, 3, 4 } is [.., > 0, > 0]);  // True
        Console.WriteLine(new[] { 2, 4 } is [.., > 0, 2, 4]);  // False
        Console.WriteLine(new[] { 2, 4 } is [.., 2, 4]);  // True

        Console.WriteLine(new[] { 1, 2, 3, 4 } is [>= 0, .., 2 or 4]);  // True
        Console.WriteLine(new[] { 1, 0, 0, 1 } is [1, 0, .., 0, 1]);  // True
        Console.WriteLine(new[] { 1, 0, 1 } is [1, 0, .., 0, 1]);  // False
    }

    // raw string literals
    public static void StringLiterals()
    {
        var longMessage = """
    This is a long message.
    It has several lines.
        Some are indented
                more than others.
    Some should start at the first column.
    Some have "quoted text" in them.
    """;
    }

    // UTF-8 string literals

    public static void Utf8StringLiterals()
    {
        byte[] AuthStringLiteral = "AUTH "u8.ToArray();
        var helloWorld = "Hello"u8 + " "u8 + "World"u8;
        var empty = ""u8;
    }

    // required members
    public class Person
    {
        public Person() { }

        [SetsRequiredMembers]
        public Person(string firstName) => FirstName = firstName;

        public required string FirstName { get; init; }

        // Omitted for brevity.
    }

    public static void RequiredMembers()
    {
        var person = new Person("John");
        person = new Person { FirstName = "John" };
        // Error CS9035: Required member `Person.FirstName` must be set:
        //person = new Person();
    }

    // nameof attributes
    [ParameterString(nameof(msg))]
    public static void Method(string msg)
    {
        [ParameterString(nameof(T))]
        void LocalFunction<T>(T param)
        { }

        var lambdaExpression = ([ParameterString(nameof(aNumber))] int aNumber) => aNumber.ToString();
    }

    // verbatim interpolated strings
    public static void VerbatimInterpolatedStrings()
    {
        var name = "John";
        var message = $@"Hello, {name}";
        Console.WriteLine(message);
    }

    private class ParameterStringAttribute : Attribute
    {
        public ParameterStringAttribute(string name) { }
    }
}

// file type
file interface IWidget
{
    int ProvideAnswer();
}

file class HiddenWidget
{
    public int Work() => 42 + 0;
}

public class Widget : IWidget
{
    public int ProvideAnswer()
    {
        var worker = new HiddenWidget();
        return worker.Work();
    }
}
