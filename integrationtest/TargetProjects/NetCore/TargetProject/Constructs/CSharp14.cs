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
    extension<T>(IEnumerable<T> source)
        where T : IEquatable<T>
    {
        public IEnumerable<T> ValuesEqualTo(T threshold)
            => source.Where(x => x.Equals(threshold));
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
