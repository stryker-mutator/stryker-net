using Moq;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class DefaultTestServerConnectionFactoryTests
{
    [TestMethod]
    public void BuildArguments_ShouldNotAddFilter_WhenFilterIsEmpty()
    {
        var supportsTestCaseFilter = new Mock<Func<string, bool>>();
        var factory = CreateFactory(string.Empty, supportsTestCaseFilter.Object);

        var arguments = factory.BuildArguments("tests.dll", 1234);

        supportsTestCaseFilter.VerifyNoOtherCalls();
        arguments.ShouldBe(["tests.dll", "--server", "--client-port", "1234"]);
    }

    [TestMethod]
    public void BuildArguments_ShouldAddFilterAsSingleArgument_WhenFilterIsConfigured()
    {
        const string filter = "(FullyQualifiedName~UnitTests&Category!=Integration)|Priority=1";
        var factory = CreateFactory(filter, _ => true);

        var arguments = factory.BuildArguments("tests.dll", 1234);

        arguments.ShouldBe(["tests.dll", "--server", "--client-port", "1234", "--filter", filter]);
    }

    [TestMethod]
    public void BuildArguments_ShouldRejectFilter_WhenTestApplicationDoesNotSupportIt()
    {
        var factory = CreateFactory("FullyQualifiedName~UnitTests", _ => false);

        var exception = Should.Throw<InputException>(() => factory.BuildArguments("tests.dll", 1234));

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
