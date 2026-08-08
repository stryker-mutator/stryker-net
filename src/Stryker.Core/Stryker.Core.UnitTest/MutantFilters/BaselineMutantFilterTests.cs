using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Configuration.Options;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Baseline.Utils;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Core.UnitTest.Reporters.Json;

namespace Stryker.Core.UnitTest.MutantFilters;

[TestClass]
public class BaselineMutantFilterTests : TestBase
{
    private static readonly DiffResult EmptyContentDiff = new([], string.Empty, string.Empty);

    [TestMethod]
    public void ShouldHaveName()
    {
        // Arrange
        var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Loose);
        var baselineProviderMock = new Mock<IBaselineProvider>(MockBehavior.Loose);

        // Act
        var target = new BaselineMutantFilter(new StrykerOptions(), baselineProviderMock.Object, gitInfoProvider.Object) as IMutantFilter;

        // Assert
        target.DisplayName.ShouldBe("baseline filter");
    }

    [TestMethod]
    public void GetBaseline_UsesBaselineFallbackVersion_WhenReportForCurrentVersionNotFound()
    {
        // Arrange
        var branchName = "refs/heads/master";
        var baselineProvider = new Mock<IBaselineProvider>();
        var gitInfoProvider = new Mock<IGitInfoProvider>();

        var options = new StrykerOptions()
        {
            WithBaseline = true,
            DashboardApiKey = "Acces_Token",
            ProjectName = "github.com/JohnDoe/project",
            ProjectVersion = "version/human/readable",
            Reporters = new[] { Reporter.Dashboard },
            FallbackVersion = "fallback/version"
        };

        var inputComponent = new Mock<IReadOnlyProjectComponent>().Object;

        var jsonReport = JsonReport.Build(options, inputComponent, It.IsAny<TestProjectsInfo>());

        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns(branchName);

        baselineProvider.Setup(x => x.Load($"baseline/{branchName}")).Returns(Task.FromResult<IJsonReport>(null));
        baselineProvider.Setup(x => x.Load($"baseline/{options.FallbackVersion}")).Returns(Task.FromResult(jsonReport));

        // Act
        var target = new BaselineMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object);

        // Assert
        baselineProvider.Verify(x => x.Load($"baseline/{branchName}"), Times.Once);
        baselineProvider.Verify(x => x.Load($"baseline/{options.FallbackVersion}"), Times.Once);
        baselineProvider.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void GetBaseline_UsesFallbackVersion_WhenBaselineFallbackVersionNotFound()
    {
        // Arrange
        var branchName = "refs/heads/master";
        var baselineProvider = new Mock<IBaselineProvider>();
        var gitInfoProvider = new Mock<IGitInfoProvider>();

        var options = new StrykerOptions()
        {
            WithBaseline = true,
            DashboardApiKey = "Acces_Token",
            ProjectName = "github.com/JohnDoe/project",
            ProjectVersion = "version/human/readable",
            Reporters = new[] { Reporter.Dashboard },
            FallbackVersion = "fallback/version"
        };

        var inputComponent = new Mock<IReadOnlyProjectComponent>().Object;

        var jsonReport = JsonReport.Build(options, inputComponent, It.IsAny<TestProjectsInfo>());

        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns(branchName);

        baselineProvider.Setup(x => x.Load(branchName)).Returns(Task.FromResult<IJsonReport>(null));
        baselineProvider.Setup(x => x.Load($"baseline/{options.FallbackVersion}")).Returns(Task.FromResult<IJsonReport>(null));
        baselineProvider.Setup(x => x.Load(options.FallbackVersion)).Returns(Task.FromResult(jsonReport));

        // Act
        var target = new BaselineMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object);

        // Assert
        baselineProvider.Verify(x => x.Load($"baseline/{branchName}"), Times.Once);
        baselineProvider.Verify(x => x.Load($"baseline/{options.FallbackVersion}"), Times.Once);
        baselineProvider.Verify(x => x.Load(options.FallbackVersion), Times.Once);
        baselineProvider.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void GetBaseline_UsesCurrentVersionReport_IfReportExists()
    {
        // Arrange
        var branchName = "refs/heads/master";
        var baselineProvider = new Mock<IBaselineProvider>();
        var gitInfoProvider = new Mock<IGitInfoProvider>();

        var options = new StrykerOptions()
        {
            WithBaseline = true,
            DashboardApiKey = "Access_Token",
            ProjectName = "github.com/JohnDoe/project",
            ProjectVersion = "version/human/readable",
            Reporters = new[] { Reporter.Dashboard },
            FallbackVersion = "fallback/version"
        };

        var inputComponent = new Mock<IReadOnlyProjectComponent>().Object;

        var jsonReport = JsonReport.Build(options, inputComponent, It.IsAny<TestProjectsInfo>());

        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns(branchName);

        baselineProvider.Setup(x => x.Load($"baseline/{branchName}")).Returns(Task.FromResult(jsonReport));

        // Act
        var target = new BaselineMutantFilter(options, gitInfoProvider: gitInfoProvider.Object, baselineProvider: baselineProvider.Object);

        // Assert
        baselineProvider.Verify(x => x.Load($"baseline/{branchName}"), Times.Once);
        baselineProvider.Verify(x => x.Load($"baseline/{options.FallbackVersion}"), Times.Never);
        baselineProvider.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void FilterMutantsReturnAllMutantsWhenCompareToDashboardEnabledAndBaselineNotAvailable()
    {
        // Arrange
        var baselineProvider = new Mock<IBaselineProvider>();
        var branchProvider = new Mock<IGitInfoProvider>();

        var options = new StrykerOptions()
        {
            WithBaseline = true,
            ProjectVersion = "version",
        };

        var target = new BaselineMutantFilter(options, baselineProvider.Object, branchProvider.Object);

        var file = new CsharpFileLeaf();

        var mutants = new List<Mutant>
        {
            new Mutant(),
            new Mutant(),
            new Mutant()
        };

        // Act
        var results = target.FilterMutants(mutants, file, options);

        // Assert
        results.Count().ShouldBe(3);
    }

    [TestMethod]
    public void FilterMutants_WhenNoMatchingMutants_MutantIsReturnedUnchanged()
    {
        // Arrange
        var branchProvider = new Mock<IGitInfoProvider>();
        var baselineProvider = new Mock<IBaselineProvider>();
        var diffProvider = new Mock<IDiffProvider>();
        var contentMatcher = new Mock<IContentMutantMatcher>();

        var options = new StrykerOptions()
        {
            WithBaseline = true,
            ProjectVersion = "version",
        };
        var file = new CsharpFileLeaf
        {
            RelativePath = "foo.cs",
            SourceCode = "var foo = \"bar\";"
        };

        var mutants = new List<IMutant>
        {
            new Mutant { ResultStatus = MutantStatus.Pending }
        };

        var jsonMutants = new HashSet<IJsonMutant>
        {
            new JsonMutant()
        };

        // Setup Mocks
        var jsonReportFileComponent = new MockJsonReportFileComponent("", "var foo = \"bar\";", jsonMutants);

        var jsonFileComponents = new Dictionary<string, ISourceFile>
        {
            ["foo.cs"] = jsonReportFileComponent
        };

        var baseline = new MockJsonReport(null, jsonFileComponents);

        baselineProvider.Setup(mock => mock.Load(It.IsAny<string>()))
            .Returns(Task.FromResult((IJsonReport)baseline));

        diffProvider.Setup(mock => mock.GetContentDiff(jsonReportFileComponent.Source, file.SourceCode))
            .Returns(EmptyContentDiff);
        contentMatcher.Setup(mock => mock.MatchByLocation(mutants, jsonMutants.First(), EmptyContentDiff))
            .Returns([]);

        // Act
        var target = new BaselineMutantFilter(options, baselineProvider.Object, branchProvider.Object, diffProvider.Object, contentMatcher.Object);

        var results = target.FilterMutants(mutants, file, options);

        // Assert
        var result = results.ShouldHaveSingleItem();
        result.ResultStatus.ShouldBe(MutantStatus.Pending);
    }

    [TestMethod]
    public void FilterMutants_WhenMutantMatchesLocation_StatusIsSetToJsonMutant()
    {
        // Arrange
        var branchProvider = new Mock<IGitInfoProvider>();
        var baselineProvider = new Mock<IBaselineProvider>();
        var diffProvider = new Mock<IDiffProvider>();
        var contentMatcher = new Mock<IContentMutantMatcher>();

        var options = new StrykerOptions()
        {
            WithBaseline = true,
            ProjectVersion = "version",
        };
        var file = new CsharpFileLeaf
        {
            RelativePath = "foo.cs",
            SourceCode = "var foo = \"bar\";"
        };

        var mutants = new List<IMutant>
        {
            new Mutant
            {
                ResultStatus = MutantStatus.Pending
            }
        };

        var jsonMutants = new HashSet<IJsonMutant>
        {
            new JsonMutant
            {
                Status = "Killed"
            }
        };

        // Setup Mocks
        var jsonReportFileComponent = new MockJsonReportFileComponent("", "var foo = \"bar\";", jsonMutants);

        var jsonFileComponents = new Dictionary<string, ISourceFile>
        {
            ["foo.cs"] = jsonReportFileComponent
        };

        var baseline = new MockJsonReport(null, jsonFileComponents);

        baselineProvider.Setup(mock => mock.Load(It.IsAny<string>()))
            .Returns(Task.FromResult(baseline as IJsonReport));

        diffProvider.Setup(mock => mock.GetContentDiff(jsonReportFileComponent.Source, file.SourceCode))
            .Returns(EmptyContentDiff);
        contentMatcher.Setup(mock => mock.MatchByLocation(mutants, jsonMutants.First(), EmptyContentDiff))
            .Returns(mutants).Verifiable();

        // Act
        var target = new BaselineMutantFilter(options, baselineProvider.Object, branchProvider.Object, diffProvider.Object, contentMatcher.Object);

        var results = target.FilterMutants(mutants, file, options);

        // Assert
        results.ShouldHaveSingleItem().ResultStatus.ShouldBe(MutantStatus.Killed);
        contentMatcher.Verify();
    }

    [TestMethod]
    public void FilterMutants_WhenMultipleMutantsMatchLocation_AllReuseBaselineStatus()
    {
        // Arrange
        // Since matching is location-based (not fragile source-text equality), multiple mutants
        // matching the same remapped location are no longer treated as ambiguous (fixes #1296):
        // they all reuse the baseline status instead of falling back to Pending.
        var branchProvider = new Mock<IGitInfoProvider>();
        var baselineProvider = new Mock<IBaselineProvider>();
        var diffProvider = new Mock<IDiffProvider>();
        var contentMatcher = new Mock<IContentMutantMatcher>();

        var options = new StrykerOptions()
        {
            WithBaseline = true,
            ProjectVersion = "version",
        };
        var file = new CsharpFileLeaf
        {
            RelativePath = "foo.cs",
            SourceCode = "var foo = \"bar\";"
        };

        var mutants = new List<IMutant>
        {
            new Mutant { ResultStatus = MutantStatus.Pending },
            new Mutant { ResultStatus = MutantStatus.Pending }
        };

        var jsonMutants = new HashSet<IJsonMutant>
        {
            new JsonMutant
            {
                Status = "Killed"
            }
        };

        // Setup Mocks
        var jsonReportFileComponent = new MockJsonReportFileComponent("", "var foo = \"bar\";", jsonMutants);

        var jsonFileComponents = new Dictionary<string, ISourceFile>
        {
            ["foo.cs"] = jsonReportFileComponent
        };

        var baseline = new MockJsonReport(null, jsonFileComponents);

        baselineProvider.Setup(mock => mock.Load(It.IsAny<string>()))
            .Returns(Task.FromResult(baseline as IJsonReport));

        diffProvider.Setup(mock => mock.GetContentDiff(jsonReportFileComponent.Source, file.SourceCode))
            .Returns(EmptyContentDiff);
        contentMatcher.Setup(mock => mock.MatchByLocation(mutants, jsonMutants.First(), EmptyContentDiff))
            .Returns(mutants).Verifiable();

        // Act
        var target = new BaselineMutantFilter(options, baselineProvider.Object, branchProvider.Object, diffProvider.Object, contentMatcher.Object);

        var results = target.FilterMutants(mutants, file, options);

        // Assert
        foreach (var result in results)
        {
            result.ResultStatus.ShouldBe(MutantStatus.Killed);
            result.ResultStatusReason.ShouldBe("Result based on previous run");
        }
        results.Count().ShouldBe(2);

        contentMatcher.Verify();
    }

    [TestMethod]
    public void ShouldNotUpdateMutantsWithBaselineIfFileNotInBaseline()
    {
        // Arrange
        var branchProvider = new Mock<IGitInfoProvider>();
        var baselineProvider = new Mock<IBaselineProvider>();

        var options = new StrykerOptions
        {
            WithBaseline = true,
            ProjectVersion = "version"
        };

        var file = new CsharpFileLeaf
        {
            RelativePath = "foo.cs"
        };

        var mutants = new List<IMutant>
        {
            new Mutant()
        };

        // Setup Mocks

        var jsonFileComponents = new Dictionary<string, ISourceFile>();

        var baseline = new MockJsonReport(null, jsonFileComponents);

        baselineProvider.Setup(mock => mock.Load(It.IsAny<string>())).Returns(Task.FromResult((IJsonReport)baseline));

        // Act
        var target = new BaselineMutantFilter(options, baselineProvider.Object, branchProvider.Object);

        var results = target.FilterMutants(mutants, file, options);

        // Assert
        results.ShouldHaveSingleItem();
    }
}
