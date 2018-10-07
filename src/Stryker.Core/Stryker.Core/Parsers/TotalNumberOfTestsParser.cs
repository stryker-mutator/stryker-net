using System;

namespace Stryker.Core.Parsers
{
    public interface ITotalNumberOfTestsParser
    {
        int ParseTotalNumberOfTests(string testProcessRunnerOutput);
    }

    public class TotalNumberOfTestsParser : ITotalNumberOfTestsParser
    {
        private const string TotalTestsStartToken = "Total tests:";
        private const string TotalTestsEndToken = ".";

        public int ParseTotalNumberOfTests(string testProcessRunnerOutput)
        {
            var tokenIndex = testProcessRunnerOutput.AsSpan().IndexOf(TotalTestsStartToken.AsSpan());

            if (tokenIndex == -1)
            {
                return 0;
            }

            var endOfStartTokenIndex = tokenIndex + TotalTestsStartToken.Length;

            var endTokenIndex = testProcessRunnerOutput.AsSpan().Slice(endOfStartTokenIndex).IndexOf(TotalTestsEndToken.AsSpan());

            var totalTestsString = testProcessRunnerOutput.AsSpan().Slice(endOfStartTokenIndex, endTokenIndex);

            if (int.TryParse(totalTestsString.ToString(), out var totalTests))
            {
                return totalTests;
            }

            return 0;
        }
    }
}