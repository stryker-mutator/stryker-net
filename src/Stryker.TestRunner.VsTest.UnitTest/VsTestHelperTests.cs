using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using Shouldly;
using Stryker.Core.UnitTest;
using Stryker.TestRunner.VsTest.Helpers;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace Stryker.TestRunner.VsTest.UnitTest;

[TestClass]
public class VsTestHelperTests : TestBase
{
    [TestMethod]
    public void DeployEmbeddedVsTestBinaries()
    {
        var vsTestHelper = VsTestHelper.CreateInstance() as VsTestHelper;
        var deployPath = vsTestHelper!.DeployEmbeddedVsTestBinaries();

        var vsTestFiles = Directory.EnumerateFiles(deployPath, "*", SearchOption.AllDirectories)
            .Select(Path.GetFileName).ToList();

        try
        {
            vsTestFiles.ShouldContain("vstest.console.dll");
            vsTestFiles.ShouldContain("vstest.console.exe");
            Assert.IsNotEmpty(Directory.EnumerateFiles(deployPath, "*", SearchOption.AllDirectories));
        }
        finally
        {
            vsTestHelper.Cleanup();
        }

        Assert.IsEmpty(Directory.EnumerateFiles(deployPath, "*", SearchOption.AllDirectories));
    }

    [TestMethod]
    [DataRow("LINUX", "vstest.console.dll")]
    [DataRow("WINDOWS", "vstest.console.exe")]
    [DataRow("OSX", "vstest.console.dll")]
    public void Support_Types(string platform, string ending)
    {
        var vsTestHelper = VsTestHelper.CreateInstance(isOsPlatform: osPlatform => osPlatform == OSPlatform.Create(platform));
        var actual = vsTestHelper.GetCurrentPlatformVsTestToolPath();
        Assert.EndsWith(ending, actual);
    }

    [TestMethod]
    public void Support_Types_FreeBSD()
    {
        var vsTestHelper = VsTestHelper.CreateInstance(isOsPlatform: osPlatform => osPlatform == OSPlatform.Create("FREEBSD"));
        Assert.Throws<PlatformNotSupportedException>(() => vsTestHelper.GetCurrentPlatformVsTestToolPath());
    }

    [TestMethod]
    public void NugetsNotFound()
    {
        var fileSystem = new FakeSystem();
        var vsTestHelper = VsTestHelper.CreateInstance(fileSystem: fileSystem,
            isOsPlatform: platform => OSPlatform.Windows == platform);

        Assert.Throws<PlatformNotSupportedException>(() => vsTestHelper.GetCurrentPlatformVsTestToolPath());
    }

    [TestMethod]
    [DataRow("WINDOWS", "vstest.console.exe")]
    [DataRow("OSX", "vstest.console.dll")]
    public void GetVsTestToolPath_WithRealFileSystem_ReturnsExpectedPath(string platformName, string expectedFileName)
    {
        var osPlatform = OSPlatform.Create(platformName);
        var fileSystem = new FileSystem();
        var vsTestHelper = VsTestHelper.CreateInstance(fileSystem: fileSystem,
            isOsPlatform: platform => platform == osPlatform);

        var currentPlatformVsTestToolPath = vsTestHelper.GetCurrentPlatformVsTestToolPath();

        // Only take the last part of the path for CI compatibility
        var actualFileName = Path.GetFileName(currentPlatformVsTestToolPath);
        Assert.AreEqual(expectedFileName, actualFileName);

        // Test that repeated calls return the same path
        var secondCall = vsTestHelper.GetCurrentPlatformVsTestToolPath();
        actualFileName = Path.GetFileName(secondCall);
        Assert.AreEqual(expectedFileName, actualFileName);
    }

    [TestMethod]
    [DataRow("WINDOWS")]
    [DataRow("OSX")]
    public void GetVsTestToolPath_WithFakeFileSystem_ThrowsPlatformNotSupportedException(string platformName)
    {
        var osPlatform = OSPlatform.Create(platformName);
        var fileSystem = new FakeSystem();
        var vsTestHelper = VsTestHelper.CreateInstance(fileSystem: fileSystem,
            isOsPlatform: platform => platform == osPlatform);

        var ex = Assert.Throws<PlatformNotSupportedException>(() => vsTestHelper.GetCurrentPlatformVsTestToolPath());
        Assert.AreEqual("Could not find any VS test tool paths for this platform.", ex.Message);
    }

    [TestMethod]
    [DataRow("FREEBSD", true)]
    [DataRow("FREEBSD", false)]
    public void GetVsTestToolPath_WithUnsupportedPlatform_ThrowsPlatformNotSupportedException(string platformName, bool fileSystemExists)
    {
        var osPlatform = OSPlatform.Create(platformName);
        var fileSystem = fileSystemExists ? new FileSystem() : new FakeSystem();
        var vsTestHelper = VsTestHelper.CreateInstance(fileSystem: fileSystem,
            isOsPlatform: platform => platform == osPlatform);

        var ex = Assert.Throws<PlatformNotSupportedException>(() => vsTestHelper.GetCurrentPlatformVsTestToolPath());
        Assert.AreEqual("The current OS is not any of the following currently supported: WINDOWS, LINUX, OSX", ex.Message);
    }
}

public class FakeSystem : FileSystem
{
    public FakeSystem() => Directory = new FakeDirectory(this);

    public override IDirectory Directory { get; }
}

public class FakeDirectory(IFileSystem fileSystem) : DirectoryWrapper(fileSystem)
{
    public override bool Exists(string path) => false;
}
