using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

// static imports
using static System.Math;

namespace TargetProject.Constructs;

public class Csharp6
{
    // nameof expressions
    public static void NameofExpressions()
    {
        Console.WriteLine(nameof(System.Collections.Generic));  // output: Generic
        Console.WriteLine(nameof(List<int>));  // output: List
        Console.WriteLine(nameof(List<int>.Count));  // output: Count
        Console.WriteLine(nameof(List<int>.Add));  // output: Add

        var numbers = new List<int>() { 1, 2, 3 };
        Console.WriteLine(nameof(numbers));  // output: numbers
        Console.WriteLine(nameof(numbers.Count));  // output: Count
        Console.WriteLine(nameof(numbers.Add));  // output: Add

        var @new = 5;
        Console.WriteLine(nameof(@new));
    }

    private string name;
    public string Name
    {
        get => name;
        set => name = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Name)} cannot be null");
    }

    // expression-bodied members
    public override string ToString() => $"{Name} {Name}".Trim();

    // exception filters
    public static async Task<string> MakeRequest()
    {
        var client = new HttpClient();
        var streamTask = client.GetStringAsync("https://localHost:10000");
        try
        {
            var responseText = await streamTask;
            return responseText;
        }
        catch (HttpRequestException e) when (e.Message.Contains("301"))
        {
            return "Site Moved";
        }
        catch (HttpRequestException e) when (e.Message.Contains("404"))
        {
            return "Page Not Found";
        }
        catch (HttpRequestException e)
        {
            return e.Message;
        }
    }

    // static imports
    public static void StaticImports()
    {
        Floor(5.5);  // output: 5
    }

    // null-conditional operators
    public static string GetFirstCharacter(string input)
    {
        return input?[0].ToString() ?? string.Empty;
    }
}
