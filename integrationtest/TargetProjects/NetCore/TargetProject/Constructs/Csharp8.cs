using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace TargetProject.Constructs
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
            var start = 1;
            var amountToTake = 3;
            var subset = numbers[start..(start + amountToTake)];
            Display(subset);  // output: 10 20 30

            var margin = 1;
            var inner = numbers[margin..^margin];
            Display(inner);  // output: 10 20 30 40

            var line = "one two three";
            var amountToTakeFromEnd = 5;
            var endIndices = ^amountToTakeFromEnd..^0;
            var end = line[endIndices];
            Display(end);  // output: three

            var amountToDrop = numbers.Length / 2;

            var rightHalf = numbers[amountToDrop..];
            Display(rightHalf);  // output: 30 40 50

            var leftHalf = numbers[..^amountToDrop];
            Display(leftHalf);  // output: 0 10 20

            var all = numbers[..];
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
