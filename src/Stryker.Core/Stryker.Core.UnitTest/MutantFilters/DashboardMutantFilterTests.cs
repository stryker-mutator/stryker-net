using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Baseline.Utils;
using Stryker.Core.DashboardCompare;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class DashboardMutantFilterTests
    {
        [Fact]
        public static void ShouldHaveName()
        {
            // Arrange
            var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            var baselineProviderMock = new Mock<IBaselineProvider>(MockBehavior.Loose);
            var baselineMutantHelperMock = new Mock<IBaselineMutantHelper>(MockBehavior.Loose);

            // Act
            var target = new DashboardMutantFilter(new StrykerOptions(), baselineProviderMock.Object, gitInfoProvider.Object, baselineMutantHelperMock.Object) as IMutantFilter;

            // Assert
            target.DisplayName.ShouldBe("dashboard filter");
        }

        [Fact]
        public void GetBaselineCallsFallbackWhenDashboardClientReturnsNull()
        {
            // Arrange
            var baselineProvider = new Mock<IBaselineProvider>();
            var gitInfoProvider = new Mock<IGitInfoProvider>();
            var baselineMutantHelperMock = new Mock<IBaselineMutantHelper>();

            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                compareToDashboard: true,
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters,
               fallbackVersion: "fallback/version");

            var inputComponent = new Mock<IReadOnlyProjectComponent>().Object;

            var jsonReport = JsonReport.Build(options, inputComponent);

            gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");

            baselineProvider.Setup(x => x.Load("dashboard-compare/refs/heads/master")).Returns(Task.FromResult<JsonReport>(null));
            baselineProvider.Setup(x => x.Load("fallback/version")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DashboardMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object);

            // Assert
            baselineProvider.Verify(x => x.Load("dashboard-compare/refs/heads/master"), Times.Once);
            baselineProvider.Verify(x => x.Load("fallback/version"), Times.Once);
        }

        [Fact]
        public void GetBaselineDoesNotCallFallbackWhenDashboardClientReturnsReport()
        {
            // Arrange
            var baselineProvider = new Mock<IBaselineProvider>();
            var gitInfoProvider = new Mock<IGitInfoProvider>();

            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                compareToDashboard: true,
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters,
               fallbackVersion: "fallback/version");

            var inputComponent = new Mock<IReadOnlyProjectComponent>().Object;

            var jsonReport = JsonReport.Build(options, inputComponent);

            gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");

            baselineProvider.Setup(x => x.Load("dashboard-compare/refs/heads/master")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DashboardMutantFilter(options, gitInfoProvider: gitInfoProvider.Object, baselineProvider: baselineProvider.Object);
        }

        [Fact]
        public void FilterMutantsReturnAllMutantsWhenCompareToDashboardEnabledAndBaselineNotAvailabe()
        {
            // Arrange
            var baselineProvider = new Mock<IBaselineProvider>();
            var branchProvider = new Mock<IGitInfoProvider>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var target = new DashboardMutantFilter(options, baselineProvider.Object, branchProvider.Object);

            var file = new Mock<ReadOnlyFileLeaf>(new CsharpFileLeaf());

            var mutants = new List<Mutant>
            {
                new Mutant(),
                new Mutant(),
                new Mutant()
            };

            // Act
            var results = target.FilterMutants(mutants, file.Object, options);

            // Assert
            results.Count().ShouldBe(3);
        }

        [Fact]
        public void FilterMutants_WhenMutantSourceCodeIsNull_MutantIsReturned()
        {
            // Arrange
            var branchProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();
            var baselineMutantHelper = new Mock<IBaselineMutantHelper>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var file = new ReadOnlyFileLeaf(new CsharpFileLeaf
            {
                RelativePath = "foo.cs"
            });

            var mutants = new List<Mutant>
            {
                new Mutant()
            };

            var jsonMutants = new HashSet<JsonMutant>
            {
                new JsonMutant()
            };

            // Setup Mocks
            var jsonReportFileComponent = new MockJsonReportFileComponent("", "", jsonMutants);

            var jsonFileComponents = new Dictionary<string, JsonReportFileComponent>
            {
                ["foo.cs"] =  jsonReportFileComponent
            };

            var baseline = new MockJsonReport(null, jsonFileComponents);

            baselineProvider.Setup(mock => mock.Load(It.IsAny<string>()))
                .Returns(Task.FromResult((JsonReport) baseline));

            baselineMutantHelper.Setup(mock => mock.GetMutantSourceCode(It.IsAny<string>(), It.IsAny<JsonMutant>())).Returns(string.Empty);

            // Act
            var target = new DashboardMutantFilter(options, baselineProvider.Object, branchProvider.Object, baselineMutantHelper.Object);

            var results = target.FilterMutants(mutants, file, options);

            // Assert
            results.ShouldHaveSingleItem();
        }

        [Fact]
        public void FilterMutants_WhenMutantMatchesSourceCode_StatusIsSetToJsonMutant()
        {
            // Arrange
            var branchProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();
            var baselineMutantHelper = new Mock<IBaselineMutantHelper>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var file = new ReadOnlyFileLeaf(new CsharpFileLeaf
            {
                RelativePath = "foo.cs"
            });

            var mutants = new List<Mutant>
            {
                new Mutant
                {
                    ResultStatus = MutantStatus.NotRun
                }
            };

            var jsonMutants = new HashSet<JsonMutant>
            {
                new JsonMutant
                {
                    Status = "Killed"
                }
            };

            // Setup Mocks
            var jsonReportFileComponent = new MockJsonReportFileComponent("", "", jsonMutants);

            var jsonFileComponents = new Dictionary<string, JsonReportFileComponent>
            {
                ["foo.cs"] = jsonReportFileComponent
            };

            var baseline = new MockJsonReport(null, jsonFileComponents);

            baselineProvider.Setup(mock => mock.Load(It.IsAny<string>()))
                .Returns(Task.FromResult(baseline as JsonReport));

            baselineMutantHelper.Setup(mock => mock.GetMutantSourceCode(It.IsAny<string>(), It.IsAny<JsonMutant>())).Returns("var foo = \"bar\";");
            baselineMutantHelper.Setup(mock => mock.GetMutantMatchingSourceCode(
                It.IsAny<IEnumerable<Mutant>>(),
                It.Is<JsonMutant>(m => m == jsonMutants.First()),
                It.Is<string>(source => source == "var foo = \"bar\";"))).Returns(mutants).Verifiable();

            // Act
            var target = new DashboardMutantFilter(options, baselineProvider.Object, branchProvider.Object, baselineMutantHelper.Object);

            var results = target.FilterMutants(mutants, file, options);

            // Assert
            results.ShouldHaveSingleItem().ResultStatus.ShouldBe(MutantStatus.Killed);
            baselineMutantHelper.Verify();
        }

        [Fact]
        public void FilterMutants_WhenMultipleMatchingMutants_ResultIsSetToNotRun()
        {
            // Arrange
            var branchProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();
            var baselineMutantHelper = new Mock<IBaselineMutantHelper>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var file = new ReadOnlyFileLeaf(new CsharpFileLeaf
            {
                RelativePath = "foo.cs"
            });

            var mutants = new List<Mutant>
            {
                new Mutant
                {
                    ResultStatus = MutantStatus.NotRun
                },
                new Mutant
                {
                    ResultStatus = MutantStatus.NotRun
                }
            };

            var jsonMutants = new HashSet<JsonMutant>
            {
                new JsonMutant
                {
                    Status = "Killed"
                }
            };

            // Setup Mocks
            var jsonReportFileComponent = new MockJsonReportFileComponent("", "", jsonMutants);

            var jsonFileComponents = new Dictionary<string, JsonReportFileComponent>
            {
                ["foo.cs"] = jsonReportFileComponent
            };

            var baseline = new MockJsonReport(null, jsonFileComponents);

            baselineProvider.Setup(mock => mock.Load(It.IsAny<string>()))
                .Returns(Task.FromResult(baseline as JsonReport));

            baselineMutantHelper.Setup(mock => mock.GetMutantSourceCode(It.IsAny<string>(), It.IsAny<JsonMutant>())).Returns("var foo = \"bar\";");
            baselineMutantHelper.Setup(mock => mock.GetMutantMatchingSourceCode(
                It.IsAny<IEnumerable<Mutant>>(),
                It.Is<JsonMutant>(m => m == jsonMutants.First()),
                It.Is<string>(source => source == "var foo = \"bar\";"))).Returns(mutants).Verifiable();

            // Act
            var target = new DashboardMutantFilter(options, baselineProvider.Object, branchProvider.Object, baselineMutantHelper.Object);

            var results = target.FilterMutants(mutants, file, options);

            // Assert
            foreach(var result in results)
            {
                result.ResultStatus.ShouldBe(MutantStatus.NotRun);
                result.ResultStatusReason.ShouldBe("Result based on previous run was inconclusive");
            }
            results.Count().ShouldBe(2);

            baselineMutantHelper.Verify();
        }
    }
}
