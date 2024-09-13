using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.UnitTest.MutantFilters;

[TestClass]
public class SinceMutantFilterTests : TestBase
{
    [TestMethod]
    public void ShouldHaveName()
    {
        // Arrange
        var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);

        // Act
        var target = new SinceMutantFilter(diffProviderMock.Object) as IMutantFilter;

        // Assert
        target.DisplayName.ShouldBe("since filter");
    }

    [TestMethod]
    public void ShouldNotMutateUnchangedFiles()
    {
        // Arrange
        var options = new StrykerOptions()
        {
            Since = false
        };
        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

        var myFile = Path.Combine("C:/test/", "myfile.cs");
        ;

        _ = diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
        {
            ChangedSourceFiles = [],
            ChangedTestFiles = []
        });

        var target = new SinceMutantFilter(diffProvider.Object);
        var file = new CsharpFileLeaf { FullPath = myFile };

        var mutant = new Mutant();

        // Act
        var filterResult = target.FilterMutants([mutant], file, options);

        // Assert
        filterResult.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldOnlyMutateChangedFiles()
    {
        // Arrange
        var options = new StrykerOptions()
        {
            Since = false
        };
        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

        var myFile = Path.Combine("C:/test/", "myfile.cs");
        ;
        _ = diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
        {
            ChangedSourceFiles =
            [
                myFile
            ]
        });

        var target = new SinceMutantFilter(diffProvider.Object);
        var file = new CsharpFileLeaf { FullPath = myFile };

        var mutant = new Mutant();

        // Act
        var filterResult = target.FilterMutants([mutant], file, options);

        // Assert
        filterResult.ShouldContain(mutant);
    }

    [TestMethod]
    public void ShouldNotFilterMutantsWhereCoveringTestsContainsChangedTestFile()
    {
        // Arrange
        var testProjectPath = "C:/MyTests";
        var options = new StrykerOptions();

        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

        // If a file inside the test project is changed, a test has been changed
        var myTestPath = Path.Combine(testProjectPath, "myTest.cs");
        ;
        var tests = new TestSet();
        var test = new TestDescription(Guid.NewGuid(), "name", myTestPath);
        tests.RegisterTests(new[] { test });
        _ = diffProvider.SetupGet(x => x.Tests).Returns(tests);
        _ = diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles =
            [
                myTestPath
            ],
            ChangedTestFiles =
            [
                myTestPath
            ]
        });
        var target = new SinceMutantFilter(diffProvider.Object);

        // check the diff result for a file not inside the test project
        var file = new CsharpFileLeaf { FullPath = Path.Combine("C:/NotMyTests", "myfile.cs") };
        var mutant = new Mutant
        {
            CoveringTests = new TestGuidsList(new[] { test })
        };


        // Act
        var filterResult = target.FilterMutants([mutant], file, options);

        // Assert
        filterResult.ShouldContain(mutant);
    }

    [TestMethod]
    public void FilterMutantsWithNoChangedFilesReturnsEmptyList()
    {
        // Arrange
        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);

        var options = new StrykerOptions();

        _ = diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = []
        });

        _ = diffProvider.SetupGet(x => x.Tests).Returns(new TestSet());
        var target = new SinceMutantFilter(diffProvider.Object);

        var mutants = new List<Mutant>
        {
            new()
            {
                Id = 1,
                Mutation = new Mutation()
            },
            new()
            {
                Id = 2,
                Mutation = new Mutation()
            },
            new()
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

    [TestMethod]
    public void FilterMutantsWithNoChangedFilesAndNoCoverage()
    {
        // Arrange
        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);

        var options = new StrykerOptions();

        _ = diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = []
        });

        _ = diffProvider.SetupGet(x => x.Tests).Returns(new TestSet());

        var target = new SinceMutantFilter(diffProvider.Object);

        var mutants = new List<Mutant>
        {
            new()
            {
                Id = 1,
                Mutation = new Mutation(),
                ResultStatus = MutantStatus.NoCoverage
            },
            new()
            {
                Id = 2,
                Mutation = new Mutation(),
                ResultStatus = MutantStatus.NoCoverage
            },
            new()
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

    [TestMethod]
    public void FilterMutants_FiltersNoMutants_IfTestsChanged()
    {
        // Arrange
        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

        var options = new StrykerOptions()
        {
            WithBaseline = false,
            ProjectVersion = "version"
        };

        _ = diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = [],
            ChangedTestFiles = ["C:/testfile1.cs"]
        });

        var tests = new TestSet();
        var test1 = new TestDescription(Guid.NewGuid(), "name1", "C:/testfile1.cs");
        var test2 = new TestDescription(Guid.NewGuid(), "name2", "C:/testfile2.cs");
        tests.RegisterTests(new[] { test1, test2 });
        _ = diffProvider.SetupGet(x => x.Tests).Returns(tests);
        var target = new SinceMutantFilter(diffProvider.Object);
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

    [TestMethod]
    public void Should_IgnoreMutants_WithoutCoveringTestsInfo_When_Tests_Have_Changed()
    {
        // Arrange
        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

        var options = new StrykerOptions()
        {
            WithBaseline = false,
            ProjectVersion = "version"
        };

        _ = diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = [],
            ChangedTestFiles = ["C:/testfile.cs"]
        });

        _ = diffProvider.SetupGet(x => x.Tests).Returns(new TestSet());
        var target = new SinceMutantFilter(diffProvider.Object);

        var mutants = new List<Mutant>
        {
            new() {CoveringTests = TestGuidsList.NoTest()}
        };

        // Act
        var results = target.FilterMutants(mutants, new CsharpFileLeaf(), options);

        // Assert
        results.ShouldBeEmpty();
    }

    [TestMethod]
    public void Should_ReturnAllMutants_When_NonSourceCodeFile_In_Tests_Has_Changed()
    {
        // Arrange
        var options = new StrykerOptions()
        {
            WithBaseline = true,
            ProjectVersion = "version"
        };

        var diffProviderMock = new Mock<IDiffProvider>();

        var diffResult = new DiffResult() { ChangedTestFiles = ["config.json"] };
        _ = diffProviderMock.Setup(x => x.ScanDiff()).Returns(diffResult);

        var target = new SinceMutantFilter(diffProviderMock.Object);

        var mutants = new List<Mutant> { new(), new(), new() };

        // Act
        var result = target.FilterMutants(mutants, new CsharpFileLeaf() { FullPath = "C:\\Foo\\Bar" }, options);

        // Assert
        result.ShouldBe(mutants);
    }
}

