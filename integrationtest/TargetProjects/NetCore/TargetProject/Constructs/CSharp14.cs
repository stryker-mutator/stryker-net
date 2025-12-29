using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace TargetProject.Constructs;

// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14
public static class Enumerable
{
    // Extension block
    extension<TSource>(IEnumerable<TSource> source) // extension members for IEnumerable<TSource>
    {
        // Extension property:
        public bool IsEmpty => !source.Any();

        // Extension method:
        public IEnumerable<TSource> Where(Func<TSource, bool> predicate) => [];
    }

    // extension block, with a receiver type only
    extension<TSource>(IEnumerable<TSource>) // static extension members for IEnumerable<Source>
    {
        // static extension method:
        public static IEnumerable<TSource> Combine(IEnumerable<TSource> first, IEnumerable<TSource> second) => [];

        // static extension property:
        public static IEnumerable<TSource> Identity => [];

        // static user defined operator:
        public static IEnumerable<TSource> operator +(IEnumerable<TSource> left, IEnumerable<TSource> right) => left.Concat(right);
    }
}

public class Example2
{
    // Field keyword
    public string Message
    {
        get;
        set => field = value ?? throw new ArgumentNullException(nameof(value));
    }

    // nameof generic unbound
    public void LogMessage()
    {
        Console.WriteLine($"{nameof(IEnumerable<>)} this should work");
    }

    // Null conditional assignment
    public void EnsureListInitialized(Example2 example)
    {
        example?.Message = "Hello, World!";
    }
}
