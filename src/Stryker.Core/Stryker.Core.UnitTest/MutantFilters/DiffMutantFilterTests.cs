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
            var target = new DiffMutantFilter(new StrykerOptions(), diffProviderMock.Object) as IMutantFilter;
            target.DisplayName.ShouldBe("git diff file filter");
        }

        [Fact]
        public void ShouldNotMutateUnchangedFiles()
        {
            var options = new StrykerOptions(diff: true);
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);
            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
            });
            var target = new DiffMutantFilter(options, diffProvider.Object);
            var file = new FileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            filterResult.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldOnlyMutateChangedFiles()
        {
            var options = new StrykerOptions(diff: true);
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);
            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
                {
                    myFile
                }
            });
            var target = new DiffMutantFilter(options, diffProvider.Object);
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
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);
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
            var target = new DiffMutantFilter(options, diffProvider.Object);

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

            branchProvider.Setup(x => x.GetCurrentBranchCanonicalName()).Returns("refs/heads/master");

            dashboardClient.Setup(x => x.PullReport("refs/heads/master")).Returns(Task.FromResult<JsonReport>(null));
            dashboardClient.Setup(x => x.PullReport("fallback/version")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DiffMutantFilter(options, dashboardClient: dashboardClient.Object, diffProvider: diffProvider.Object, branchProvider: branchProvider.Object);

            // Assert
            dashboardClient.Verify(x => x.PullReport("refs/heads/master"), Times.Once);
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

            branchProvider.Setup(x => x.GetCurrentBranchCanonicalName()).Returns("refs/heads/master");

            dashboardClient.Setup(x => x.PullReport("refs/heads/master")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DiffMutantFilter(options, branchProvider: branchProvider.Object, dashboardClient: dashboardClient.Object, diffProvider: diffProvider.Object);

            // Assert
            dashboardClient.Verify(x => x.PullReport("refs/heads/master"), Times.Once);
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

            var mutants = new List<Mutant>
            {
                new Mutant(),
                new Mutant(),
                new Mutant()
            };

            var results = target.FilterMutants(mutants, null, options);

            results.Count().ShouldBe(3);
        }
    }
}
