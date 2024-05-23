using Shouldly;
using Spectre.Console.Testing;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters;
using Stryker.Shared.Mutants;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters;

public class ConsoleDotReporterTests : TestBase
{
    [Theory]
    [InlineData(MutantStatus.Killed, ".", "default")]
    [InlineData(MutantStatus.Survived, "S", "red")]
    [InlineData(MutantStatus.Timeout, "T", "default")]
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
