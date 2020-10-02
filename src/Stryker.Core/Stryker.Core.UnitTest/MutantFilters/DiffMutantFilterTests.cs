using DotNet.Globbing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Shouldly;
using Stryker.Core.Baseline;
using Stryker.Core.DashboardCompare;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using Stryker.Core.UnitTest.Reporters;
using System;
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

            var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Loose);

            var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

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

            var target = new DiffMutantFilter(new StrykerOptions(diff: false), diffProviderMock.Object, baselineProvider.Object, gitInfoProvider.Object);

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

            var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Loose);

            var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

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

            var target = new DiffMutantFilter(new StrykerOptions(diff: false), diffProviderMock.Object, baselineProvider.Object, gitInfoProvider.Object);

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

            var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Loose);

            var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

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

            var target = new DiffMutantFilter(new StrykerOptions(diff: false), diffProviderMock.Object, baselineProvider.Object, gitInfoProvider.Object);

            // Act
            var result = target.GetMutantSourceCode(source, jsonMutant);

            // Assert
            result.ShouldBe("\"\\n\"");

        }

        [Fact]
        public static void ShouldHaveName()
        {
            // Arrange
            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
            var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Loose);

            // Act
            var target = new DiffMutantFilter(new StrykerOptions(), diffProviderMock.Object, gitInfoProvider: gitInfoProvider.Object) as IMutantFilter;

            // Assert
            target.DisplayName.ShouldBe("git diff file filter");
        }

        [Fact]
        public void ShouldNotMutateUnchangedFiles()
        {
            // Arrange
            var options = new StrykerOptions(diff: true);
            var baselineProvider = new Mock<IBaselineProvider>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IGitInfoProvider>();

            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>(),
                TestFilesChanged = new Collection<string>()
            });
            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, branchProvider.Object);
            var file = new FileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            // Act
            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            // Assert
            filterResult.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldOnlyMutateChangedFiles()
        {
            // Arrange
            var options = new StrykerOptions(diff: true);

            var baselineProvider = new Mock<IBaselineProvider>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IGitInfoProvider>();

            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
                {
                    myFile
                }
            });
            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, branchProvider.Object);
            var file = new FileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            // Act
            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            // Assert
            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void ShouldNotFilterMutantsWhereCoveringTestsContainsChangedTestFile()
        {
            // Arrange
            string testProjectPath = "C:/MyTests";
            var options = new StrykerOptions(diff: false);

            var baselineProvider = new Mock<IBaselineProvider>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IGitInfoProvider>();

            // If a file inside the test project is changed, a test has been changed
            string myTest = Path.Combine(testProjectPath, "myTest.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
                {
                    myTest
                },
                TestFilesChanged = new Collection<string>() {
                    myTest
                }
            });
            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, branchProvider.Object);

            // check the diff result for a file not inside the test project
            var file = new FileLeaf { FullPath = Path.Combine("C:/NotMyTests", "myfile.cs") };

            var mutant = new Mutant();

            mutant.CoveringTests.Add(new TestDescription(Guid.NewGuid().ToString(), "name", myTest));

            // Act
            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            // Assert
            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void GetBaselineCallsFallbackWhenDashboardClientReturnsNull()
        {
            // Arrange 
            var baselineProvider = new Mock<IBaselineProvider>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
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

            var inputComponent = new Mock<IReadOnlyInputComponent>().Object;

            var jsonReport = JsonReport.Build(options, inputComponent);

            gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");

            baselineProvider.Setup(x => x.Load("dashboard-compare/refs/heads/master")).Returns(Task.FromResult<JsonReport>(null));
            baselineProvider.Setup(x => x.Load("fallback/version")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DiffMutantFilter(options, baselineProvider: baselineProvider.Object, diffProvider: diffProvider.Object, gitInfoProvider: gitInfoProvider.Object);

            // Assert
            baselineProvider.Verify(x => x.Load("dashboard-compare/refs/heads/master"), Times.Once);
            baselineProvider.Verify(x => x.Load("fallback/version"), Times.Once);
        }

        [Fact]
        public void GetBaselineDoesNotCallFallbackWhenDashboardClientReturnsReport()
        {
            // Arrange 
            var baselineProvider = new Mock<IBaselineProvider>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
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

            var inputComponent = new Mock<IReadOnlyInputComponent>().Object;

            var jsonReport = JsonReport.Build(options, inputComponent);

            gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");

            baselineProvider.Setup(x => x.Load("dashboard-compare/refs/heads/master")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DiffMutantFilter(options, gitInfoProvider: gitInfoProvider.Object, baselineProvider: baselineProvider.Object, diffProvider: diffProvider.Object);

            // Assert
            baselineProvider.Verify(x => x.Load("dashboard-compare/refs/heads/master"), Times.Once);
            baselineProvider.Verify(x => x.Load("fallback/version"), Times.Never);
        }


        [Fact]
        public void FilterMutantsReturnAllMutantsWhenCompareToDashboardEnabledAndBaselineNotAvailabe()
        {
            // Arrange 
            var baselineProvider = new Mock<IBaselineProvider>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IGitInfoProvider>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, branchProvider.Object);

            var file = new Mock<FileLeaf>(MockBehavior.Loose);

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
        public void FilterMutantsForStatusNotRunReturnsAllMutantsWithStatusNotRun()
        {
            // Arrange 
            var baselineProvider = new Mock<IBaselineProvider>();

            baselineProvider.Setup(x => x.Load(It.IsAny<string>())).ReturnsAsync(JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith()));

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IGitInfoProvider>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                ChangedFiles = new List<string>()
            });

            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, branchProvider.Object);

            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = Mutator.Arithmetic
            };

            var mutants = new List<Mutant>
            {
                new Mutant()
                {
                    Id = 1,
                    ResultStatus = MutantStatus.NotRun,
                    Mutation = mutation
                },
                new Mutant()
                {
                    Id = 2,
                    ResultStatus = MutantStatus.NotRun,
                    Mutation = mutation
                },
                new Mutant()
                {
                    Id = 3,
                    ResultStatus = MutantStatus.Killed,
                    Mutation = mutation
                }
            };

            // Act
            var results = target.FilterMutants(mutants, new FileLeaf() { RelativePath = "src/1/SomeFile0.cs" }, options);

            // Assert
            results.Count().ShouldBe(3);
        }

        [Fact]
        public void FilterMutantsFiltersAll_WhenNoTestsChanged_CompareToDashboardDisabled_AndFileNotCahnged()
        {
            // Arrange 
            var baselineProvider = new Mock<IBaselineProvider>();

            baselineProvider.Setup(x =>
            x.Load(It.IsAny<string>())
            ).Returns(
                Task.FromResult(
                    JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith())
                    ));

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IGitInfoProvider>();

            var options = new StrykerOptions(compareToDashboard: false, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                ChangedFiles = new List<string>(),
            });

            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, branchProvider.Object);

            var mutants = new List<Mutant>
            {
                new Mutant(),
                new Mutant(),
                new Mutant()
            };

            // Act
            var results = target.FilterMutants(mutants, new FileLeaf(), options);

            // Assert
            results.Count().ShouldBe(0);
        }

        [Fact]
        public void FilterMutants_FiltersNoMutants_IfTestsChanged()
        {
            // Arrange 
            var baselineProvider = new Mock<IBaselineProvider>();

            baselineProvider.Setup(x =>
            x.Load(It.IsAny<string>())
            ).Returns(
                Task.FromResult(
                    JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith())
                    ));

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var gitInfoProvider = new Mock<IGitInfoProvider>();

            var options = new StrykerOptions(compareToDashboard: false, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                ChangedFiles = new List<string>(),
                TestFilesChanged = new List<string> { "C:/testfile.cs" }
            });

            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, gitInfoProvider.Object);

            var testDescriptions = new List<TestDescription> { new TestDescription(Guid.NewGuid().ToString(), "name", "C:/testfile.cs") };
            var testListDescription = new TestListDescription(testDescriptions);

            var mutants = new List<Mutant>
            {
                new Mutant {
                    CoveringTests = testListDescription
                },
                new Mutant(),
                new Mutant()
            };

            // Act
            var results = target.FilterMutants(mutants, new FileLeaf(), options);

            // Assert
            results.Count().ShouldBe(1);
        }
    }
}

