using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Shouldly;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class SinceMutantFilterTests : TestBase
    {
        [Fact]
        public static void ShouldHaveName()
        {
            // Act
            var target = new SinceMutantFilter(new DiffResult(), new TestSet()) as IMutantFilter;

            // Assert
            target.DisplayName.ShouldBe("since filter");
        }

        [Fact]
        public void ShouldNotMutateUnchangedFiles()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                Since = false
            };

            string myFile = Path.Combine("C:/test/", "myfile.cs");

            var diffResult = new DiffResult
            {
                ChangedSourceFiles = new Collection<string>(),
                ChangedTestFiles = new Collection<string>()
            };

            var target = new SinceMutantFilter(diffResult, new TestSet());
            var file = new CsharpFileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            // Act
            var filterResult = target.FilterMutants(new List<Mutant> { mutant }, file, options);

            // Assert
            filterResult.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldOnlyMutateChangedFiles()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                Since = false
            };

            string myFile = Path.Combine("C:/test/", "myfile.cs");

            var diffResult = new DiffResult
            {
                ChangedSourceFiles = new Collection<string>()
                {
                    myFile
                }
            };

            var target = new SinceMutantFilter(diffResult, new TestSet());
            var file = new CsharpFileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            // Act
            var filterResult = target.FilterMutants(new List<Mutant> { mutant }, file, options);

            // Assert
            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void ShouldNotFilterMutantsWhereCoveringTestsContainsChangedTestFile()
        {
            // Arrange
            var testProjectPath = "C:/MyTests";
            var options = new StrykerOptions();

            // If a file inside the test project is changed, a test has been changed
            var myTestPath = Path.Combine(testProjectPath, "myTest.cs");
            ;
            var tests = new TestSet();
            var test = new TestDescription(Guid.NewGuid(), "name", myTestPath);
            tests.RegisterTests(new[] { test });
            var diffResult = new DiffResult
            {
                ChangedSourceFiles = new Collection<string>
                {
                    myTestPath
                },
                ChangedTestFiles = new Collection<string>
                {
                    myTestPath
                }
            };
            var target = new SinceMutantFilter(diffResult, tests);

            // check the diff result for a file not inside the test project
            var file = new CsharpFileLeaf { FullPath = Path.Combine("C:/NotMyTests", "myfile.cs") };
            var mutant = new Mutant
            {
                CoveringTests = new TestGuidsList(new[] { test })
            };

            // Act
            var filterResult = target.FilterMutants(new List<Mutant> { mutant }, file, options);

            // Assert
            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void FilterMutantsWithNoChangedFilesReturnsEmptyList()
        {
            // Arrange
            var options = new StrykerOptions();

            var diffResult = new DiffResult
            {
                ChangedSourceFiles = new List<string>()
            };

            var tests = new TestSet();
            var target = new SinceMutantFilter(diffResult, tests);

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
            var results = target.FilterMutants(mutants, new CsharpFileLeaf() { RelativePath = "src/1/SomeFile0.cs" }, options);

            // Assert
            results.Count().ShouldBe(0);
            mutants.ShouldAllBe(m => m.ResultStatus == MutantStatus.Ignored);
            mutants.ShouldAllBe(m => m.ResultStatusReason == "Mutant not changed compared to target commit");
        }

        [Fact]
        public void FilterMutantsWithNoChangedFilesAndNoCoverage()
        {
            // Arrange
            var options = new StrykerOptions();

            var diffResult = new DiffResult
            {
                ChangedSourceFiles = new List<string>()
            };

            var target = new SinceMutantFilter(diffResult, new TestSet());

            var mutants = new List<Mutant>
            {
                new Mutant()
                {
                    Id = 1,
                    Mutation = new Mutation(),
                    ResultStatus = MutantStatus.NoCoverage
                },
                new Mutant()
                {
                    Id = 2,
                    Mutation = new Mutation(),
                    ResultStatus = MutantStatus.NoCoverage
                },
                new Mutant()
                {
                    Id = 3,
                    Mutation = new Mutation(),
                    ResultStatus = MutantStatus.NoCoverage
                }
            };

            // Act
            var results = target.FilterMutants(mutants, new CsharpFileLeaf() { RelativePath = "src/1/SomeFile0.cs" }, options);

            // Assert
            results.Count().ShouldBe(0);
            mutants.ShouldAllBe(m => m.ResultStatus == MutantStatus.Ignored);
            mutants.ShouldAllBe(m => m.ResultStatusReason == "Mutant not changed compared to target commit");
        }

        [Fact]
        public void FilterMutants_FiltersNoMutants_IfTestsChanged()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                WithBaseline = false,
                ProjectVersion = "version"
            };

            var diffResults = new DiffResult
            {
                ChangedSourceFiles = new List<string>(),
                ChangedTestFiles = new List<string> { "C:/testfile1.cs" }
            };

            var tests = new TestSet();
            var test1 = new TestDescription(Guid.NewGuid(), "name1", "C:/testfile1.cs");
            var test2 = new TestDescription(Guid.NewGuid(), "name2", "C:/testfile2.cs");
            tests.RegisterTests(new[] { test1, test2 });

            var target = new SinceMutantFilter(diffResults, tests);
            var testFile1 = new TestGuidsList(new[] { test1 });
            var testFile2 = new TestGuidsList(new[] { test2 });

            var expectedToStay1 = new Mutant { CoveringTests = testFile1 };
            var expectedToStay2 = new Mutant { CoveringTests = testFile1 };
            var newMutant = new Mutant { CoveringTests = testFile2 };
            var mutants = new List<Mutant>
            {
                expectedToStay1,
                expectedToStay2,
                newMutant
            };

            // Act
            var results = target.FilterMutants(mutants, new CsharpFileLeaf(), options);

            // Assert
            results.ShouldBe(new[] { expectedToStay1, expectedToStay2 });
        }

        [Fact]
        public void Should_IgnoreMutants_WithoutCoveringTestsInfo_When_Tests_Have_Changed()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                WithBaseline = false,
                ProjectVersion = "version"
            };

            var diffResults = new DiffResult
            {
                ChangedSourceFiles = new List<string>(),
                ChangedTestFiles = new List<string> { "C:/testfile.cs" }
            };

            var target = new SinceMutantFilter(diffResults, new TestSet());

            var mutants = new List<Mutant>
            {
                new Mutant{CoveringTests = TestGuidsList.NoTest()}
            };

            // Act
            var results = target.FilterMutants(mutants, new CsharpFileLeaf(), options);

            // Assert
            results.ShouldBeEmpty();
        }

        [Fact]
        public void Should_ReturnAllMutants_When_NonSourceCodeFile_In_Tests_Has_Changed()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                WithBaseline = true,
                ProjectVersion = "version"
            };

            var diffResult = new DiffResult { ChangedTestFiles = new List<string> { "config.json" } };

            var target = new SinceMutantFilter(diffResult, new TestSet());

            var mutants = new List<Mutant> { new Mutant(), new Mutant(), new Mutant() };

            // Act
            var result = target.FilterMutants(mutants, new CsharpFileLeaf() { FullPath = "C:\\Foo\\Bar" }, options);

            // Assert
            result.ShouldBe(mutants);
        }
    }
}

