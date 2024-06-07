using System.Linq;

public class StringMethods
{
    public char ExampleChain() => "test ".ToUpper().Trim().PadLeft(2).Substring(2).ElementAt(0);

    public char ExampleChain2()
    {
        var test = "test ";
        return test.ToUpper().Trim().PadLeft(2).Substring(2).ElementAt(0);
    }
}
