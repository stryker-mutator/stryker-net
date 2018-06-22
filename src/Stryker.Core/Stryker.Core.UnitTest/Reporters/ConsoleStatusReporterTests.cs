using Moq;
using Shouldly;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ConsoleStatusReporterTests
    {

        [Fact]
        public void ConsoleStatusReporter_OnInitialisationStarted()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });

            var target = new ConsoleStatusReporter(chalkMock.Object);

            target.OnInitialisationStarted();

            chalkMock.Verify(x => x.Default(It.IsAny<string>()));
            output.ShouldBeWithNewlineReplace(@"Analyzing project
");
        }

        [Fact]
        public void ConsoleStatusReporter_OnInitialBuildStarted()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });

            var target = new ConsoleStatusReporter(chalkMock.Object);

            target.OnInitialBuildStarted();

            chalkMock.Verify(x => x.Default(It.IsAny<string>()));
            output.ShouldBeWithNewlineReplace(@"Building project
");
        }

        [Fact]
        public void ConsoleStatusReporter_OnInitialTestRunStarted()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });

            var target = new ConsoleStatusReporter(chalkMock.Object);

            target.OnInitialTestRunStarted();

            chalkMock.Verify(x => x.Default(It.IsAny<string>()));
            output.ShouldBeWithNewlineReplace(@"Starting initial testrun
");
        }

        [Fact]
        public void ConsoleStatusReporter_OnInitialisationDone()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });

            var target = new ConsoleStatusReporter(chalkMock.Object);

            target.OnInitialisationDone();

            chalkMock.Verify(x => x.Default(It.IsAny<string>()));
            output.ShouldBeWithNewlineReplace(@"Project OK
Generating mutants
");
        }
    }
}
