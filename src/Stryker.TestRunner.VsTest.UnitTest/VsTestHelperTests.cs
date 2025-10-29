using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Shouldly;
using Stryker.Core.UnitTest;
using Stryker.TestRunner.VsTest.Helpers;
using VerifyMSTest;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace Stryker.TestRunner.VsTest.UnitTest;

[UsesVerify]
[TestClass]
public partial class VsTestHelperTests : TestBase
{
    [TestMethod]
    public void DeployEmbeddedVsTestBinaries()
    {
        var deployPath = (VsTestHelper.CreateInstance() as VsTestHelper)?.DeployEmbeddedVsTestBinaries();

        var vsTestFiles = Directory.EnumerateFiles(deployPath, "*", SearchOption.AllDirectories)
            .Select(Path.GetFileName).ToList();

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
    public async Task Combinations()
    {

        IEnumerable<OSPlatform> osPlatforms = [OSPlatform.Windows, OSPlatform.FreeBSD, OSPlatform.OSX];

        IEnumerable<bool> trueFalse = [true, false];
        await Verifier.Combination().Verify((repeat, osPlatform, exists) =>
        {
            var fileSystem = new FakeSystem();
            var vsTestHelper = VsTestHelper.CreateInstance(fileSystem: exists ? new FileSystem(): fileSystem,
                isOsPlatform: platform => platform == osPlatform);
            try
            {
                var currentPlatformVsTestToolPath = vsTestHelper.GetCurrentPlatformVsTestToolPath();
                if(repeat)
                    return vsTestHelper.GetCurrentPlatformVsTestToolPath();

                return currentPlatformVsTestToolPath;
            }
            catch ( PlatformNotSupportedException ex)
            {
                return ex.Message;
            }
        }, trueFalse, osPlatforms, trueFalse);
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
