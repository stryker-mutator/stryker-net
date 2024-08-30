namespace TargetProject.Constructs;

public class Csharp4
{
    // named parameters
    public static void NamedParameters()
    {
        Console.WriteLine(format: "{0:f}", arg0: 6.02214179e23);
        Console.WriteLine(arg0: 6.02214179e23, format: "{0:f}");
    }

    // default parameters
    public static void DefaultParameters()
    {
        ExampleDefaultParameterMethod("Hello");
        ExampleDefaultParameterMethod("Hello", "World");
        ExampleDefaultParameterMethod("Hello", "World", "Foo");
        ExampleDefaultParameterMethod("Hello", "World", "Foo", "Bar");
    }

    // default named parameters
    public static void DefaultNamedParameters()
    {
        ExampleDefaultParameterMethod(required1: "Hello");
        ExampleDefaultParameterMethod(optional3: "Hello", required1: "World");
        ExampleDefaultParameterMethod(optional2: "Hello", required1: "World",  optional1: "Foo");
        ExampleDefaultParameterMethod(optional3: "Hello", optional2: "World", required1: "Foo", optional1: "Bar");
    }

    private static void ExampleDefaultParameterMethod(string required1, string optional1 = "optional1",
        string optional2 = "optional2", string optional3 = "optional3")
    {
    }
}
