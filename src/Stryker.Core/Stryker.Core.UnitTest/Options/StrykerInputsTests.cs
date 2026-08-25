using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Options;
using Stryker.Configuration.Options;
using Stryker.Configuration.Options.Inputs;

namespace Stryker.Core.UnitTest.Options;

[TestClass]
public class StrykerInputsTests : TestBase
{
    private IFileSystem _fileSystem = new  MockFileSystem();
    private readonly StrykerInputs _target;

    public StrykerInputsTests() =>
        _target = new StrykerInputs(_fileSystem)
        {
            AdditionalTimeoutInput = new AdditionalTimeoutInput(),
            AzureFileStorageSasInput = new AzureFileStorageSasInput(),
            AzureFileStorageUrlInput = new AzureFileStorageUrlInput(),
            BaselineProviderInput = new BaselineProviderInput(),
            BaselineOutputInput = new BaselineOutputInput(),
            BasePathInput = new BasePathInput { SuppliedInput = _fileSystem.Directory.GetCurrentDirectory() },
            ConcurrencyInput = new ConcurrencyInput(),
            DashboardApiKeyInput = new DashboardApiKeyInput(),
            DashboardUrlInput = new DashboardUrlInput(),
            DiagModeInput = new DiagModeInput(),
            DiffIgnoreChangesInput = new DiffIgnoreChangesInput(),
            DisableBailInput = new DisableBailInput(),
            DisableMixMutantsInput = new DisableMixMutantsInput(),
            IgnoreMutationsInput = new IgnoreMutationsInput(),
            FallbackVersionInput = new FallbackVersionInput(),
            IgnoredMethodsInput = new IgnoreMethodsInput(),
            LanguageVersionInput = new LanguageVersionInput(),
            VerbosityInput = new VerbosityInput(),
            LogToFileInput = new LogToFileInput(),
            ModuleNameInput = new ModuleNameInput(),
            MutateInput = new MutateInput(),
            MutationLevelInput = new MutationLevelInput(),
            CoverageAnalysisInput = new CoverageAnalysisInput(),
            OutputPathInput = new OutputPathInput { SuppliedInput = _fileSystem.Directory.GetCurrentDirectory() },
            ProjectNameInput = new ProjectNameInput(),
            SourceProjectNameInput = new SourceProjectNameInput(),
            ProjectVersionInput = new ProjectVersionInput(),
            ReportersInput = new ReportersInput(),
            SinceInput = new SinceInput(),
            SinceTargetInput = new SinceTargetInput(),
            SolutionInput = new SolutionInput(),
            TestProjectsInput = new TestProjectsInput(),
            ThresholdBreakInput = new ThresholdBreakInput(),
            ThresholdHighInput = new ThresholdHighInput(),
            ThresholdLowInput = new ThresholdLowInput(),
            WithBaselineInput = new WithBaselineInput(),
            BreakOnInitialTestFailureInput = new BreakOnInitialTestFailureInput(),
        };

    [TestMethod]
    public void PerTestInIsolationShouldSetOptimizationFlags()
    {
        _target.CoverageAnalysisInput.SuppliedInput = "perTestInIsolation";

        var result = _target.ValidateAll();

        result.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();
        result.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest).ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldSetConfiguration()
    {
        _target.ConfigurationInput.SuppliedInput = "TheRelease";
        var result = _target.ValidateAll();
        result.Configuration.ShouldBe("TheRelease");
    }

    [TestMethod]
    public void ShouldSetConfigurationAndPlatform()
    {
        _target.ConfigurationInput.SuppliedInput = "TheRelease|x64";
        var result = _target.ValidateAll();
        result.Configuration.ShouldBe("TheRelease");
        result.Platform.ShouldBe("x64");
    }

    [TestMethod]
    public void ShouldIgnoreExtraInfoInConfiguration()
    {
        _target.ConfigurationInput.SuppliedInput = "TheRelease|x64|Disregarded";
        var result = _target.ValidateAll();
        result.Configuration.ShouldBe("TheRelease");
        result.Platform.ShouldBe("x64");
    }

    [TestMethod]
    public void DisableBailShouldSetOptimizationFlags()
    {
        _target.DisableMixMutantsInput.SuppliedInput = true;

        var result = _target.ValidateAll();

        result.OptimizationMode.HasFlag(OptimizationModes.DisableMixMutants).ShouldBeTrue();
        result.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();
    }

    [TestMethod]
    public void DisableMixMutantsShouldSetOptimizationFlags()
    {
        _target.DisableBailInput.SuppliedInput = true;

        var result = _target.ValidateAll();

        result.OptimizationMode.HasFlag(OptimizationModes.DisableBail).ShouldBeTrue();
        result.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();
    }

