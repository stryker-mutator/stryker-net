using System;
using Stryker.Core.Parsers;
using Stryker.Core.Testing;

namespace Stryker.Core.Logging.TotalNumberOfTests
{
    public class TotalNumberOfTestsLogger : ITotalNumberOfTestsLogger
    {
        private readonly IChalk _chalk;
        private readonly ITotalNumberOfTestsParser _totalNumberOfTestsParser;

        public TotalNumberOfTestsLogger(IChalk chalk, ITotalNumberOfTestsParser totalNumberOfTestsParser)
        {
            _chalk = chalk;
            _totalNumberOfTestsParser = totalNumberOfTestsParser;
        }

        public void LogTotalNumberOfTests(string testRunOutput)
        {
            var totalNumberOfTests = _totalNumberOfTestsParser.ParseTotalNumberOfTests(testRunOutput);

            _chalk.Default($"Total number of tests found in initial test run: {totalNumberOfTests} {Environment.NewLine}");
        }
    }
}