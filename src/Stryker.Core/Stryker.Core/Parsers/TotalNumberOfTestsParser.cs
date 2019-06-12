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

        public int ParseTotalNumberOfTests(string testProcessRunnerOutput)
        {
            var testProcessRunnerOutputSpan = testProcessRunnerOutput.AsSpan();

            var tokenIndex = testProcessRunnerOutputSpan.IndexOf(TotalTestsStartToken.AsSpan());

            if (tokenIndex == -1)
            {
                return 0;
            }

            var endOfStartTokenIndex = tokenIndex + TotalTestsStartToken.Length;

            var firstNonWhiteSpaceIndex = IndexOfFirstNonWhiteSpace(testProcessRunnerOutputSpan.Slice(endOfStartTokenIndex));

            if (firstNonWhiteSpaceIndex == -1)
            {
                return 0;
            }

            var firstDigitIndex = endOfStartTokenIndex + firstNonWhiteSpaceIndex;

            var firstNonDigitIndex = IndexOfFirstNonDigit(testProcessRunnerOutputSpan.Slice(firstDigitIndex));

            ReadOnlySpan<char> totalTestsString;
            if (firstNonDigitIndex == -1)
            {
                totalTestsString = testProcessRunnerOutputSpan.Slice(firstDigitIndex);
            }
            else
            {
                totalTestsString = testProcessRunnerOutputSpan.Slice(firstDigitIndex, firstNonDigitIndex);
            }

            if (int.TryParse(totalTestsString.ToString(), out var totalTests))
            {
                return totalTests;
            }

            return 0;
        }

        private int IndexOfFirstNonWhiteSpace(ReadOnlySpan<char> text)
        {
            for (int i = 0; i < text.Length; ++i)
            {
                if (!char.IsWhiteSpace(text[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private int IndexOfFirstNonDigit(ReadOnlySpan<char> text)
        {
            for (int i = 0; i < text.Length; ++i)
            {
                if (!char.IsDigit(text[i]))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}