using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class MtpTestDescriptionTests
{
    [TestMethod]
    public void MtpTestDescription_WithLocationFile_PopulatesTestFilePath()
    {
        var testNode = new TestNode("uid-1", "TestMethod1", "action", "discovered",
            LocationFile: "/path/to/TestFile.cs");

        var description = new MtpTestDescription(testNode);

        description.Description.TestFilePath.ShouldBe("/path/to/TestFile.cs");
    }

    [TestMethod]
    public void MtpTestDescription_WithoutLocationFile_UsesEmptyTestFilePath()
    {
        var testNode = new TestNode("uid-1", "TestMethod1", "action", "discovered");

        var description = new MtpTestDescription(testNode);

        description.Description.TestFilePath.ShouldBe(string.Empty);
    }
}
