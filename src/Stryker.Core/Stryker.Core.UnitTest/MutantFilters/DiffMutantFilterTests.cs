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
        public void GetMutantSourceShouldReturnMutantSource()
        {
            // Arrange
            var path = $"TestResources{Path.DirectorySeparatorChar}ExampleSourceFile.cs";

            var file = new FileInfo(path);

            var source = File.ReadAllText(file.FullName);

            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);

            var jsonMutant = new JsonMutant
            {
                Location = new JsonMutantLocation(new JsonMutantPosition
                {
                    Column = 17,
                    Line = 17
                },
                new JsonMutantPosition
                {
                    Column = 62,
                    Line = 17
                }),
            };

            var target = new DiffMutantFilter(new StrykerOptions(diff: false), diffProviderMock.Object);

            // Act
            var result = target.GetMutantSourceCode(source, jsonMutant);

            // Assert
            result.ShouldBe("return Fibonacci(b, a + b, counter + 1, len);");
        }

        [Fact]
        public void GetMutantSourceShouldReturnMutantSource_When_Multiple_Lines()
        {
            // Arrange
            var path = $"TestResources{Path.DirectorySeparatorChar}ExampleSourceFile.cs";

            var file = new FileInfo(path);

            var source = File.ReadAllText(file.FullName);

            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);

            var jsonMutant = new JsonMutant
            {
                Location = new JsonMutantLocation(new JsonMutantPosition
                {
                    Column = 13,
                    Line = 24
                },
                new JsonMutantPosition
                {
                    Column = 38,
                    Line = 26
                }),
            };

            var target = new DiffMutantFilter(new StrykerOptions(diff: false), diffProviderMock.Object);

            // Act
            var result = target.GetMutantSourceCode(source, jsonMutant);

            // Assert
            result.ShouldBe(@"return @""Lorem Ipsum
                    Dolor Sit Amet
                    Lorem Dolor Sit"";");
        }

        [Fact]
        public void GetMutantSource_Gets_Partial_Line()
        {
            // Arrange
            var path = $"TestResources{Path.DirectorySeparatorChar}ExampleSourceFile.cs";

            var file = new FileInfo(path);

            var source = File.ReadAllText(file.FullName);

            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);

            var jsonMutant = new JsonMutant
            {
                Location = new JsonMutantLocation(new JsonMutantPosition
                {
                    Column = 30,
                    Line = 34
                },
                new JsonMutantPosition
                {
                    Column = 34,
                    Line = 34
                }),
            };

            var target = new DiffMutantFilter(new StrykerOptions(diff: false), diffProviderMock.Object);

            // Act
            var result = target.GetMutantSourceCode(source, jsonMutant);

            // Assert
            result.ShouldBe("\"\\n\"");

        }

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

            var filterResult = target.FilterMutants( new List<Mutant>() { mutant }, file, options);

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
        public async Task GetFallbackBaselineReturnsBaseline()
        {
            // Arrange 
            var dashboardClient = new Mock<IDashboardClient>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

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

            dashboardClient.Setup(x => x.PullReport("fallback/version")).Returns(Task.FromResult(jsonReport));

            var target = new DiffMutantFilter(options, dashboardClient: dashboardClient.Object, diffProvider: diffProvider.Object);

            // Act
            var result  = await target.GetFallbackBaseline();

            // Assert
            result.ShouldBe(jsonReport);
        }

        [Fact]
        public async Task GetFallBackBaselineReturnsNullWhenDashboardClientReturnsNull()
        {
            // Arrange 
            var dashboardClient = new Mock<IDashboardClient>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                compareToDashboard: true,
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters,
               fallbackVersion: "fallback/version");

            dashboardClient.Setup(x => x.PullReport("fallback/version")).Returns(Task.FromResult<JsonReport>(null));

            var target = new DiffMutantFilter(options, dashboardClient: dashboardClient.Object, diffProvider: diffProvider.Object);

            // Act
            var result = await target.GetFallbackBaseline();

            // Assert
            result.ShouldBe(null);
        }

        [Fact]
        public async Task GetBackBaselineReturnsFallbackWhenDashboardClientReturnsNull()
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

            var target = new DiffMutantFilter(options, dashboardClient: dashboardClient.Object, diffProvider: diffProvider.Object);

            // Act
            var result = await target.GetBaseline();

            // Assert
            result.ShouldBe(jsonReport);
        }

        [Fact]
        public async Task GetBaselineReturnsWhenDashboardClientReturnsReport()
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

            var target = new DiffMutantFilter(options, branchProvider: branchProvider.Object, dashboardClient: dashboardClient.Object, diffProvider: diffProvider.Object);

            // Act
            var result = await target.GetBaseline();

            // Assert
            result.ShouldBe(jsonReport);
        }


        [Fact]
        public void FilterMutantsReturnAllMutantsWhenCompareToDashboardEnabledAndBaselineNotAvailabe()
        {
            // Arrange 
            var dashboardClient = new Mock<IDashboardClient>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IBranchProvider>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var target = new DiffMutantFilter(options, diffProvider.Object, dashboardClient.Object, branchProvider.Object );

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
