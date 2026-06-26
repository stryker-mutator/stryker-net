using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.Mutants;

namespace Stryker.Core.UnitTest.Mutants;

[TestClass]
public class DisableTimeoutsFeatureTests : TestBase
{
    [TestMethod]
    public void ShouldNotApplyWhenDisabled()
    {
        // Arrange
        var fileSystem = new Mock<IFileSystem>();
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(x => x.DisableTimeouts).Returns(false);

        var rootComponent = new TestProjectComponent();
        var target = new DisableTimeoutsFeature(fileSystem.Object);

        // Act
        target.Apply(rootComponent, options.Object);

        // Assert - file system should never be called
        fileSystem.VerifyNotCalled();
    }

    [TestMethod]
    public void ShouldNotApplyWhenNoTimeoutMutants()
    {
        // Arrange
        var fileSystem = new Mock<IFileSystem>();
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(x => x.DisableTimeouts).Returns(true);

        var rootComponent = new TestProjectComponent
        {
            Mutants = new List<IMutant>
            {
                new Mutant { Id = 1, ResultStatus = MutantStatus.Killed },
                new Mutant { Id = 2, ResultStatus = MutantStatus.Survived }
            }
        };

        // Act
        new DisableTimeoutsFeature(fileSystem.Object).Apply(rootComponent, options.Object);

        // Assert - file system should never be called
        fileSystem.VerifyNotCalled();
    }

    [TestMethod]
    public void ShouldApplyWhenTimeoutMutantsExist()
    {
        // Arrange
        var sourceCode = @"namespace Test
{
    public class Sample
    {
        public int Calculate(int a, int b)
        {
            return a + b;
        }
    }
}";
        var mockFileSystem = MockFileSystemHelper.CreateFileSystemWithFile("test.cs", sourceCode);
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(x => x.DisableTimeouts).Returns(true);

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, path: "test.cs");
        var root = syntaxTree.GetRoot();
        var returnStatement = root.DescendantNodes().OfType<ReturnStatementSyntax>().First();

        var mutant = new Mutant
        {
            Id = 1,
            ResultStatus = MutantStatus.Timeout,
            Mutation = new Mutation
            {
                Type = Mutator.Arithmetic,
                OriginalNode = returnStatement.Expression,
                DisplayName = "a + b to a - b"
            }
        };

        var rootComponent = new TestProjectComponent
        {
            Mutants = new List<IMutant> { mutant }
        };

        var target = new DisableTimeoutsFeature(mockFileSystem);

        // Act
        target.Apply(rootComponent, options.Object);