    [TestMethod]
    public void AllShouldSetOptimizationFlags()
    {
        _target.CoverageAnalysisInput.SuppliedInput = "all";

        var result = _target.ValidateAll();

        result.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants).ShouldBeTrue();
    }

    [TestMethod]
    public void OffShouldSetOptimizationFlags()
    {
        _target.CoverageAnalysisInput.SuppliedInput = "off";

        var result = _target.ValidateAll();

        result.OptimizationMode.HasFlag(OptimizationModes.None).ShouldBeTrue();
    }

    [TestMethod]
    public void OptimizationFlagsShouldHaveDefaultCoverageBasedTest()
    {
        _target.CoverageAnalysisInput.SuppliedInput = null;

        var result = _target.ValidateAll();

        result.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();
    }

    [TestMethod]
    public void UsingDashboardReporterShouldEnableDashboardApiKey()
    {
        _target.DashboardApiKeyInput.SuppliedInput = "dashboard_api_key";
        _target.ReportersInput.SuppliedInput = new[] { "dashboard" };

        var result = _target.ValidateAll();

        result.DashboardApiKey.ShouldBe("dashboard_api_key");
    }

    [TestMethod]
    public void UsingDashboardBaselineStorageWithBaselineShouldEnableDashboardApiKey()
    {
        _target.DashboardApiKeyInput.SuppliedInput = "dashboard_api_key";
        _target.ReportersInput.SuppliedInput = new[] { "html" };
        _target.BaselineProviderInput.SuppliedInput = "dashboard";
        _target.WithBaselineInput.SuppliedInput = true;
        _target.ProjectVersionInput.SuppliedInput = "develop";

        var result = _target.ValidateAll();

        result.DashboardApiKey.ShouldBe("dashboard_api_key");
    }

    [TestMethod]
    public void NotUsingDashboardBaselineStorageWithBaselineOrDashboardReporterShouldDisableDashboardApiKey()
    {
        _target.DashboardApiKeyInput.SuppliedInput = "dashboard_api_key";
        _target.ReportersInput.SuppliedInput = new[] { "html" };
        _target.BaselineProviderInput.SuppliedInput = "disk";
        _target.WithBaselineInput.SuppliedInput = true;
        _target.ProjectVersionInput.SuppliedInput = "develop";
        _target.BaselineOutputInput.SuppliedInput = Path.GetFullPath("StrykerOutput");

        var result = _target.ValidateAll();

        result.DashboardApiKey.ShouldBeNull();
    }

    [TestMethod]
    public void WithBaselineAndSinceShouldBeMutuallyExclusive()
    {
        _target.WithBaselineInput.SuppliedInput = true;
        _target.SinceInput.SuppliedInput = true;

        var exception = Should.Throw<InputException>(() => _target.ValidateAll());
        exception.Message.ShouldBe("The since and baseline features are mutually exclusive.");
    }

    [TestMethod]
    public void WithBaselineShouldNotThrow_2743() // https://github.com/stryker-mutator/stryker-net/issues/2743
    {
        _target.ProjectVersionInput.SuppliedInput = "1";
        _target.WithBaselineInput.SuppliedInput = true;
        // the disk baseline output is mandatory and is set from the output path (a full path) by the CLI before validation
        _target.BaselineOutputInput.SuppliedInput = Path.GetFullPath("StrykerOutput");

        Should.NotThrow(() => _target.ValidateAll());
    }

    [TestMethod]
    public void BaselineOutputPathShouldThrowWhenDiskBaselineAndNotSupplied()
    {
        _target.ProjectVersionInput.SuppliedInput = "1";
        _target.WithBaselineInput.SuppliedInput = true;

        Should.Throw<InputException>(() => _target.ValidateAll());
    }

    [TestMethod]
    public void BaselineOutputPathShouldBeDefaultWhenBaselineDisabled()
    {
        var result = _target.ValidateAll();

        result.BaselineOutputPath.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldSetBaselineOutputPathWhenSupplied()
    {
        _target.ProjectVersionInput.SuppliedInput = "1";
        _target.WithBaselineInput.SuppliedInput = true;
        var fullPath = Path.GetFullPath("custom-baseline");
        _target.BaselineOutputInput.SuppliedInput = fullPath;

        var result = _target.ValidateAll();

        result.BaselineOutputPath.ShouldBe(fullPath);
    }

    [TestMethod]
    public void BaseLineOptionsShouldBeSetToDefaultWhenBaselineIsDisabled()
    {
        _target.WithBaselineInput.SuppliedInput = false;
        _target.BaselineProviderInput.SuppliedInput = "azurefilestorage";
        _target.AzureFileStorageSasInput.SuppliedInput = "sasCredential";
        _target.AzureFileStorageUrlInput.SuppliedInput = "azureUrl";

        var result = _target.ValidateAll();

        result.WithBaseline.ShouldBeFalse();
        result.BaselineProvider.ShouldBe(BaselineProvider.Disk);
        result.AzureFileStorageSas.ShouldBe(string.Empty);
        result.AzureFileStorageUrl.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldThrowWhenUsingProjectNameInSolutionMode()
    {
        const string SolutionFile = "/test.sln";
        const string ProjectFile = "/project.csproj";
        _fileSystem.Directory.CreateDirectory("/root");
        _fileSystem.File.WriteAllText(SolutionFile, string.Empty);
        _fileSystem.File.WriteAllText(ProjectFile, string.Empty);

        _target.ProjectNameInput.SuppliedInput = ProjectFile;
        _target.SolutionInput.SuppliedInput = SolutionFile;
        Action action = () => _target.ValidateAll();
        action.ShouldThrow<InputException>().Message.ShouldBe("Project name cannot be specified when running Stryker in solution context.");
    }

    [TestMethod]
    public void ShouldThrowWhenUsingTestProjectsInSolutionMode()
    {
        const string SolutionFile = "/test.sln";
        const string ProjectFile = "/project.csproj";
        _fileSystem.Directory.CreateDirectory("/root");
        _fileSystem.File.WriteAllText(SolutionFile, string.Empty);

        _target.TestProjectsInput.SuppliedInput = [ProjectFile];
        _target.SolutionInput.SuppliedInput = SolutionFile;
        Action action = () => _target.ValidateAll();
        action.ShouldThrow<InputException>().Message.ShouldBe("Test projects cannot be specified when running Stryker in solution context.");
    }

    [TestMethod]
    public void ShouldThrowWhenTestProjectsDoesNotExist()
    {
        var projectFile = _fileSystem.Path.GetFullPath( "project.csproj");
        _target.TestProjectsInput.SuppliedInput = [projectFile];
        Action action = () => _target.ValidateAll();
        action.ShouldThrow<InputException>().Message.ShouldBe($"TestProject not found: {projectFile}");
    }
}
