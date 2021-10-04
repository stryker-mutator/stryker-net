using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Shouldly;
using Stryker.CLI.Logging;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    public class InputBuilderTests
    {
        [Fact]
        public void ShouldAddGitIgnore()
        {
            var fileSystemMock = new MockFileSystem();
            var basePath = Directory.GetCurrentDirectory();
            var target = new LoggingInitializer();

            var inputs = new StrykerInputs();
            inputs.BasePathInput.SuppliedInput = basePath;
            target.SetupLogOptions(inputs, fileSystemMock);

            var gitIgnoreFile = fileSystemMock.AllFiles.FirstOrDefault(x => x.EndsWith(Path.Combine("StrykerOutput", ".gitignore")));
            gitIgnoreFile.ShouldNotBeNull();
            var fileContents = fileSystemMock.GetFile(gitIgnoreFile).Contents;
            Encoding.Default.GetString(fileContents).ShouldBe("*");
        }
    }
}
