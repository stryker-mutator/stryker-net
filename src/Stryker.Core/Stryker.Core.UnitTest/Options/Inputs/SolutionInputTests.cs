using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class SolutionInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new SolutionInput();
            target.HelpText.ShouldBe(@"Full path to your solution file. Required on dotnet framework.");
        }

        [Fact]
        public void ShouldReturnSolutionPathIfExists()
        {
            var dir = Directory.GetCurrentDirectory();
            var path = Path.Combine(dir, "solution.sln");
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(dir);
            fileSystem.AddFile(path, new MockFileData(""));

            var input = new SolutionInput { SuppliedInput = path };

            input.Validate(fileSystem).ShouldBe(path);
        }

        [Fact]
        public void ShouldReturnFullPathWhenRelativePathGiven()
        {
            var dir = Directory.GetCurrentDirectory();
            var relativePath = "./solution.sln";
            var fullPath = Path.Combine(dir, "solution.sln");
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(dir);
            fileSystem.AddFile(fullPath, new MockFileData(""));
            fileSystem.Directory.SetCurrentDirectory(dir);

            var input = new SolutionInput { SuppliedInput = relativePath };

            input.Validate(fileSystem).ShouldBe(fullPath);
        }

        [Fact]
        public void ShouldBeEmptyWhenNull()
        {
            var input = new SolutionInput { SuppliedInput = null };

            input.Validate(new MockFileSystem()).ShouldBeNull();
        }

        [Fact]
        public void ShouldThrowWhenNotExists()
        {
            var input = new SolutionInput { SuppliedInput = "/c/root/bla/solution.sln" };

            var ex = Should.Throw<InputException>(() =>
            {
                input.Validate(new MockFileSystem());
            });

            ex.Message.ShouldBe("Given path does not exist: /c/root/bla/solution.sln");
        }

        [Fact]
        public void ShouldThrowWhenPathIsNoSolutionFile()
        {
            var input = new SolutionInput { SuppliedInput = "/c/root/bla/solution.csproj" };

            var ex = Should.Throw<InputException>(() =>
            {
                input.Validate(new MockFileSystem());
            });

            ex.Message.ShouldBe("Given path is not a solution file: /c/root/bla/solution.csproj");
        }
    }
}
