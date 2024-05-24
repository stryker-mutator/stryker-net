namespace TargetProject.Defects
{
    public class StringReplaceChain
    {
        public string ExampleBugMethod()
        {
            var someString = "";
            return someString.Replace("ab", "cd")
                .Replace("12", "34")
                .PadLeft(12)
                .Replace("12", "34");
        }
    }
}
