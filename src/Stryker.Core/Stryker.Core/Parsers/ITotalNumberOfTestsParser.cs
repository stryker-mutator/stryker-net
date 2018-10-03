namespace Stryker.Core.Parsers
{
    public interface ITotalNumberOfTestsParser
    {
        int ParseTotalNumberOfTests(string testProcessRunnerOutput);
    }
}