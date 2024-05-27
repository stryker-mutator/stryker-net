using System.Linq;

namespace Test
{
    public class StringReplaceChain
    {
        public string ExampleBugMethod()
        {
            string someString = "";
            return someString.Replace("ab", "cd")
                .Replace("12", "34")
                .PadLeft(12)
                .Replace("12", "34");
        }

        public char ExampleChain() => "test ".ToUpper().Trim().PadLeft(2).Substring(2).ElementAt(2);
    }
}
