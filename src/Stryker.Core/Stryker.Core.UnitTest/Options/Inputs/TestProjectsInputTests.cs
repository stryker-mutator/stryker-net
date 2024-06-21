using System.IO;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class TestProjectsInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new TestProjectsInput();
            target.HelpText.ShouldBe(@"Specify the test projects. | default: []");
        }

        [TestMethod]
        public void ShouldUseDefaultWhenNull()
        {
            var input = new TestProjectsInput { SuppliedInput = null };

            input.Validate().ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldIgnoreEmptyString()
        {
            var input = new TestProjectsInput { SuppliedInput = new[] { "", "", "" } };

            input.Validate().ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldNormalizePaths()
        {
            var paths = new[] { "/c/root/bla/test.csproj" };
            var expected = new[] { Path.GetFullPath(FilePathUtils.NormalizePathSeparators(paths[0])) };
            var input = new TestProjectsInput { SuppliedInput = paths };

            input.Validate().ShouldBe(expected);
        }
    }
}
