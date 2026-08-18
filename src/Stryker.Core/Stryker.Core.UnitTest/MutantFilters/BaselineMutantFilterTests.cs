using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Abstractions.Testing;
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
using Stryker.Core.Reporters.Json.TestFiles;
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

        gitInfoProvider.SetupGet(x => x.IsRepository).Returns(true);
        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns(branchName);

        baselineProvider.Setup(x => x.Load($"baseline/{options.ProjectVersion}")).Returns(Task.FromResult<IJsonReport>(null));
        baselineProvider.Setup(x => x.Load($"baseline/{options.FallbackVersion}")).Returns(Task.FromResult(jsonReport));

        // Act
        var target = new BaselineMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object);

        // Assert
        baselineProvider.Verify(x => x.Load($"baseline/{options.ProjectVersion}"), Times.Once);
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

        gitInfoProvider.SetupGet(x => x.IsRepository).Returns(true);
        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns(branchName);

        baselineProvider.Setup(x => x.Load(options.ProjectVersion)).Returns(Task.FromResult<IJsonReport>(null));
        baselineProvider.Setup(x => x.Load($"baseline/{options.FallbackVersion}")).Returns(Task.FromResult<IJsonReport>(null));
        baselineProvider.Setup(x => x.Load(options.FallbackVersion)).Returns(Task.FromResult(jsonReport));

        // Act
        var target = new BaselineMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object);

        // Assert
        baselineProvider.Verify(x => x.Load($"baseline/{options.ProjectVersion}"), Times.Once);
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

        gitInfoProvider.SetupGet(x => x.IsRepository).Returns(true);
        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns(branchName);

        baselineProvider.Setup(x => x.Load($"baseline/{options.ProjectVersion}")).Returns(Task.FromResult(jsonReport));

        // Act
        var target = new BaselineMutantFilter(options, gitInfoProvider: gitInfoProvider.Object, baselineProvider: baselineProvider.Object);

        // Assert
        baselineProvider.Verify(x => x.Load($"baseline/{options.ProjectVersion}"), Times.Once);
        baselineProvider.Verify(x => x.Load($"baseline/{options.FallbackVersion}"), Times.Never);
        baselineProvider.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void GetBaseline_UsesBranchName_WhenProjectVersionIsEmpty()
    {
        var baselineProvider = new Mock<IBaselineProvider>();
        var gitInfoProvider = new Mock<IGitInfoProvider>();
        gitInfoProvider.SetupGet(x => x.IsRepository).Returns(true);
        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("feature/branch");

        var options = new StrykerOptions
        {
            WithBaseline = true,
            FallbackVersion = "fallback/version"
        };

        var jsonReport = JsonReport.Build(options, new Mock<IReadOnlyProjectComponent>().Object, It.IsAny<TestProjectsInfo>());
        baselineProvider.Setup(x => x.Load("baseline/feature/branch")).Returns(Task.FromResult(jsonReport));

        _ = new BaselineMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object);

        baselineProvider.Verify(x => x.Load("baseline/feature/branch"), Times.Once);
        baselineProvider.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void GetBaseline_UsesCompareVersion_InsteadOfCurrentVersion_WhenConfigured()
    {
        var baselineProvider = new Mock<IBaselineProvider>();
        var gitInfoProvider = new Mock<IGitInfoProvider>();
        gitInfoProvider.SetupGet(x => x.IsRepository).Returns(true);
        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("feature/branch");

        var options = new StrykerOptions
        {
            WithBaseline = true,
            ProjectVersion = "feature/branch",
            BaselineCompareVersion = "development",
            FallbackVersion = "master"
        };

        var jsonReport = JsonReport.Build(options, new Mock<IReadOnlyProjectComponent>().Object, It.IsAny<TestProjectsInfo>());
        baselineProvider.Setup(x => x.Load("baseline/development")).Returns(Task.FromResult(jsonReport));

        _ = new BaselineMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object);

        baselineProvider.Verify(x => x.Load("baseline/development"), Times.Once);
        baselineProvider.Verify(x => x.Load("baseline/feature/branch"), Times.Never);
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

    [TestMethod]
    public void FilterMutants_ReusesStatus_WhenCoveringTestIsUnchanged_AndCachesTestFileDiff()
    {
        var baselineProvider = new Mock<IBaselineProvider>();
        var diffProvider = new Mock<IDiffProvider>();
        var contentMatcher = new Mock<IContentMutantMatcher>();
        var contentTestMatcher = new Mock<IContentTestMatcher>();
        var options = new StrykerOptions
        {
            WithBaseline = true,
            ProjectVersion = "version"
        };
        var source = "class Source { }";
        var testSource = "class Tests { void Test() { Assert.IsTrue(true); } }";
        var baselineMutants = new HashSet<IJsonMutant>
        {
            new JsonMutant { Status = "Killed", CoveredBy = ["test-id"] },
            new JsonMutant { Status = "Killed", CoveredBy = ["test-id"] }
        };
        var baselineTestFile = new JsonTestFile
        {
            Source = testSource,
            Tests = new HashSet<IJsonTest> { new JsonTest("test-id") }
        };
        var baseline = new MockJsonReport(null, new Dictionary<string, ISourceFile>
        {
            ["foo.cs"] = new MockJsonReportFileComponent("", source, baselineMutants)
        })
        {
            TestFiles = new Dictionary<string, IJsonTestFile> { ["tests.cs"] = baselineTestFile }
        };
        var currentTest = CreateCurrentTest(testSource, "test-id");
        var currentTestFile = new Mock<ITestFile>();
        currentTestFile.SetupGet(x => x.RelativePath).Returns("tests.cs");
        currentTestFile.SetupGet(x => x.Source).Returns(testSource);
        currentTestFile.SetupGet(x => x.Tests).Returns(new List<Abstractions.ProjectComponents.ITestCase> { currentTest });
        var testProjectsInfo = new Mock<ITestProjectsInfo>();
        testProjectsInfo.SetupGet(x => x.TestFiles).Returns(new[] { currentTestFile.Object });
        var gitInfoProvider = new Mock<IGitInfoProvider>();
        var baselineFile = new CsharpFileLeaf { RelativePath = "foo.cs", SourceCode = source };
        var mutants = baselineMutants.Select(_ => (IMutant)new Mutant { ResultStatus = MutantStatus.Pending }).ToList();

        baselineProvider.Setup(x => x.Load("baseline/version")).ReturnsAsync(baseline);
        diffProvider.Setup(x => x.GetContentDiff(source, source)).Returns(EmptyContentDiff);
        diffProvider.Setup(x => x.GetContentDiff(testSource, testSource)).Returns(EmptyContentDiff);
        contentMatcher.Setup(x => x.MatchByLocation(It.IsAny<IEnumerable<IMutant>>(), It.IsAny<IJsonMutant>(), EmptyContentDiff)).Returns(mutants);
        contentTestMatcher.Setup(x => x.IsTestUnchanged(It.IsAny<IJsonTest>(), currentTest, EmptyContentDiff)).Returns(true);

        var target = new BaselineMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object,
            diffProvider.Object, contentMatcher.Object, testProjectsInfo.Object, contentTestMatcher.Object);

        target.FilterMutants(mutants, baselineFile, options).ToList();

        mutants.ShouldAllBe(mutant => mutant.ResultStatus == MutantStatus.Killed);
        diffProvider.Verify(x => x.GetContentDiff(source, source), Times.Once);
        diffProvider.Verify(x => x.GetContentDiff(testSource, testSource), Times.Once);
        contentTestMatcher.Verify(x => x.IsTestUnchanged(It.IsAny<IJsonTest>(), currentTest, EmptyContentDiff), Times.Exactly(2));
    }

    [TestMethod]
    public void FilterMutants_ResetsMutantsToPending_WhenCoveringTestIsChanged()
    {
        var baselineProvider = new Mock<IBaselineProvider>();
        var diffProvider = new Mock<IDiffProvider>();
        var contentMatcher = new Mock<IContentMutantMatcher>();
        var contentTestMatcher = new Mock<IContentTestMatcher>();
        var options = new StrykerOptions { WithBaseline = true, ProjectVersion = "version" };
        var source = "class Tests { void Test() { Assert.IsTrue(true); } }";
        var baselineMutant = new JsonMutant { Status = "Killed", CoveredBy = ["test-id"] };
        var baselineTestFile = new JsonTestFile
        {
            Source = source,
            Tests = new HashSet<IJsonTest> { new JsonTest("test-id") }
        };
        var baseline = new MockJsonReport(null, new Dictionary<string, ISourceFile>
        {
            ["foo.cs"] = new MockJsonReportFileComponent("", source, new HashSet<IJsonMutant> { baselineMutant })
        })
        {
            TestFiles = new Dictionary<string, IJsonTestFile> { ["tests.cs"] = baselineTestFile }
        };
        var currentTest = CreateCurrentTest(source, "test-id");
        var currentTestFile = new Mock<ITestFile>();
        currentTestFile.SetupGet(x => x.RelativePath).Returns("tests.cs");
        currentTestFile.SetupGet(x => x.Source).Returns(source);
        currentTestFile.SetupGet(x => x.Tests).Returns(new List<Abstractions.ProjectComponents.ITestCase> { currentTest });
        var testProjectsInfo = new Mock<ITestProjectsInfo>();
        testProjectsInfo.SetupGet(x => x.TestFiles).Returns(new[] { currentTestFile.Object });
        var gitInfoProvider = new Mock<IGitInfoProvider>();
        var diff = new DiffResult([], source, source);
        var file = new CsharpFileLeaf { RelativePath = "foo.cs", SourceCode = source };
        var mutant = new Mutant { ResultStatus = MutantStatus.Killed, ResultStatusReason = "old result" };

        baselineProvider.Setup(x => x.Load("baseline/version")).ReturnsAsync(baseline);
        diffProvider.Setup(x => x.GetContentDiff(source, source)).Returns(diff);
        contentMatcher.Setup(x => x.MatchByLocation(It.IsAny<IEnumerable<IMutant>>(), baselineMutant, diff)).Returns(new[] { mutant });
        contentTestMatcher.Setup(x => x.IsTestUnchanged(It.IsAny<IJsonTest>(), currentTest, diff)).Returns(false);

        var target = new BaselineMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object,
            diffProvider.Object, contentMatcher.Object, testProjectsInfo.Object, contentTestMatcher.Object);

        target.FilterMutants(new[] { mutant }, file, options).ToList();

        mutant.ResultStatus.ShouldBe(MutantStatus.Pending);
        mutant.ResultStatusReason.ShouldBe("One or more covering tests changed since the previous run");
    }

    [TestMethod]
    public void FilterMutants_ResetsMutantsToPending_WhenCoveringTestIsMissing()
    {
        var baselineProvider = new Mock<IBaselineProvider>();
        var diffProvider = new Mock<IDiffProvider>();
        var contentMatcher = new Mock<IContentMutantMatcher>();
        var options = new StrykerOptions { WithBaseline = true, ProjectVersion = "version" };
        var source = "class Tests { void Test() { Assert.IsTrue(true); } }";
        var baselineMutant = new JsonMutant { Status = "Killed", CoveredBy = ["removed-test"] };
        var baseline = new MockJsonReport(null, new Dictionary<string, ISourceFile>
        {
            ["foo.cs"] = new MockJsonReportFileComponent("", source, new HashSet<IJsonMutant> { baselineMutant })
        })
        {
            TestFiles = new Dictionary<string, IJsonTestFile>
            {
                ["tests.cs"] = new JsonTestFile
                {
                    Source = "class Tests { }",
                    Tests = new HashSet<IJsonTest> { new JsonTest("removed-test") }
                }
            }
        };
        var gitInfoProvider = new Mock<IGitInfoProvider>();
        var testProjectsInfo = new Mock<ITestProjectsInfo>();
        testProjectsInfo.SetupGet(x => x.TestFiles).Returns(Enumerable.Empty<ITestFile>());
        var file = new CsharpFileLeaf { RelativePath = "foo.cs", SourceCode = source };
        var mutant = new Mutant { ResultStatus = MutantStatus.Killed };

        baselineProvider.Setup(x => x.Load("baseline/version")).ReturnsAsync(baseline);
        contentMatcher.Setup(x => x.MatchByLocation(It.IsAny<IEnumerable<IMutant>>(), baselineMutant, EmptyContentDiff)).Returns(new[] { mutant });
        diffProvider.Setup(x => x.GetContentDiff(source, source)).Returns(EmptyContentDiff);

        var target = new BaselineMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object,
            diffProvider.Object, contentMatcher.Object, testProjectsInfo.Object);

        target.FilterMutants(new[] { mutant }, file, options).ToList();

        mutant.ResultStatus.ShouldBe(MutantStatus.Pending);
        mutant.ResultStatusReason.ShouldBe("One or more covering tests changed since the previous run");
    }

    private static Abstractions.ProjectComponents.ITestCase CreateCurrentTest(string source, string id)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var node = tree.GetRoot().DescendantNodes().First();
        var testCase = new Mock<Abstractions.ProjectComponents.ITestCase>();
        testCase.SetupGet(x => x.Id).Returns(id);
        testCase.SetupGet(x => x.Node).Returns(node);
        return testCase.Object;
    }
}
