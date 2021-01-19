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
                ChangedSourceFiles = new Collection<string>(),
                ChangedTestFiles = new Collection<string>()
            });
            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, branchProvider.Object);
            var file = new CsharpFileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            // Act
            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file.ToReadOnly(), options);

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
                ChangedSourceFiles = new Collection<string>()
                {
                    myFile
                }
            });
            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, branchProvider.Object);
            var file = new CsharpFileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            // Act
            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file.ToReadOnly(), options);

            // Assert
            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void ShouldNotFilterMutantsWhereCoveringTestsContainsChangedTestFile()
        {
            // Arrange
            string testProjectPath = "C:/MyTests";
            var options = new StrykerOptions();

            var baselineProvider = new Mock<IBaselineProvider>();
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProvider = new Mock<IGitInfoProvider>();

            // If a file inside the test project is changed, a test has been changed
            string myTest = Path.Combine(testProjectPath, "myTest.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedSourceFiles = new Collection<string>()
                {
                    myTest
                },
                ChangedTestFiles = new Collection<string>() {
                    myTest
                }
            });
            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, branchProvider.Object);

            // check the diff result for a file not inside the test project
            var file = new CsharpFileLeaf { FullPath = Path.Combine("C:/NotMyTests", "myfile.cs") };

            var mutant = new Mutant();

            mutant.CoveringTests.Add(new TestDescription(Guid.NewGuid().ToString(), "name", myTest));

            // Act
            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file.ToReadOnly(), options);

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

            var inputComponent = new Mock<IReadOnlyProjectComponent>().Object;

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

            var inputComponent = new Mock<IReadOnlyProjectComponent>().Object;

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
        public void FilterMutantsWithNoChangedFilesReturnsEmptyList()
        {
            // Arrange
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);

            var options = new StrykerOptions();

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                ChangedSourceFiles = new List<string>()
            });

            var target = new DiffMutantFilter(options, diffProvider.Object, new Mock<IBaselineProvider>().Object, new Mock<IGitInfoProvider>().Object);

            var mutants = new List<Mutant>
            {
                new Mutant()
                {
                    Id = 1,
                    Mutation = new Mutation()
                },
                new Mutant()
                {
                    Id = 2,
                    Mutation = new Mutation()
                },
                new Mutant()
                {
                    Id = 3,
                    Mutation = new Mutation()
                }
            };

            // Act
            var results = target.FilterMutants(mutants, new CsharpFileLeaf() { RelativePath = "src/1/SomeFile0.cs" }.ToReadOnly(), options);

            // Assert
            results.Count().ShouldBe(0);
            mutants.ShouldAllBe(m => m.ResultStatus == MutantStatus.Ignored);
            mutants.ShouldAllBe(m => m.ResultStatusReason == "Mutant not changed compared to target commit");
        }

        [Fact]
        public void FilterMutants_MergesResetMutants_WhenDashboardCompareOn()
        {
            // Arrange
            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var target = new DiffMutantFilter(options, new Mock<IDiffProvider>().Object, new Mock<IBaselineProvider>().Object, new Mock<IGitInfoProvider>().Object);

            var targetMutants = new List<Mutant>
            {
                new Mutant()
                {
                    Id = 1,
                    Mutation = new Mutation(),
                    ResultStatus = MutantStatus.Ignored,
                    ResultStatusReason = "A"
                },
                new Mutant()
                {
                    Id = 2,
                    Mutation = new Mutation(),
                    ResultStatus = MutantStatus.NotRun,
                    ResultStatusReason = "B"
                },
                new Mutant()
                {
                    Id = 3,
                    Mutation = new Mutation(),
                    ResultStatus = MutantStatus.NotRun,
                    ResultStatusReason = "B"
                }
            };

            var sourceMutants = new List<Mutant>
            {
                new Mutant()
                {
                    Id = 2,
                    Mutation = new Mutation(),
                    ResultStatus = MutantStatus.Ignored,
                    ResultStatusReason = "A"
                },
                new Mutant()
                {
                    Id = 3,
                    Mutation = new Mutation(),
                    ResultStatus = MutantStatus.Ignored,
                    ResultStatusReason = "A"
                },
                new Mutant()
                {
                    Id = 4,
                    Mutation = new Mutation(),
                    ResultStatus = MutantStatus.Ignored,
                    ResultStatusReason = "A"
                }
            };

            // Act
            var results = target.MergeMutantLists(targetMutants, sourceMutants);

            // Assert
            results.Count().ShouldBe(4);
            results.ShouldAllBe(m => m.ResultStatus == MutantStatus.Ignored);
            results.ShouldAllBe(m => m.ResultStatusReason == "A");
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
                    JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent())
                    ));

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var gitInfoProvider = new Mock<IGitInfoProvider>();

            var options = new StrykerOptions(compareToDashboard: false, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                ChangedSourceFiles = new List<string>(),
                ChangedTestFiles = new List<string> { "C:/testfile1.cs" }
            });

            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, gitInfoProvider.Object);

            var testFile1 = new TestListDescription(new[] { new TestDescription(Guid.NewGuid().ToString(), "name1", "C:/testfile1.cs") });
            var testFile2 = new TestListDescription(new[] { new TestDescription(Guid.NewGuid().ToString(), "name2", "C:/testfile2.cs") });

            var expectedToStay1 = new Mutant
            {
                CoveringTests = testFile1
            };
            var expectedToStay2 = new Mutant
            {
                CoveringTests = testFile1
            };
            var mutants = new List<Mutant>
            {
                expectedToStay1,
                expectedToStay2,
                new Mutant
                {
                    CoveringTests = testFile2
                }
            };

            // Act
            var results = target.FilterMutants(mutants, new CsharpFileLeaf().ToReadOnly(), options);

            // Assert
            results.ShouldBe(new []{expectedToStay1, expectedToStay2});
        }

        [Fact]
        public void Should_IgnoreMutants_WithoutCoveringTestsInfo_When_Tests_Have_Changed()
        {
            // Arrange
            var baselineProvider = new Mock<IBaselineProvider>();

            baselineProvider.Setup(x =>
                x.Load(It.IsAny<string>())
            ).Returns(
                Task.FromResult(
                    JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent())
                ));

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
            var gitInfoProvider = new Mock<IGitInfoProvider>();

            var options = new StrykerOptions(compareToDashboard: false, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                ChangedSourceFiles = new List<string>(),
                ChangedTestFiles = new List<string> { "C:/testfile.cs" }
            });

            var target = new DiffMutantFilter(options, diffProvider.Object, baselineProvider.Object, gitInfoProvider.Object);

            var mutants = new List<Mutant>
            {
                new Mutant()
            };

            // Act
            var results = target.FilterMutants(mutants, new CsharpFileLeaf().ToReadOnly(), options);

            // Assert
            results.ShouldBeEmpty();
        }

        [Fact]
        public void Should_ReturnAllMutants_When_NonSourceCodeFile_In_Tests_Has_Changed()
        {
            // Arrange
            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var diffProviderMock = new Mock<IDiffProvider>();
            var baselineProviderMock = new Mock<IBaselineProvider>();
            var gitInfoProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);

            var diffResult = new DiffResult() { ChangedTestFiles = new List<string> { "config.json" } };
            diffProviderMock.Setup(x => x.ScanDiff()).Returns(diffResult);

            baselineProviderMock.Setup(x =>
                x.Load(It.IsAny<string>())
                ).Returns(
                    Task.FromResult(
                         JsonReport.Build(new StrykerOptions(), JsonReportTestHelper.CreateProjectWith().ToReadOnlyInputComponent())
                         ));

            var target = new DiffMutantFilter(options, diffProviderMock.Object, baselineProviderMock.Object, gitInfoProviderMock.Object);

            var mutants = new List<Mutant> { new Mutant(), new Mutant(), new Mutant() };

            // Act
            var result = target.FilterMutants(mutants, new CsharpFileLeaf() { FullPath = "C:\\Foo\\Bar" }.ToReadOnly(), options);

            // Assert
            result.ShouldBe(mutants);
        }
    }
}

