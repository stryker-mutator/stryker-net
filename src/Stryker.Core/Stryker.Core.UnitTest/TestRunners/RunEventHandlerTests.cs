using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners.VsTest;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    public class RunEventHandlerTests
    {
        [Fact]
        public void ShouldNotSupportDebugging()
        {
            var tests = new Dictionary<Guid, VsTestDescription>();
            var sut = new RunEventHandler(tests, TestGuidsList.EveryTest(), null, "test");

            Assert.Throws<NotSupportedException>(() => sut.LaunchProcessWithDebuggerAttached(null));
        }

        [Fact]
        public void ShouldForwardLogMessages()
        {
            var tests = new Dictionary<Guid, VsTestDescription>();
            var mockLogger = new Mock<ILogger>();
            var sut = new RunEventHandler(tests, TestGuidsList.EveryTest(), mockLogger.Object, "test");

            sut.HandleLogMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel.Informational,
                "hello");
            sut.HandleLogMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel.Warning,
                "hello");
            sut.HandleLogMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel.Error,
                "hello");
            // we just check the log level
            mockLogger.Verify(m => m.Log<It.IsAnyType>(LogLevel.Trace,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(), null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(3));

        }
    }
}
