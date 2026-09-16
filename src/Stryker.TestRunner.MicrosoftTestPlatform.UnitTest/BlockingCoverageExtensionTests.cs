using System.Runtime.Versioning;
using System.Text.Json;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;
using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Extension;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class BlockingCoverageExtensionTests
{
    [TestInitialize]
    public void Initialize()
    {
        FakeInjectedHelper.MutantControl.FlushCount = 0;
        Environment.SetEnvironmentVariable(MtpCompatibility.BlockingCoverageEnvironmentVariable, "1");
    }

    [TestCleanup]
    public void Cleanup() =>
        Environment.SetEnvironmentVariable(MtpCompatibility.BlockingCoverageEnvironmentVariable, null);

    [TestMethod]
    public async Task ConsumeAsync_ShouldFlushLoadedMutantControlsForTerminalUpdate()
    {
        var consumer = new BlockingCoverageDataConsumer();
        var message = CreateMessage(PassedTestNodeStateProperty.CachedInstance);

        await consumer.ConsumeAsync(null!, message, CancellationToken.None);

        FakeInjectedHelper.MutantControl.FlushCount.ShouldBe(1);
    }

    [TestMethod]
    public async Task ConsumeAsync_ShouldNotFlushForNonTerminalUpdate()
    {
        var consumer = new BlockingCoverageDataConsumer();

        await consumer.ConsumeAsync(
            null!,
            CreateMessage(InProgressTestNodeStateProperty.CachedInstance),
            CancellationToken.None);

        FakeInjectedHelper.MutantControl.FlushCount.ShouldBe(0);
    }

    [TestMethod]
    public void DynamicExtensionManifest_ShouldReferenceBlockingCoverageHookAndDeleteOnDispose()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"stryker-manifest-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var testApplication = Path.Combine(directory, "Tests.dll");
        File.WriteAllBytes(testApplication, []);

        string manifestPath;
        try
        {
            using (var firstManifest = DynamicExtensionManifest.Acquire(testApplication))
            {
                using var secondManifest = DynamicExtensionManifest.Acquire(testApplication);
                manifestPath = firstManifest.Path;
                secondManifest.Path.ShouldBe(manifestPath);
                File.Exists(manifestPath).ShouldBeTrue();

                using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
                var extension = document.RootElement.GetProperty("extensions")[0];
                extension.GetProperty("assemblyPath").GetString()
                    .ShouldBe(typeof(BlockingCoverageTestingPlatformBuilderHook).Assembly.Location);
                extension.GetProperty("typeFullName").GetString()
                    .ShouldBe(typeof(BlockingCoverageTestingPlatformBuilderHook).FullName);

                firstManifest.Dispose();
                File.Exists(manifestPath).ShouldBeTrue(
                    "the manifest must remain while another server for the test application is starting");
            }

            File.Exists(manifestPath).ShouldBeFalse();
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void DynamicExtensionManifest_ShouldRemoveManifestFromDeadProcess()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"stryker-manifest-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var testApplication = Path.Combine(directory, "Tests.dll");
        var staleManifest = Path.Combine(
            directory,
            "stryker-2147483647-1.testingplatformextensions.json");
        File.WriteAllBytes(testApplication, []);
        File.WriteAllText(staleManifest, "{}");

        try
        {
            using var manifest = DynamicExtensionManifest.Acquire(testApplication);

            File.Exists(staleManifest).ShouldBeFalse();
        }

        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void DynamicExtensionManifest_ShouldShareManifestAcrossTestApplicationsInSameDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"stryker-manifest-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var firstTestApplication = Path.Combine(directory, "FirstTests.dll");
        var secondTestApplication = Path.Combine(directory, "SecondTests.dll");
        File.WriteAllBytes(firstTestApplication, []);
        File.WriteAllBytes(secondTestApplication, []);

        try
        {
            using var firstManifest = DynamicExtensionManifest.Acquire(firstTestApplication);
            using var secondManifest = DynamicExtensionManifest.Acquire(secondTestApplication);

            secondManifest.Path.ShouldBe(firstManifest.Path);
            firstManifest.Dispose();
            File.Exists(secondManifest.Path).ShouldBeTrue();
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void BlockingCoverageExtension_ShouldTargetNetStandard()
    {
        var targetFramework = typeof(BlockingCoverageTestingPlatformBuilderHook).Assembly
            .GetCustomAttributes(typeof(TargetFrameworkAttribute), inherit: false)
            .OfType<TargetFrameworkAttribute>()
            .ShouldHaveSingleItem();

        targetFramework.FrameworkName.ShouldBe(".NETStandard,Version=v2.0");
    }

    private static TestNodeUpdateMessage CreateMessage(TestNodeStateProperty state) =>
        new(
            new SessionUid(Guid.NewGuid().ToString()),
            new Microsoft.Testing.Platform.Extensions.Messages.TestNode
            {
                Uid = new TestNodeUid("test"),
                DisplayName = "Test",
                Properties = new(state)
            });
}
