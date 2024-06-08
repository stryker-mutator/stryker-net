namespace TargetProject.Constructs;

public class Csharp4
{
    // named parameters
    public static void NamedParameters()
    {
        Console.WriteLine(format: "{0:f}", arg0: 6.02214179e23);
        Console.WriteLine(arg0: 6.02214179e23, format: "{0:f}");
    }
}
