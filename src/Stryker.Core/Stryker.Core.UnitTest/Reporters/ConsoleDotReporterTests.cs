using Shouldly;
using Spectre.Console.Testing;
using Stryker.Configuration.Mutants;
using Stryker.Configuration.Reporters;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Reporters
{
    [TestClass]
    public class ConsoleDotReporterTests : TestBase
    {
        [TestMethod]
        [DataRow(MutantStatus.Killed, ".", "default")]
        [DataRow(MutantStatus.Survived, "S", "red")]
        [DataRow(MutantStatus.Timeout, "T", "default")]
        public void ConsoleDotReporter_ShouldPrintRightCharOnMutation(MutantStatus givenStatus, string expectedOutput, string color)
        {
            var console = new TestConsole().EmitAnsiSequences();
            var target = new ConsoleDotProgressReporter(console);

            target.OnMutantTested(new Mutant()
            {
                ResultStatus = givenStatus
            });

            if (color == "default")
            {
                console.Output.AnyForegroundColorSpanCount().ShouldBe(0);
            }

            if (color == "red")
            {
                console.Output.RedSpanCount().ShouldBe(1);
            }

            console.Output.RemoveAnsi().ShouldBe(expectedOutput);
        }
    }
}
