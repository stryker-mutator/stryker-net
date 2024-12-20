using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TargetProject.Constructs;

// https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13
public class CSharp13
{
    public void Concat<T>(params ReadOnlySpan<T> items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            Console.Write(items[i]);
            Console.Write(" ");
        }
        Console.WriteLine();
    }

    public void SystemThreadingLock()
    {
        var lockObject = new Lock();
        lock (lockObject)
        {
            Console.WriteLine("Locked");
        }
    }

    public void EscapeCharacterLiteral()
    {
        var escapeLiteralRegex = new Regex("This is a text containing the literal ESCAPE character \e, not to be confused with AN escape character");
    }

    public void ImplicitIndexOperatorInObjectInitializer()
    {
        var countdown = new TimerRemaining()
        {
            buffer =
            {
                [^1] = 0,
                [^2] = 1,
                [^3] = 2,
                [^4] = 3,
                [^5] = 4,
                [^6] = 5,
                [^7] = 6,
                [^8] = 7,
                [^9] = 8,
                [^10] = 9
            }
        };
    }

    private int highscoreValue = 5;

    public IEnumerable<int> RefAndUnsafeIterator()
    {
        int x;
        unsafe
        {
            x = 10;
        }
        ref int highscore = ref highscoreValue;

        yield return highscore;
        yield return x;
    }

    public async Task<int> RefAndUnsafeAsync()
    {
        int x;
        unsafe
        {
            x = 10;
        }

        ref int highscore = ref highscoreValue;

        return x + highscore;
    }
}

public class TimerRemaining
{
    public int[] buffer { get; set; } = new int[10];
}

public interface I { };

public class C<T> : I where T : allows ref struct
{
    // Use T as a ref struct:
    public void M(scoped T p)
    {
        // The parameter p must follow ref safety rules
    }
}

public partial class C
{
    // Declaring declaration
    public partial string Name { get; set; }
}

public partial class C
{
    // implementation declaration:
    private string _name;
    public partial string Name
    {
        get => _name;
        set => _name = value;
    }
}
