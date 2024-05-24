namespace TargetProject;

public  class Csharp1
{
    // string interpolation
    public static void StringInterpolation()
    {
        string name = "John";
        string message = $"Hello, {name}!";
        Console.WriteLine(message);
    }

    // string concatenation
    public static void StringConcatenation()
    {
        string firstName = "John";
        string lastName = "Doe";
        string message = "Hello, " + firstName + " " + lastName + "!";
        Console.WriteLine(message);
    }

    // delegates
    public delegate void Print(string message = "");

    public static void Delegates()
    {
        Print print = (message) => Console.WriteLine("printing: " + message);
        print("Hello, World!");
    }
}
