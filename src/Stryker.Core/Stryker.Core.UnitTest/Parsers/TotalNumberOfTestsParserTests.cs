using Stryker.Core.Parsers;
using Xunit;

namespace Stryker.Core.UnitTest.Parsers
{
    public class TotalNumberOfTestsParserTests
    {
        [Theory]
        [InlineDataAttribute("155")]
        [InlineDataAttribute("0")]
        [InlineDataAttribute("10221")]
        public void TotalNumberOfTestsParser_ShouldReturnTotalNumberOfTests_WhenCorrectOutputFromTestsIsGiven(
            string numberOfTests)
        {
            var totalNumberOfTestsParser = new TotalNumberOfTestsParser();

            var outputFromTestRun =
                $@"Test run for *\Stryker.Core.UnitTest\bin\Debug\netcoreapp2.0\Stryker.Core.UnitTest.dll(.NETCoreApp,Version=v2.0)
            Microsoft(R) Test Execution Command Line Tool Version 15.8.0
            Copyright(c) Microsoft Corporation.  All rights reserved.

                Starting test execution, please wait...

                                                Total tests: {numberOfTests}.Passed: 1595.Failed: 0.Skipped: 0.
                Test Run Successful.
                Test execution time: 6,1493 Seconds
            ";

            var result = totalNumberOfTestsParser.ParseTotalNumberOfTests(outputFromTestRun);
            var expected = int.Parse(numberOfTests);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TotalNumberOfTestsParser_ShouldReturnZero_WhenPassedParameterDoesNotContainTotalNumberOfTests()
        {
            var totalNumberOfTestsParser = new TotalNumberOfTestsParser();

            var outputFromTestRun =
                $@"Test run for *\Stryker.Core.UnitTest\bin\Debug\netcoreapp2.0\Stryker.Core.UnitTest.dll(.NETCoreApp,Version=v2.0)
         
                Test execution time: 6,1493 Seconds
            ";

            var result = totalNumberOfTestsParser.ParseTotalNumberOfTests(outputFromTestRun);

            Assert.Equal(0, result);
        }
    }
}