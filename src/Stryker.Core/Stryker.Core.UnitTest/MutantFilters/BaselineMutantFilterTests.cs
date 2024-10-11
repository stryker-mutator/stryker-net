using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Baseline.Utils;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Core.UnitTest.Reporters.Json;

namespace Stryker.Core.UnitTest.MutantFilters
{
    [TestClass]
    public class BaselineMutantFilterTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveName()
        {
            // Arrange
            var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            var baselineProviderMock = new Mock<IBaselineProvider>(MockBehavior.Loose);
            var baselineMutantHelperMock = new Mock<IBaselineMutantHelper>(MockBehavior.Loose);

            // Act
            var target = new BaselineMutantFilter(new StrykerOptions(), baselineProviderMock.Object, gitInfoProvider.Object, baselineMutantHelperMock.Object) as IMutantFilter;

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
            var baselineMutantHelperMock = new Mock<IBaselineMutantHelper>();

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
            var baselineMutantHelperMock = new Mock<IBaselineMutantHelper>();

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
        public void FilterMutants_WhenMutantSourceCodeIsNull_MutantIsReturned()
        {
            // Arrange
            var branchProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();
            var baselineMutantHelper = new Mock<IBaselineMutantHelper>();

            var options = new StrykerOptions()
            {
                WithBaseline = true,
                ProjectVersion = "version",
            };
            var file = new CsharpFileLeaf
            {
                RelativePath = "foo.cs"
            };

            var mutants = new List<Mutant>
            {
                new Mutant()
            };

            var jsonMutants = new HashSet<IJsonMutant>
            {
                new JsonMutant()
            };

            // Setup Mocks
            var jsonReportFileComponent = new MockJsonReportFileComponent("", "", jsonMutants);

            var jsonFileComponents = new Dictionary<string, ISourceFile>
            {
                ["foo.cs"] = jsonReportFileComponent
            };

            var baseline = new MockJsonReport(null, jsonFileComponents);

            baselineProvider.Setup(mock => mock.Load(It.IsAny<string>()))
                .Returns(Task.FromResult((IJsonReport)baseline));

            baselineMutantHelper.Setup(mock => mock.GetMutantSourceCode(It.IsAny<string>(), It.IsAny<JsonMutant>())).Returns(string.Empty);

            // Act
            var target = new BaselineMutantFilter(options, baselineProvider.Object, branchProvider.Object, baselineMutantHelper.Object);

            var results = target.FilterMutants(mutants, file, options);

            // Assert
            results.ShouldHaveSingleItem();
        }

        [TestMethod]
        public void FilterMutants_WhenMutantMatchesSourceCode_StatusIsSetToJsonMutant()
        {
            // Arrange
            var branchProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();
            var baselineMutantHelper = new Mock<IBaselineMutantHelper>();

            var options = new StrykerOptions()
            {
                WithBaseline = true,
                ProjectVersion = "version",
            };
            var file = new CsharpFileLeaf
            {
                RelativePath = "foo.cs"
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
            var jsonReportFileComponent = new MockJsonReportFileComponent("", "", jsonMutants);

            var jsonFileComponents = new Dictionary<string, ISourceFile>
            {
                ["foo.cs"] = jsonReportFileComponent
            };

            var baseline = new MockJsonReport(null, jsonFileComponents);

            baselineProvider.Setup(mock => mock.Load(It.IsAny<string>()))
                .Returns(Task.FromResult(baseline as IJsonReport));

            baselineMutantHelper.Setup(mock => mock.GetMutantSourceCode(It.IsAny<string>(), It.IsAny<IJsonMutant>())).Returns("var foo = \"bar\";");
            baselineMutantHelper.Setup(mock => mock.GetMutantMatchingSourceCode(
                It.IsAny<IEnumerable<IMutant>>(),
                It.Is<IJsonMutant>(m => m == jsonMutants.First()),
                It.Is<string>(source => source == "var foo = \"bar\";"))).Returns(mutants).Verifiable();

            // Act
            var target = new BaselineMutantFilter(options, baselineProvider.Object, branchProvider.Object, baselineMutantHelper.Object);

            var results = target.FilterMutants(mutants, file, options);

            // Assert
            results.ShouldHaveSingleItem().ResultStatus.ShouldBe(MutantStatus.Killed);
            baselineMutantHelper.Verify();
        }

        [TestMethod]
        public void FilterMutants_WhenMultipleMatchingMutants_ResultIsSetToNotRun()
        {
            // Arrange
            var branchProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();
            var baselineMutantHelper = new Mock<IBaselineMutantHelper>();

            var options = new StrykerOptions()
            {
                WithBaseline = true,
                ProjectVersion = "version",
            };
            var file = new CsharpFileLeaf
            {
                RelativePath = "foo.cs"
            };

            var mutants = new List<IMutant>
            {
                new Mutant
                {
                    ResultStatus = MutantStatus.Pending
                },
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
            var jsonReportFileComponent = new MockJsonReportFileComponent("", "", jsonMutants);

            var jsonFileComponents = new Dictionary<string, ISourceFile>
            {
                ["foo.cs"] = jsonReportFileComponent
            };

            var baseline = new MockJsonReport(null, jsonFileComponents);

            baselineProvider.Setup(mock => mock.Load(It.IsAny<string>()))
                .Returns(Task.FromResult(baseline as IJsonReport));

            baselineMutantHelper.Setup(mock => mock.GetMutantSourceCode(It.IsAny<string>(), It.IsAny<IJsonMutant>())).Returns("var foo = \"bar\";");
            baselineMutantHelper.Setup(mock => mock.GetMutantMatchingSourceCode(
                It.IsAny<IEnumerable<IMutant>>(),
                It.Is<IJsonMutant>(m => m == jsonMutants.First()),
                It.Is<string>(source => source == "var foo = \"bar\";"))).Returns(mutants).Verifiable();

            // Act
            var target = new BaselineMutantFilter(options, baselineProvider.Object, branchProvider.Object, baselineMutantHelper.Object);

            var results = target.FilterMutants(mutants, file, options);

            // Assert
            foreach (var result in results)
            {
                result.ResultStatus.ShouldBe(MutantStatus.Pending);
                result.ResultStatusReason.ShouldBe("Result based on previous run was inconclusive");
            }
            results.Count().ShouldBe(2);

            baselineMutantHelper.Verify();
        }

        [TestMethod]
        public void ShouldNotUpdateMutantsWithBaselineIfFileNotInBaseline()
        {
            // Arrange
            var branchProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();
            var baselineMutantHelper = new Mock<IBaselineMutantHelper>();

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

            var jsonMutants = new HashSet<IJsonMutant>
            {
                new JsonMutant()
            };

            // Setup Mocks

            var jsonFileComponents = new Dictionary<string, ISourceFile>();

            var baseline = new MockJsonReport(null, jsonFileComponents);

            baselineProvider.Setup(mock => mock.Load(It.IsAny<string>())).Returns(Task.FromResult((IJsonReport)baseline));

            // Act
            var target = new BaselineMutantFilter(options, baselineProvider.Object, branchProvider.Object, baselineMutantHelper.Object);

            var results = target.FilterMutants(mutants, file, options);

            // Assert
            results.ShouldHaveSingleItem();
        }
    }
}
