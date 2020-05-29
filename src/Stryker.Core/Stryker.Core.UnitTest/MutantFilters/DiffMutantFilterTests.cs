using Moq;
using Shouldly;
using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class DiffMutantFilterTests
    {
        [Fact]
        public static void ShouldHaveName()
        {
            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProviderMock = new Mock<IBranchProvider>(MockBehavior.Loose);
            var target = new DiffMutantFilter(new StrykerOptions(), diffProviderMock.Object, branchProvider: branchProviderMock.Object) as IMutantFilter;
            target.DisplayName.ShouldBe("git diff file filter");
        }

        [Fact]
        public void ShouldNotMutateUnchangedFiles()
        {
            var options = new StrykerOptions(diff: true);
            var dashboardClient = new Mock<IDashboardClient>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
            });
            var target = new DiffMutantFilter(options, diffProvider.Object, dashboardClient.Object, branchProvider.Object);
            var file = new FileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            filterResult.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldOnlyMutateChangedFiles()
        {
            var options = new StrykerOptions(diff: true);

            var dashboardClient = new Mock<IDashboardClient>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
                {
                    myFile
                }
            });
            var target = new DiffMutantFilter(options, diffProvider.Object, dashboardClient.Object, branchProvider.Object);
            var file = new FileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void ShouldMutateAllFilesWhenATestHasBeenChanged()
        {
            string testProjectPath = "C:/MyTests";
            var options = new StrykerOptions(diff: false);

            var dashboardClient = new Mock<IDashboardClient>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            // If a file inside the test project is changed, a test has been changed
            string myTest = Path.Combine(testProjectPath, "myTest.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
                {
                    myTest
                },
                TestsChanged = true
            });
            var target = new DiffMutantFilter(options, diffProvider.Object, dashboardClient.Object, branchProvider.Object);

            // check the diff result for a file not inside the test project
            var file = new FileLeaf { FullPath = Path.Combine("C:/NotMyTests", "myfile.cs") };

            var mutant = new Mutant();

            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void GetBaselineCallsFallbackWhenDashboardClientReturnsNull()
        {
            // Arrange 
            var dashboardClient = new Mock<IDashboardClient>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                compareToDashboard: true,
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters,
               fallbackVersion: "fallback/version");

            var inputComponent = new Mock<IReadOnlyInputComponent>().Object;

            var jsonReport = JsonReport.Build(options, inputComponent);

            branchProvider.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");

            dashboardClient.Setup(x => x.PullReport("dashboard-compare/refs/heads/master")).Returns(Task.FromResult<JsonReport>(null));
            dashboardClient.Setup(x => x.PullReport("fallback/version")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DiffMutantFilter(options, dashboardClient: dashboardClient.Object, diffProvider: diffProvider.Object, branchProvider: branchProvider.Object);

            // Assert
            dashboardClient.Verify(x => x.PullReport("dashboard-compare/refs/heads/master"), Times.Once);
            dashboardClient.Verify(x => x.PullReport("fallback/version"), Times.Once);
        }

        [Fact]
        public void GetBaselineDoesNotCallFallbackWhenDashboardClientReturnsReport()
        {
            // Arrange 
            var dashboardClient = new Mock<IDashboardClient>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                compareToDashboard: true,
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters,
               fallbackVersion: "fallback/version");

            var inputComponent = new Mock<IReadOnlyInputComponent>().Object;

            var jsonReport = JsonReport.Build(options, inputComponent);

            branchProvider.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");

            dashboardClient.Setup(x => x.PullReport("dashboard-compare/refs/heads/master")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DiffMutantFilter(options, branchProvider: branchProvider.Object, dashboardClient: dashboardClient.Object, diffProvider: diffProvider.Object);

            // Assert
            dashboardClient.Verify(x => x.PullReport("dashboard-compare/refs/heads/master"), Times.Once);
            dashboardClient.Verify(x => x.PullReport("fallback/version"), Times.Never);
        }


        [Fact]
        public void FilterMutantsReturnAllMutantsWhenCompareToDashboardEnabledAndBaselineNotAvailabe()
        {
            // Arrange 
            var dashboardClient = new Mock<IDashboardClient>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var target = new DiffMutantFilter(options, diffProvider.Object, dashboardClient.Object, branchProvider.Object);

            var file = new Mock<FileLeaf>(MockBehavior.Loose);

            var mutants = new List<Mutant>
            {
                new Mutant(),
                new Mutant(),
                new Mutant()
            };

            var results = target.FilterMutants(mutants, file.Object, options);

            results.Count().ShouldBe(3);
        }

        [Fact]
        public void FilterMutantsForStatusNotRunReturnsAllMutantsWithStatusNotRun()
        {
            // Arrange 
            var dashboardClient = new Mock<IDashboardClient>();

            dashboardClient.Setup(x =>
            x.PullReport(It.IsAny<string>())
            ).Returns(
                Task.FromResult(
                    JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith())
                    ));

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                TestsChanged = false,
                ChangedFiles = new List<string>()
            });

            var target = new DiffMutantFilter(options, diffProvider.Object, dashboardClient.Object, branchProvider.Object);

            var mutants = new List<Mutant>
            {
                new Mutant()
                {
                    ResultStatus = MutantStatus.NotRun
                },
                new Mutant()
                {
                    ResultStatus = MutantStatus.NotRun
                },
                new Mutant()
                {
                    ResultStatus = MutantStatus.Killed
                }
            };

            var results = target.FilterMutants(mutants, new FileLeaf(), options);

            results.Count().ShouldBe(2);
        }

        [Fact]
        public void FilterMutantsFiltersAll_WhenNoTestsChanged_CompareToDashboardDisabled_AndFileNotCahnged()
        {
            // Arrange 
            var dashboardClient = new Mock<IDashboardClient>();

            dashboardClient.Setup(x =>
            x.PullReport(It.IsAny<string>())
            ).Returns(
                Task.FromResult(
                    JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith())
                    ));

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            var options = new StrykerOptions(compareToDashboard: false, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                TestsChanged = false,
                ChangedFiles = new List<string>()
            });

            var target = new DiffMutantFilter(options, diffProvider.Object, dashboardClient.Object, branchProvider.Object);

            var mutants = new List<Mutant>
            {
                new Mutant(),
                new Mutant(),
                new Mutant()
            };

            var results = target.FilterMutants(mutants, new FileLeaf(), options);

            results.Count().ShouldBe(0);
        }

        [Fact]
        public void FilterMutants_FiltersNoMutants_IfTestsChanged()
        {
            // Arrange 
            var dashboardClient = new Mock<IDashboardClient>();

            dashboardClient.Setup(x =>
            x.PullReport(It.IsAny<string>())
            ).Returns(
                Task.FromResult(
                    JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith())
                    ));

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            var options = new StrykerOptions(compareToDashboard: false, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                TestsChanged = true,
                ChangedFiles = new List<string>()
            });

            var target = new DiffMutantFilter(options, diffProvider.Object, dashboardClient.Object, branchProvider.Object);

            var mutants = new List<Mutant>
            {
                new Mutant(),
                new Mutant(),
                new Mutant()
            };

            var results = target.FilterMutants(mutants, new FileLeaf(), options);

            results.Count().ShouldBe(3);
        }
    }
}