        // Assert
        var updatedContent = mockFileSystem.File.ReadAllText("test.cs");
        updatedContent.ShouldContain("// Stryker disable once Arithmetic: this mutation causes a timeout");
    }

    [TestMethod]
    public void ShouldMergeMultipleMutatorTypesOnSameLine()
    {
        // Arrange
        var sourceCode = @"namespace Test
{
    public class Sample
    {
        public bool Check(bool x)
        {
            while (1 < 0)
            {
                ;
            }
        }
    }
}";
        var mockFileSystem = MockFileSystemHelper.CreateFileSystemWithFile("test.cs", sourceCode);
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(x => x.DisableTimeouts).Returns(true);

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, path: "test.cs");
        var root = syntaxTree.GetRoot();
        var whileStatement = root.DescendantNodes().OfType<WhileStatementSyntax>().First();
        var condition = whileStatement.Condition;

        var mutants = new List<IMutant>
        {
            new Mutant
            {
                Id = 1,
                ResultStatus = MutantStatus.Timeout,
                Mutation = new Mutation
                {
                    Type = Mutator.Boolean,
                    OriginalNode = condition,
                    DisplayName = "1 < 0 to 1 <= 0"
                }
            },
            new Mutant
            {
                Id = 2,
                ResultStatus = MutantStatus.Timeout,
                Mutation = new Mutation
                {
                    Type = Mutator.Equality,
                    OriginalNode = condition,
                    DisplayName = "1 < 0 to 1 < 1"
                }
            },
            new Mutant
            {
                Id = 3,
                ResultStatus = MutantStatus.Timeout,
                Mutation = new Mutation
                {
                    Type = Mutator.Arithmetic,
                    OriginalNode = condition,
                    DisplayName = "1 < 0 to 0 < 0"
                }
            }
        };

        var rootComponent = new TestProjectComponent
        {
            Mutants = mutants
        };

        var target = new DisableTimeoutsFeature(mockFileSystem);

        // Act
        target.Apply(rootComponent, options.Object);

        // Assert
        var updatedContent = mockFileSystem.File.ReadAllText("test.cs");
        updatedContent.ShouldContain("// Stryker disable once Arithmetic,Boolean,Equality: this mutation causes a timeout");
        updatedContent.Split(new[] { '\n' }, StringSplitOptions.None).Count(l => l.Contains("Stryker disable once"))
            .ShouldBe(1);
    }

    [TestMethod]
    public void ShouldNotReinjectWhenCommentAlreadyExists()
    {
        // Arrange
        var sourceCode = @"namespace Test
{
    public class Sample
    {
        public int Calculate(int a, int b)
        {
            // Stryker disable once Arithmetic: this mutation causes a timeout
            return a + b;
        }
    }
}";
        var mockFileSystem = MockFileSystemHelper.CreateFileSystemWithFile("test.cs", sourceCode);
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(x => x.DisableTimeouts).Returns(true);

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, path: "test.cs");
        var root = syntaxTree.GetRoot();
        var returnStatement = root.DescendantNodes().OfType<ReturnStatementSyntax>().First();

        var mutant = new Mutant
        {
            Id = 1,
            ResultStatus = MutantStatus.Timeout,
            Mutation = new Mutation
            {
                Type = Mutator.Arithmetic,
                OriginalNode = returnStatement.Expression,
            }
        };

        var rootComponent = new TestProjectComponent
        {
            Mutants = new List<IMutant> { mutant }
        };

        var target = new DisableTimeoutsFeature(mockFileSystem);

        // Act
        target.Apply(rootComponent, options.Object);

        // Assert
        var updatedContent = mockFileSystem.File.ReadAllText("test.cs");
        updatedContent.ShouldBe(sourceCode);
    }

    [TestMethod]
    public void ShouldPreserveIndentation()
    {
        // Arrange
        var sourceCode = @"namespace Test
{
    public class Sample
    {
        public int Calculate(int a, int b)
        {
            return a + b;
        }
    }
}";
        var mockFileSystem = MockFileSystemHelper.CreateFileSystemWithFile("test.cs", sourceCode);
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(x => x.DisableTimeouts).Returns(true);

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, path: "test.cs");
        var root = syntaxTree.GetRoot();
        var returnStatement = root.DescendantNodes().OfType<ReturnStatementSyntax>().First();

        var mutant = new Mutant
        {
            Id = 1,
            ResultStatus = MutantStatus.Timeout,
            Mutation = new Mutation
            {
                Type = Mutator.Arithmetic,
                OriginalNode = returnStatement.Expression,
            }
        };

        var rootComponent = new TestProjectComponent
        {
            Mutants = new List<IMutant> { mutant }
        };

        var target = new DisableTimeoutsFeature(mockFileSystem);

        // Act
        target.Apply(rootComponent, options.Object);

        // Assert
        var updatedContent = mockFileSystem.File.ReadAllText("test.cs");
        var lines = updatedContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var commentLine = Array.FindIndex(lines, l => l.Contains("Stryker disable once"));
        commentLine.ShouldNotBe(-1);
        lines[commentLine].ShouldStartWith("            // Stryker disable once");
    }

    [TestMethod]
    public void ShouldHandleMultipleLinesWithSameMutator()
    {
        // Arrange
        var sourceCode = @"namespace Test
{
    public class Sample
    {
        public int Calculate(int a, int b)
        {
            return a + b;
        }

        public int Multiply(int x, int y)
        {
            return x * y;
        }
    }
}";
        var mockFileSystem = MockFileSystemHelper.CreateFileSystemWithFile("test.cs", sourceCode);
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(x => x.DisableTimeouts).Returns(true);

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, path: "test.cs");
        var root = syntaxTree.GetRoot();

        var addExpression = root.DescendantNodes().OfType<ReturnStatementSyntax>().First().Expression;
        var multiplyStatement = root.DescendantNodes().OfType<ReturnStatementSyntax>().Last();
        var multiplyExpression = multiplyStatement.Expression;

        var mutants = new List<IMutant>
        {
            new Mutant
            {
                Id = 1,
                ResultStatus = MutantStatus.Timeout,
                Mutation = new Mutation
                {
                    Type = Mutator.Arithmetic,
                    OriginalNode = addExpression,
                }
            },
            new Mutant
            {
                Id = 2,
                ResultStatus = MutantStatus.Timeout,
                Mutation = new Mutation
                {
                    Type = Mutator.Arithmetic,
                    OriginalNode = multiplyExpression,
                }
            }
        };

        var rootComponent = new TestProjectComponent
        {
            Mutants = mutants
        };

        var target = new DisableTimeoutsFeature(mockFileSystem);

        // Act
        target.Apply(rootComponent, options.Object);

        // Assert
        var updatedContent = mockFileSystem.File.ReadAllText("test.cs");
        updatedContent.ShouldContain("// Stryker disable once Arithmetic: this mutation causes a timeout");
        updatedContent.Split(new[] { '\n' }, StringSplitOptions.None).Count(l => l.Contains("Stryker disable once"))
            .ShouldBe(2);
    }
}

