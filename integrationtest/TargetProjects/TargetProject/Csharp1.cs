namespace TargetProject;

public  class Csharp1
{
    // delegates
    public delegate void Print(string message = "");

    public static void Delegates()
    {
        Print print = (message) => Console.WriteLine("printing: " + message);
        print("Hello, World!");
    }

}
