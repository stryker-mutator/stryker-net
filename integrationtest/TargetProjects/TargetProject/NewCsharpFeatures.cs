using System.Diagnostics.CodeAnalysis;

namespace ExampleProject
{
    public class NewCsharpFeatures
    {
        public static string RockPaperScissors(string first, string second)
            => (first, second) switch
            {
                ("rock", "paper") => 1 > 2 ? "rock is covered by paper. Paper wins." : "",
                (_, _) => "tie"
            };

        public double GetDefaultDoubleValue()
        {
            var outcome = ("rock", "paper") switch
            {
                ("rock", "paper") => RockPaperScissors("", ""),
                (_, _) => "tie"
            };
            return default;
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
