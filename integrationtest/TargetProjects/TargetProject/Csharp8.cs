using System.Diagnostics.CodeAnalysis;

namespace ExampleProject
{
    public class Csharp8
    {
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
    }

    public class GenericClass<T1>
    {
        public bool TryGet<T2>([NotNullWhen(true)] out T2? result) where T2 : class
        {
            result = null;
            return false;
        }
    }
}
