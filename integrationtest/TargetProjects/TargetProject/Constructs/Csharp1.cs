using System.Linq;

namespace TargetProject.Constructs;

public class Csharp1
{
    // string interpolation
    public static void StringInterpolation()
    {
        var name = "John";
        var message = $"Hello, {name}!";
        Console.WriteLine(message);
    }

    // string concatenation
    public static void StringConcatenation()
    {
        var firstName = "John";
        var lastName = "Doe";
        var message = "Hello, " + firstName + " " + lastName + "!";
        Console.WriteLine(message);
    }

    // string formatting
    public static void StringFormatting()
    {
        var name = "John";
        var message = string.Format("Hello, {0}!", name);
        Console.WriteLine(message);
    }

    // multiline strings
    public static void MultilineStrings()
    {
        var message = @"Hello,

world!";
        Console.WriteLine(message);
    }

    // delegates
    public delegate void Print(string message = "");

    public static void Delegates()
    {
        Print print = (message) => Console.WriteLine("printing: " + message);
        print("Hello, World!");
    }

    // string methods
    public char ExampleChain() => "test".ToUpper().Trim().PadLeft(2).Substring(2).ElementAt(0);
}