public class TestProjectComponent : IReadOnlyProjectComponent
{
    public string FullPath { get; set; }
    public string RelativePath { get; set; }
    public IFolderComposite Parent { get; set; }
    public Display DisplayFile { get; set; }
    public Display DisplayFolder { get; set; }

    private IEnumerable<IMutant> _mutants = Enumerable.Empty<IMutant>();
    public IEnumerable<IMutant> Mutants
    {
        get => _mutants;
        set => _mutants = value;
    }

    public IEnumerable<SyntaxTree> CompilationSyntaxTrees => Enumerable.Empty<SyntaxTree>();
    public IEnumerable<SyntaxTree> MutatedSyntaxTrees => Enumerable.Empty<SyntaxTree>();

    public IEnumerable<IReadOnlyMutant> TotalMutants() => Mutants;
    public IEnumerable<IReadOnlyMutant> ValidMutants() => Mutants;
    public IEnumerable<IReadOnlyMutant> InvalidMutants() => Enumerable.Empty<IReadOnlyMutant>();
    public IEnumerable<IReadOnlyMutant> UndetectedMutants() => Enumerable.Empty<IReadOnlyMutant>();
    public IEnumerable<IReadOnlyMutant> IgnoredMutants() => Enumerable.Empty<IReadOnlyMutant>();
    public IEnumerable<IReadOnlyMutant> NotRunMutants() => Enumerable.Empty<IReadOnlyMutant>();
    public IEnumerable<IReadOnlyMutant> DetectedMutants() => Enumerable.Empty<IReadOnlyMutant>();
    public double GetMutationScore() => 0;
    public Health CheckHealth(Stryker.Abstractions.Options.IThresholds threshold) => Health.None;

    public IEnumerable<IFileLeaf> GetAllFiles() => Enumerable.Empty<IFileLeaf>();
    public void Display() { }
}

public static class MockFileSystemHelper
{
    public static IFileSystem CreateFileSystemWithFile(string path, string content)
    {
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { path, new MockFileData(content.Replace("\r\n", "\n")) }
        });
        return mockFileSystem;
    }
}

public static class MockExtensions
{
    public static void VerifyNotCalled(this Mock<IFileSystem> mock)
    {
        mock.VerifyAll();
    }
}
