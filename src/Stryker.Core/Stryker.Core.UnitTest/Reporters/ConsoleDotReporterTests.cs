using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ConsoleDotReporterTests
    {

        [Theory]
        [InlineData(MutantStatus.Killed, ".", "default")]
        [InlineData(MutantStatus.RuntimeError, "E", "red")]
        [InlineData(MutantStatus.Survived, "S", "red")]
        [InlineData(MutantStatus.Timeout, "T", "red")]
        public void ConsoleDotReporter_ShouldPrintRightCharOnMutation(MutantStatus givenStatus, string expectedOutput, string color)
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Red(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });
            var target = new ConsoleDotProgressReporter(chalkMock.Object);

            target.OnMutantTested(new Mutant()
            {
                ResultStatus = givenStatus
            });
            if (color == "default")
                chalkMock.Verify(x => x.Default(It.IsAny<string>()));
            if (color == "red")
                chalkMock.Verify(x => x.Red(It.IsAny<string>()));
            output.ShouldBe(expectedOutput);
        }
    }
}
