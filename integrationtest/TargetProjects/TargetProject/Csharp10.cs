// global usings
global using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetProject;

public  class Csharp10
{
    // record structs
    public record Person(string FirstName, string LastName);
    public record Person2
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public Person Dad { get; init; }
    };
    public readonly record struct Point(double X, double Y, double Z);
    public record struct Point2
    {
        public double X { get; init; }
        public double Y { get; init; }
        public double Z { get; init; }
    }
    public record Person3
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    };
    public record struct DataMeasurement(DateTime TakenAt, double Measurement);
    public static void RecordStructs()
    {
        var person = new Person("John", "Doe");
        var person2 = new Person2 { FirstName = "John", LastName = "Doe" };
        var point = new Point(1, 2, 3);
        var point2 = new Point2 { X = 1, Y = 2, Z = 3 };
        var person3 = new Person3 { FirstName = "John", LastName = "Doe" };
        var data = new DataMeasurement(DateTime.Now, 3.14);
    }

    // extended property patterns
    public static void ExtendedPropertyPatterns()
    {
        var person2 = new Person2 { FirstName = "John", LastName = "Doe" };
        if (person2 is Person2 { Dad.FirstName: "John" })
        {
            Console.WriteLine("John");
        }
    }

    // constant interpolated strings
    public static void ConstantInterpolatedStrings()
    {
        const string name = "John";
        const string message = $"Hello, {name}";
        Console.WriteLine(message);
    }

    // assignment in a deconstruction
    public static void AssignmentInADeconstruction(Point point)
    {
        double x = 0;
        (x, double y, double z) = point;
    }

    // line pragmas
    public static void LinePragmas()
    {
#line 200 "Special"
            int i;
            int j;
#line default
            char c;
            float f;
#line hidden // numbering not affected
            string s;
            double d;
    }
}
