using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.CLI.UnitTest;

public class FileConfigReaderTests
{
    [Fact]
    public void WithNoArgumentsAndNoConfigFile_ShouldStartStrykerWithConfigOptions()
    {
        var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
        var options = new StrykerOptions()
        {
            Thresholds = new Thresholds()
            {
                High = 80,
                Low = 60,
                Break = 0
            }
        };
        var currentDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory($"..{Path.DirectorySeparatorChar}");
        var runResults = new StrykerRunResult(options, 0.3);
        mock.Setup(x => x.RunMutationTest(It.IsAny<StrykerInputs>(), It.IsAny<ILoggerFactory>(), It.IsAny<IProjectOrchestrator>())).Returns(runResults).Verifiable();
        var target = new StrykerCli(mock.Object);

        target.Run(new string[] { });

        mock.VerifyAll();

        Directory.SetCurrentDirectory(currentDirectory);
    }

    [Theory]
    [InlineData("--config-file")]
    [InlineData("-f")]
    public void WithJsonConfigFile_ShouldStartStrykerWithConfigFileOptions(string argName)
    {
        IStrykerInputs actualInputs = null;
        var options = new StrykerOptions()
        {
            Thresholds = new Thresholds()
            {
                High = 80,
                Low = 60,
                Break = 0
            }
        };
        var runResults = new StrykerRunResult(options, 0.3);

        var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
        mock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<ILoggerFactory>(), It.IsAny<IProjectOrchestrator>()))
            .Callback<IStrykerInputs, ILoggerFactory, IProjectOrchestrator>((c, l, p) => actualInputs = c)
            .Returns(runResults)
            .Verifiable();

        var target = new StrykerCli(mock.Object);

        target.Run(new string[] { argName, "filled-stryker-config.json" });

        mock.VerifyAll();

        actualInputs.AdditionalTimeoutInput.SuppliedInput.ShouldBe(9999);
        actualInputs.VerbosityInput.SuppliedInput.ShouldBe("trace");
        actualInputs.SourceProjectNameInput.SuppliedInput.ShouldBe("ExampleProject.csproj");
        actualInputs.ReportersInput.SuppliedInput.ShouldHaveSingleItem();
        actualInputs.ReportersInput.SuppliedInput.ShouldContain(Reporter.Json.ToString());
        actualInputs.ConcurrencyInput.SuppliedInput.ShouldBe(1);
        actualInputs.ThresholdBreakInput.SuppliedInput.ShouldBe(20);
        actualInputs.ThresholdLowInput.SuppliedInput.ShouldBe(30);
        actualInputs.ThresholdHighInput.SuppliedInput.ShouldBe(40);
        actualInputs.MutateInput.SuppliedInput.ShouldHaveSingleItem();
        actualInputs.MutateInput.SuppliedInput.ShouldContain("!**/Test.cs{1..100}{200..300}");
        actualInputs.CoverageAnalysisInput.SuppliedInput.ShouldBe("perTest");
        actualInputs.DisableBailInput.SuppliedInput.ShouldBe(true);
        actualInputs.IgnoreMutationsInput.SuppliedInput.ShouldContain("linq.FirstOrDefault");
        actualInputs.IgnoredMethodsInput.SuppliedInput.ShouldContain("Log*");
        actualInputs.TestCaseFilterInput.SuppliedInput.ShouldBe("(FullyQualifiedName~UnitTest1&TestCategory=CategoryA)|Priority=1");
        actualInputs.DashboardUrlInput.SuppliedInput.ShouldBe("https://alternative-stryker-dashboard.io");
        actualInputs.BreakOnInitialTestFailureInput.SuppliedInput.ShouldNotBeNull().ShouldBeFalse();
    }

    [Fact]
    public void WithYamlConfigFile_ShouldStartStrykerWithConfigFileOptions()
    {
        IStrykerInputs actualInputs = null;
        var options = new StrykerOptions()
        {
            Thresholds = new Thresholds()
            {
                High = 80,
                Low = 60,
                Break = 0
            }
        };
        var runResults = new StrykerRunResult(options, 0.3);

        var mock = new Mock<IStrykerRunner>(MockBehavior.Strict);
        mock.Setup(x => x.RunMutationTest(It.IsAny<IStrykerInputs>(), It.IsAny<ILoggerFactory>(), It.IsAny<IProjectOrchestrator>()))
            .Callback<IStrykerInputs, ILoggerFactory, IProjectOrchestrator>((c, l, p) => actualInputs = c)
            .Returns(runResults)
            .Verifiable();

        var target = new StrykerCli(mock.Object);

        target.Run(new string[] { "-f", "filled-stryker-config.yaml" });

        mock.VerifyAll();

        actualInputs.AdditionalTimeoutInput.SuppliedInput.ShouldBe(9999);
        actualInputs.VerbosityInput.SuppliedInput.ShouldBe("trace");
        actualInputs.SourceProjectNameInput.SuppliedInput.ShouldBe("ExampleProject.csproj");
        actualInputs.ReportersInput.SuppliedInput.ShouldHaveSingleItem();
        actualInputs.ReportersInput.SuppliedInput.ShouldContain(Reporter.Json.ToString());
        actualInputs.ConcurrencyInput.SuppliedInput.ShouldBe(1);
        actualInputs.ThresholdBreakInput.SuppliedInput.ShouldBe(20);
        actualInputs.ThresholdLowInput.SuppliedInput.ShouldBe(30);
        actualInputs.ThresholdHighInput.SuppliedInput.ShouldBe(40);
        actualInputs.MutateInput.SuppliedInput.ShouldHaveSingleItem();
        actualInputs.MutateInput.SuppliedInput.ShouldContain("!**/Test.cs{1..100}{200..300}");
        actualInputs.CoverageAnalysisInput.SuppliedInput.ShouldBe("perTest");
        actualInputs.DisableBailInput.SuppliedInput.ShouldBe(true);
        actualInputs.IgnoreMutationsInput.SuppliedInput.ShouldContain("linq.FirstOrDefault");
        actualInputs.IgnoredMethodsInput.SuppliedInput.ShouldContain("Log*");
        actualInputs.TestCaseFilterInput.SuppliedInput.ShouldBe("(FullyQualifiedName~UnitTest1&TestCategory=CategoryA)|Priority=1");
        actualInputs.DashboardUrlInput.SuppliedInput.ShouldBe("https://alternative-stryker-dashboard.io");
        actualInputs.BreakOnInitialTestFailureInput.SuppliedInput.ShouldNotBeNull().ShouldBeTrue();
    }
}
