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
    }
}
