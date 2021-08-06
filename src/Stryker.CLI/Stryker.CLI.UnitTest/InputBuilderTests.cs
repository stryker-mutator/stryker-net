using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Shouldly;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    [Collection("StaticConfigBuilder")]
    public class InputBuilderTests
    {
        [Fact]
        public void ShouldAddGitIgnore()
        {
            var fileSystemMock = new MockFileSystem();
            var basePath = Directory.GetCurrentDirectory();

            var inputs = InputBuilder.InitializeInputs();
            inputs.BasePathInput.SuppliedInput = basePath;
            InputBuilder.SetupLogOptions(inputs, fileSystemMock);

            var gitIgnoreFile = fileSystemMock.AllFiles.FirstOrDefault(x => x.EndsWith(Path.Combine("StrykerOutput", ".gitignore")));
            gitIgnoreFile.ShouldNotBeNull();
            var fileContents = fileSystemMock.GetFile(gitIgnoreFile).Contents;
            Encoding.Default.GetString(fileContents).ShouldBe("*");
        }
    }
}
