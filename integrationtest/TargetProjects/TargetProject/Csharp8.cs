using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace ExampleProject
{
    public class Csharp8
    {
        // switch expressions
        public static string RockPaperScissors(string first, string second) => (first, second) switch
            {
                ("rock", "paper") => 1 > 2 ? "rock is covered by paper. Paper wins." : "",
                (_, _) => "tie"
            };

        public void SwitchExpression()
        {
            _ = ("rock", "paper") switch
            {
                ("rock", "paper") => RockPaperScissors("", ""),
                (_, _) => "tie"
            };
        }

        // using statements
        public void UsingStatements()
        {
            using var metric = new Meter("metric");
            metric.ToString();
        }

        // nullable reference types
        private string? detailedDescription;

        // range operator
        public static void Range()
        {
            int[] numbers = [0, 10, 20, 30, 40, 50];
            int start = 1;
            int amountToTake = 3;
            int[] subset = numbers[start..(start + amountToTake)];
            Display(subset);  // output: 10 20 30

            int margin = 1;
            int[] inner = numbers[margin..^margin];
            Display(inner);  // output: 10 20 30 40

            string line = "one two three";
            int amountToTakeFromEnd = 5;
            Range endIndices = ^amountToTakeFromEnd..^0;
            string end = line[endIndices];
            Display(end);  // output: three

            int amountToDrop = numbers.Length / 2;

            int[] rightHalf = numbers[amountToDrop..];
            Display(rightHalf);  // output: 30 40 50

            int[] leftHalf = numbers[..^amountToDrop];
            Display(leftHalf);  // output: 0 10 20

            int[] all = numbers[..];
            Display(all);  // output: 0 10 20 30 40 50

            void Display<T>(IEnumerable<T> xs) => Console.WriteLine(string.Join(" ", xs));
        }
    }

    public class GenericClass<T1>
    {
        public bool TryGet<T2>([NotNullWhen(true)] out T2? result) where T2 : class, T1
        {
            result = null;
            return false;
        }
    }
}
