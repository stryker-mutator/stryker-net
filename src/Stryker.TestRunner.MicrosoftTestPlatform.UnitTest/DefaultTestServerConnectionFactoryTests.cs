using Microsoft.Testing.Platform.ServerMode.Client;
using Moq;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class DefaultTestServerConnectionFactoryTests
{
    [TestMethod]
    public void BuildCommand_ShouldUseDotnetForDll()
    {
        var supportsTestCaseFilter = new Mock<Func<string, bool>>();
        var factory = CreateFactory(string.Empty, supportsTestCaseFilter.Object);

        var (targetFilePath, arguments) = factory.BuildCommand(@"C:\tests\TestProject.dll", 1234);

        supportsTestCaseFilter.VerifyNoOtherCalls();
        targetFilePath.ShouldBe("dotnet");
        arguments.ShouldBe([@"C:\tests\TestProject.dll", .. MtpServerConnector.BuildInProcessServerArguments(1234)]);
    }

    [TestMethod]
    [DataRow(@"C:\tests\TestProject.exe")]
    [DataRow(@"C:\tests\TestProject.EXE")]
    public void BuildCommand_ShouldRunExeDirectly(string assembly)
    {
        var supportsTestCaseFilter = new Mock<Func<string, bool>>();
        var factory = CreateFactory(string.Empty, supportsTestCaseFilter.Object);

        var (targetFilePath, arguments) = factory.BuildCommand(assembly, 1234);

        supportsTestCaseFilter.VerifyNoOtherCalls();
        targetFilePath.ShouldBe(assembly);
        arguments.ShouldBe([.. MtpServerConnector.BuildInProcessServerArguments(1234)]);
    }

    [TestMethod]
    public void BuildCommand_ShouldAddFilterAsSingleArgument_WhenFilterIsConfigured()
    {
        const string filter = "(FullyQualifiedName~UnitTests&Category!=Integration)|Priority=1";
        var factory = CreateFactory(filter, _ => true);

        var (targetFilePath, arguments) = factory.BuildCommand("tests.dll", 1234);

        targetFilePath.ShouldBe("dotnet");
        arguments.ShouldBe(["tests.dll", .. MtpServerConnector.BuildInProcessServerArguments(1234), "--filter", filter]);
    }

    [TestMethod]
    public void BuildCommand_ShouldRejectFilter_WhenTestApplicationDoesNotSupportIt()
    {
        var factory = CreateFactory("FullyQualifiedName~UnitTests", _ => false);

        var exception = Should.Throw<InputException>(() => factory.BuildCommand("tests.dll", 1234));

        exception.Message.ShouldContain("tests.dll");
        exception.Message.ShouldContain("does not support");
        exception.Details.ShouldContain("--filter");
    }

    [TestMethod]
    [DataRow("  --filter")]
    [DataRow("  --filter <EXPRESSION>")]
    public void HelpListsTestCaseFilter_ShouldReturnTrue_ForExactFilterOption(string helpOutput)
    {
        DefaultTestServerConnectionFactory.HelpListsTestCaseFilter(helpOutput).ShouldBeTrue();
    }

    [TestMethod]
    [DataRow("  --filter-uid")]
    [DataRow("  --treenode-filter")]
    [DataRow("  -filter \"query\"")]
    public void HelpListsTestCaseFilter_ShouldReturnFalse_ForDifferentFilterOptions(string helpOutput)
    {
        DefaultTestServerConnectionFactory.HelpListsTestCaseFilter(helpOutput).ShouldBeFalse();
    }

    [TestMethod]
    public void CreateTestCaseFilterSupportProbe_ShouldExecuteProbeOnce_WhenReadConcurrently()
    {
        var probeCalls = 0;
        var probe = DefaultTestServerConnectionFactory.CreateTestCaseFilterSupportProbe("tests.dll", _ =>
        {
            Interlocked.Increment(ref probeCalls);
            Thread.Sleep(50);
            return true;
        });

        Parallel.For(0, 20, _ => probe.Value.ShouldBeTrue());

        probeCalls.ShouldBe(1);
    }

    private static DefaultTestServerConnectionFactory CreateFactory(
        string testCaseFilter,
        Func<string, bool> supportsTestCaseFilter)
    {
        var options = new Mock<IStrykerOptions> { DefaultValue = DefaultValue.Mock };
        options.SetupGet(x => x.TestCaseFilter).Returns(testCaseFilter);

        return new DefaultTestServerConnectionFactory(options.Object, supportsTestCaseFilter);
    }
}
