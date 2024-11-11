using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Helpers;

namespace Stryker.Core.UnitTest.Helpers;

[TestClass]
public class VsTestHelperTests : TestBase
{
    [TestMethod]
    public void DeployEmbeddedVsTestBinaries()
    {
        var deployPath = new VsTestHelper().DeployEmbeddedVsTestBinaries();

        var vsTestFiles = Directory.EnumerateFiles(deployPath, "*", SearchOption.AllDirectories).Select(Path.GetFileName).ToList();

        try
        {
            vsTestFiles.ShouldContain("vstest.console.dll");
            vsTestFiles.ShouldContain("vstest.console.exe");
        }
        finally
        {
            Directory.Delete(deployPath, recursive: true);
        }
    }
}
