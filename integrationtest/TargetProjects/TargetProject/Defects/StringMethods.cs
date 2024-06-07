using System.Linq;

public class StringMethods
{
    public char ExampleChain() => "test".ToUpper().Trim().PadLeft(2).Substring(2).ElementAt(0);
}
