using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.Configuration.Options;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.VsTest;
using Stryker.Utilities.Logging;

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

        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
        {
            ChangedSourceFiles = new Collection<string>(),
            ChangedTestFiles = new Collection<string>()
        });

        var target = new SinceMutantFilter(diffProvider.Object);
        var file = new CsharpFileLeaf { FullPath = myFile };

        var mutant = new Mutant();

        // Act
        var filterResult = target.FilterMutants(new List<Mutant> { mutant }, file, options);

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
        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
        {
            ChangedSourceFiles = new Collection<string>()
            {
                myFile
            }
        });

        var target = new SinceMutantFilter(diffProvider.Object);
        var file = new CsharpFileLeaf { FullPath = myFile };

        var mutant = new Mutant();

        // Act
        var filterResult = target.FilterMutants(new List<Mutant> { mutant }, file, options);

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
        var test = new TestDescription("id", "name", myTestPath);
        tests.RegisterTests(new[] { test });
        diffProvider.SetupGet(x => x.Tests).Returns(tests);
        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = new Collection<string>
            {
                myTestPath
            },
            ChangedTestFiles = new Collection<string>
            {
                myTestPath
            }
        });
        var target = new SinceMutantFilter(diffProvider.Object);

        // check the diff result for a file not inside the test project
        var file = new CsharpFileLeaf { FullPath = Path.Combine("C:/NotMyTests", "myfile.cs") };
        var mutant = new Mutant
        {
            CoveringTests = new TestIdentifierList(new[] { test })
        };


        // Act
        var filterResult = target.FilterMutants(new List<Mutant> { mutant }, file, options);

        // Assert
        filterResult.ShouldContain(mutant);
    }

    [TestMethod]
    public void FilterMutantsWithNoChangedFilesReturnsEmptyList()
    {
        // Arrange
        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);

        var options = new StrykerOptions();

        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = new List<string>()
        });

        diffProvider.SetupGet(x => x.Tests).Returns(new TestSet());
        var target = new SinceMutantFilter(diffProvider.Object);

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

    [TestMethod]
    public void FilterMutantsWithNoChangedFilesAndNoCoverage()
    {
        // Arrange
        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);

        var options = new StrykerOptions();

        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = new List<string>()
        });

        diffProvider.SetupGet(x => x.Tests).Returns(new TestSet());

        var target = new SinceMutantFilter(diffProvider.Object);

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

        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = new List<string>(),
            ChangedTestFiles = new List<string> { "C:/testfile1.cs" }
        });

        var tests = new TestSet();
        var test1 = new TestDescription("id1", "name1", "C:/testfile1.cs");
        var test2 = new TestDescription("id2", "name2", "C:/testfile2.cs");
        tests.RegisterTests(new[] { test1, test2 });
        diffProvider.SetupGet(x => x.Tests).Returns(tests);
        var target = new SinceMutantFilter(diffProvider.Object);
        var testFile1 = new TestIdentifierList(new[] { test1 });
        var testFile2 = new TestIdentifierList(new[] { test2 });

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

        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = new List<string>(),
            ChangedTestFiles = new List<string> { "C:/testfile.cs" }
        });

        diffProvider.SetupGet(x => x.Tests).Returns(new TestSet());
        var target = new SinceMutantFilter(diffProvider.Object);

        var mutants = new List<Mutant>
        {
            new Mutant{CoveringTests = TestIdentifierList.NoTest()}
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

        var diffResult = new DiffResult() { ChangedTestFiles = new List<string> { "config.json" } };
        diffProviderMock.Setup(x => x.ScanDiff()).Returns(diffResult);

        var target = new SinceMutantFilter(diffProviderMock.Object);

        var mutants = new List<Mutant> { new Mutant(), new Mutant(), new Mutant() };

        // Act
        var result = target.FilterMutants(mutants, new CsharpFileLeaf() { FullPath = "C:\\Foo\\Bar" }, options);

        // Assert
        result.ShouldBe(mutants);
    }

    [TestMethod]
    public void Should_LogCsharpChangedFilesAtDebug_AndNonCsharpChangedFilesAtInformation()
    {
        // Arrange
        var captureProvider = new CapturingLoggerProvider();
        ApplicationLogging.LoggerFactory = new LoggerFactory(new[] { captureProvider });

        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);
        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = new Collection<string> { "C:/src/myfile.cs", "C:/docs/readme.md" },
            ChangedTestFiles = new Collection<string> { "C:/tests/mytest.cs" }
        });

        // Act
        _ = new SinceMutantFilter(diffProvider.Object);

        // Assert
        captureProvider.Entries.ShouldContain(e => e.Level == LogLevel.Information && e.Message.Contains("readme.md"));
        captureProvider.Entries.ShouldNotContain(e => e.Level == LogLevel.Information && e.Message.Contains("myfile.cs"));
        captureProvider.Entries.ShouldNotContain(e => e.Level == LogLevel.Information && e.Message.Contains("mytest.cs"));
        captureProvider.Entries.ShouldContain(e => e.Level == LogLevel.Debug && e.Message.Contains("myfile.cs"));
        captureProvider.Entries.ShouldContain(e => e.Level == LogLevel.Debug && e.Message.Contains("mytest.cs"));
        captureProvider.Entries.ShouldNotContain(e => e.Level == LogLevel.Debug && e.Message.Contains("readme.md"));
    }

    [TestMethod]
    public void Should_LogRelevantChangedSourceFile_WhenFilteringMutants()
    {
        // Arrange
        var captureProvider = new CapturingLoggerProvider();
        ApplicationLogging.LoggerFactory = new LoggerFactory(new[] { captureProvider });

        var options = new StrykerOptions() { Since = false };
        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

        var myFile = Path.Combine("C:/test/", "myfile.cs");

        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
        {
            ChangedSourceFiles = new Collection<string>()
            {
                myFile
            },
            ChangedTestFiles = new Collection<string>()
        });

        var target = new SinceMutantFilter(diffProvider.Object);
        var file = new CsharpFileLeaf { FullPath = myFile };

        // Act
        target.FilterMutants(new List<Mutant> { new Mutant() }, file, options);

        // Assert
        captureProvider.Entries.ShouldContain(e => e.Level == LogLevel.Information && e.Message.Contains(myFile));
    }

    [TestMethod]
    public void Should_LogRelevantChangedTestFile_WhenCoveringTestsChanged()
    {
        // Arrange
        var captureProvider = new CapturingLoggerProvider();
        ApplicationLogging.LoggerFactory = new LoggerFactory(new[] { captureProvider });

        var options = new StrykerOptions()
        {
            WithBaseline = false,
            ProjectVersion = "version"
        };

        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

        var tests = new TestSet();
        var test1 = new TestDescription("id1", "name1", "C:/testfile1.cs");
        var test2 = new TestDescription("id2", "name2", "C:/testfile2.cs");
        tests.RegisterTests(new[] { test1, test2 });
        diffProvider.SetupGet(x => x.Tests).Returns(tests);
        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = new List<string>(),
            ChangedTestFiles = new List<string> { "C:/testfile1.cs" }
        });

        var target = new SinceMutantFilter(diffProvider.Object);
        var mutant = new Mutant { CoveringTests = new TestIdentifierList(new[] { test1 }) };

        // Act
        target.FilterMutants(new List<Mutant> { mutant }, new CsharpFileLeaf(), options);

        // Assert
        captureProvider.Entries.ShouldContain(e => e.Level == LogLevel.Information && e.Message.Contains("C:/testfile1.cs"));
        captureProvider.Entries.ShouldNotContain(e => e.Message.Contains("C:/testfile2.cs"));
    }

    [TestMethod]
    public void Should_LogRelevantChangedTestFile_OnlyOnce()
    {
        // Arrange
        var captureProvider = new CapturingLoggerProvider();
        ApplicationLogging.LoggerFactory = new LoggerFactory(new[] { captureProvider });

        var options = new StrykerOptions()
        {
            WithBaseline = false,
            ProjectVersion = "version"
        };

        var diffProvider = new Mock<IDiffProvider>(MockBehavior.Loose);

        var tests = new TestSet();
        var test1 = new TestDescription("id1", "name1", "C:/testfile1.cs");
        tests.RegisterTests(new[] { test1 });
        diffProvider.SetupGet(x => x.Tests).Returns(tests);
        diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult
        {
            ChangedSourceFiles = new List<string>(),
            ChangedTestFiles = new List<string> { "C:/testfile1.cs" }
        });

        var target = new SinceMutantFilter(diffProvider.Object);
        var mutants = new List<Mutant> { new Mutant { CoveringTests = new TestIdentifierList(new[] { test1 }) } };

        // Act
        target.FilterMutants(mutants, new CsharpFileLeaf() { FullPath = "C:/src/file1.cs" }, options);
        target.FilterMutants(mutants, new CsharpFileLeaf() { FullPath = "C:/src/file2.cs" }, options);

        // Assert
        captureProvider.Entries.Count(e => e.Level == LogLevel.Information && e.Message.Contains("C:/testfile1.cs")).ShouldBe(1);
    }

    private sealed class CapturingLoggerProvider : ILoggerProvider
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = new List<(LogLevel Level, string Message)>();

        public ILogger CreateLogger(string categoryName) => new CapturingLogger(Entries);

        public void Dispose() { }

        private sealed class CapturingLogger : ILogger
        {
            private readonly List<(LogLevel Level, string Message)> _entries;

            public CapturingLogger(List<(LogLevel Level, string Message)> entries) => _entries = entries;

            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
                _entries.Add((logLevel, formatter(state, exception)));
        }
    }
}

