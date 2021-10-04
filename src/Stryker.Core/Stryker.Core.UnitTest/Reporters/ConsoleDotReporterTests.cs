using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters;
using System.IO;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ConsoleDotReporterTests : TestBase
    {
        [Theory]
        [InlineData(MutantStatus.Killed, ".", "default")]
        [InlineData(MutantStatus.Survived, "S", "red")]
        [InlineData(MutantStatus.Timeout, "T", "default")]
        public void ConsoleDotReporter_ShouldPrintRightCharOnMutation(MutantStatus givenStatus, string expectedOutput, string color)
        {
            var textWriter = new StringWriter();
            var target = new ConsoleDotProgressReporter(textWriter);

            target.OnMutantTested(new Mutant()
            {
                ResultStatus = givenStatus
            });

            if (color == "default")
            {
                textWriter.AnyForegroundColorSpanCount().ShouldBe(0);
            }

            if (color == "red")
            {
                textWriter.RedSpanCount().ShouldBe(1);
            }

            textWriter.RemoveAnsi().ShouldBe(expectedOutput);
        }
    }
}
