using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class BasePathInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new BasePathInput();
            target.HelpText.ShouldBe(@$"The path from which stryker is started.");
        }

        [TestMethod]
        public void ShouldAllowExistingDir()
        {
            var target = new BasePathInput { SuppliedInput = "C:/MyDir/" };
            var fileSystemMock = new MockFileSystem();
            fileSystemMock.AddDirectory("C:/MyDir/");

            var result = target.Validate(fileSystemMock);

            result.ShouldBe("C:/MyDir/");
        }

        [TestMethod]
        public void ShouldThrowOnNonexistentDir()
        {
            var target = new BasePathInput { SuppliedInput = "C:/MyDir/" };
            var fileSystemMock = new MockFileSystem();

            var exception = Should.Throw<InputException>(() => target.Validate(fileSystemMock));

            exception.Message.ShouldBe("Base path must exist.");
        }

        [TestMethod]
        public void ShouldThrowOnNull()
        {
            var target = new BasePathInput { SuppliedInput = null };
            var fileSystemMock = new MockFileSystem();

            var exception = Should.Throw<InputException>(() => target.Validate(fileSystemMock));

            exception.Message.ShouldBe("Base path can't be null or empty.");
        }
    }
}
