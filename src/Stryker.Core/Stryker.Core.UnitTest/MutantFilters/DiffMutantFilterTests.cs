using Moq;
using Shouldly;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class DiffMutantFilterTests
    {
        [Fact]
        public static void ShouldHaveName()
        {
            // Arrange
            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);

            // Act
            var target = new DiffMutantFilter(diffProviderMock.Object) as IMutantFilter;

            // Assert
            target.DisplayName.ShouldBe("git diff file filter");
        }

        [Fact]
        public void ShouldNotMutateUnchangedFiles()
        {
            // Arrange
            var options = new StrykerOptions(diff: true);
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedSourceFiles = new Collection<string>(),
                ChangedTestFiles = new Collection<string>()
            });

            var target = new DiffMutantFilter(diffProvider.Object);
            var file = new CsharpFileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            // Act
            var filterResult = target.FilterMutants(new List<Mutant> { mutant }, file.ToReadOnly(), options);

            // Assert
            filterResult.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldOnlyMutateChangedFiles()
        {
            // Arrange
            var options = new StrykerOptions(diff: true);

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedSourceFiles = new Collection<string>()
                {
                    myFile
                }
            });

            var target = new DiffMutantFilter(diffProvider.Object);
            var file = new CsharpFileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            // Act
            var filterResult = target.FilterMutants(new List<Mutant> { mutant }, file.ToReadOnly(), options);

            // Assert
            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void ShouldNotFilterMutantsWhereCoveringTestsContainsChangedTestFile()
        {
            // Arrange
            var testProjectPath = "C:/MyTests";
            var options = new StrykerOptions();

            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

            // If a file inside the test project is changed, a test has been changed
            var myTest = Path.Combine(testProjectPath, "myTest.cs"); ;
            var tests = new TestSet();
            var test = new TestDescription(Guid.NewGuid(), "name", myTest);
            tests.RegisterTests(new[] {test});
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                ChangedSourceFiles = new Collection<string>
                {
                    myTest
                },
                ChangedTestFiles = new Collection<string>
                {
                    myTest
                }
            });
            var target = new DiffMutantFilter(diffProvider.Object);

            // check the diff result for a file not inside the test project
            var file = new CsharpFileLeaf { FullPath = Path.Combine("C:/NotMyTests", "myfile.cs") };
            var mutant = new Mutant
            {
                CoveringTests = new TestsGuidList(tests, new[] {test})
            };


            // Act
            var filterResult = target.FilterMutants(new List<Mutant> { mutant }, file.ToReadOnly(), options);

            // Assert
            filterResult.ShouldContain(mutant);
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

            var target = new DiffMutantFilter(diffProvider.Object);

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
        public void FilterMutants_FiltersNoMutants_IfTestsChanged()
        {
            // Arrange
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

            var options = new StrykerOptions(compareToDashboard: false, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                ChangedSourceFiles = new List<string>(),
                ChangedTestFiles = new List<string> { "C:/testfile1.cs" }
            });

            var target = new DiffMutantFilter(diffProvider.Object);
            var tests = new TestSet();
            var test1 = new TestDescription(Guid.NewGuid(), "name1", "C:/testfile1.cs");
            var test2 = new TestDescription(Guid.NewGuid(), "name2", "C:/testfile2.cs");
            tests.RegisterTests(new[] {test1, test2});

            var testFile1 = new TestsGuidList(tests, new [] {test1});
            var testFile2 = new TestsGuidList(tests, new [] {test2});

            var expectedToStay1 = new Mutant {CoveringTests = testFile1};
            var expectedToStay2 = new Mutant {CoveringTests = testFile1};
            var newMutant = new Mutant {CoveringTests = testFile2};
            var mutants = new List<Mutant>
            {
                expectedToStay1,
                expectedToStay2,
                newMutant
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
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

            var options = new StrykerOptions(compareToDashboard: false, projectVersion: "version");

            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
            {
                ChangedSourceFiles = new List<string>(),
                ChangedTestFiles = new List<string> { "C:/testfile.cs" }
            });

            var target = new DiffMutantFilter(diffProvider.Object);

            var mutants = new List<Mutant>
            {
                new Mutant{CoveringTests = TestsGuidList.NoTest()}
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

            var diffResult = new DiffResult() { ChangedTestFiles = new List<string> { "config.json" } };
            diffProviderMock.Setup(x => x.ScanDiff()).Returns(diffResult);

            var target = new DiffMutantFilter(diffProviderMock.Object);

            var mutants = new List<Mutant> { new Mutant(), new Mutant(), new Mutant() };

            // Act
            var result = target.FilterMutants(mutants, new CsharpFileLeaf() { FullPath = "C:\\Foo\\Bar" }.ToReadOnly(), options);

            // Assert
            result.ShouldBe(mutants);
        }
    }
}

