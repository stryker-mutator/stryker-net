using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TargetProject.Constructs;

public class Csharp5
{
    // caller information
    public static void ValidateArgument(string parameterName, bool condition, [CallerArgumentExpression("condition")] string? message = null)
    {
        if (!condition)
        {
            throw new ArgumentException($"Argument failed validation: <{message}>", parameterName);
        }
    }

    public static void Test()
    {
        ShowCallerInfo();
    }
    public static void ShowCallerInfo([CallerMemberName] string callerName = null,
        [CallerFilePath] string callerFilePath = null,
        [CallerLineNumber] int callerLine = -1)
    {
        Console.WriteLine("Caller Name: {0}", callerName);
        Console.WriteLine("Caller FilePath: {0}", callerFilePath);
        Console.WriteLine("Caller Line number: {0}", callerLine);
    }

    // async await

    // These classes are intentionally empty for the purpose of this example. They are simply marker classes for the purpose of demonstration, contain no properties, and serve no other purpose.
    internal class Bacon { }
    internal class Coffee { }
    internal class Egg { }
    internal class Juice { }
    internal class Toast { }

    static async Task Main(string[] args)
    {
        var cup = PourCoffee();
        Console.WriteLine("coffee is ready");

        var eggsTask = FryEggsAsync(2);
        var baconTask = FryBaconAsync(3);
        var toastTask = MakeToastWithButterAndJamAsync(2);

        var breakfastTasks = new List<Task> { eggsTask, baconTask, toastTask };
        while (breakfastTasks.Count > 0)
        {
            var finishedTask = await Task.WhenAny(breakfastTasks);
            if (finishedTask == eggsTask)
            {
                Console.WriteLine("eggs are ready");
            }
            else if (finishedTask == baconTask)
            {
                Console.WriteLine("bacon is ready");
            }
            else if (finishedTask == toastTask)
            {
                Console.WriteLine("toast is ready");
            }
            await finishedTask;
            breakfastTasks.Remove(finishedTask);
        }

        var oj = PourOJ();
        Console.WriteLine("oj is ready");
        Console.WriteLine("Breakfast is ready!");
    }

    static async Task<Toast> MakeToastWithButterAndJamAsync(int number)
    {
        var toast = await ToastBreadAsync(number);
        ApplyButter(toast);
        ApplyJam(toast);

        return toast;
    }

    private static Juice PourOJ()
    {
        Console.WriteLine("Pouring orange juice");
        return new Juice();
    }

    private static void ApplyJam(Toast toast) =>
        Console.WriteLine("Putting jam on the toast");

    private static void ApplyButter(Toast toast) =>
        Console.WriteLine("Putting butter on the toast");

    private static async Task<Toast> ToastBreadAsync(int slices)
    {
        for (var slice = 0; slice < slices; slice++)
        {
            Console.WriteLine("Putting a slice of bread in the toaster");
        }
        Console.WriteLine("Start toasting...");
        await Task.Delay(3000);
        Console.WriteLine("Remove toast from toaster");

        return new Toast();
    }

    private static async Task<Bacon> FryBaconAsync(int slices)
    {
        Console.WriteLine($"putting {slices} slices of bacon in the pan");
        Console.WriteLine("cooking first side of bacon...");
        await Task.Delay(3000);
        for (var slice = 0; slice < slices; slice++)
        {
            Console.WriteLine("flipping a slice of bacon");
        }
        Console.WriteLine("cooking the second side of bacon...");
        await Task.Delay(3000);
        Console.WriteLine("Put bacon on plate");

        return new Bacon();
    }

    private static async Task<Egg> FryEggsAsync(int howMany)
    {
        Console.WriteLine("Warming the egg pan...");
        await Task.Delay(3000);
        Console.WriteLine($"cracking {howMany} eggs");
        Console.WriteLine("cooking the eggs ...");
        await Task.Delay(3000);
        Console.WriteLine("Put eggs on plate");

        return new Egg();
    }

    private static Coffee PourCoffee()
    {
        Console.WriteLine("Pouring coffee");
        return new Coffee();
    }
}
