using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.CLI.MutationServer;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents.Csharp;

namespace Stryker.CLI.UnitTest.MutationServer;

[TestClass]
public class MutationServerScopeTests
{
    private readonly string _basePath = Path.GetFullPath("project");

    [TestMethod]
    public void DiscoveryScopeShouldSelectAnExactFile()
    {
        var (file, mutant) = CreateMutant("src/Calculator.cs", "true");
        var scope = MutationServerScope.ForDiscovery(_basePath, new DiscoverParams
        {
            Files = [new FileRange { Path = "src/Calculator.cs" }]
        });

        scope.Includes(file, mutant).ShouldBeTrue();
        scope.IncludesFile(file).ShouldBeTrue();

        var (otherFile, otherMutant) = CreateMutant("src/Other.cs", "true");
        scope.Includes(otherFile, otherMutant).ShouldBeFalse();
        scope.IncludesFile(otherFile).ShouldBeFalse();
    }

    [TestMethod]
    public void DiscoveryScopeShouldSelectDirectoriesAndExactRanges()
    {
        var (file, mutant) = CreateMutant("src/Calculator.cs", "true");
        var location = mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan();
        var scope = MutationServerScope.ForDiscovery(_basePath, new DiscoverParams
        {
            Files =
            [
                new FileRange
                {
                    Path = "src/",
                    Range = new MutationServerLocation
                    {
                        Start = new MutationServerPosition
                        {
                            Line = location.StartLinePosition.Line + 1,
                            Column = location.StartLinePosition.Character + 1
                        },
                        End = new MutationServerPosition
                        {
                            Line = location.EndLinePosition.Line + 1,
                            Column = location.EndLinePosition.Character + 1
                        }
                    }
                }
            ]
        });

        scope.Includes(file, mutant).ShouldBeTrue();
        scope.IncludesFile(file).ShouldBeTrue();

        var outsideScope = MutationServerScope.ForDiscovery(_basePath, new DiscoverParams
        {
            Files =
            [
                new FileRange
                {
                    Path = "src/",
                    Range = new MutationServerLocation
                    {
                        Start = new MutationServerPosition { Line = 1, Column = 1 },
                        End = new MutationServerPosition { Line = 1, Column = 2 }
                    }
                }
            ]
        });
        outsideScope.Includes(file, mutant).ShouldBeFalse();
        outsideScope.IncludesFile(file).ShouldBeTrue();
    }

    [TestMethod]
    public void MutationScopeShouldPreferDiscoveredMutantsOverFiles()
    {
        var (file, mutant) = CreateMutant("src/Calculator.cs", "true");
        var protocolId = MutationServerMutantIdentity.GetId(_basePath, file, mutant);
        var scope = MutationServerScope.ForMutationTest(_basePath, new MutationTestParams
        {
            Files = [new FileRange { Path = "src/Other.cs" }],
            Mutants = new Dictionary<string, DiscoveredFile>
            {
                ["src/Calculator.cs"] = new()
                {
                    Mutants =
                    [
                        new DiscoveredMutant
                        {
                            Id = protocolId,
                            Location = new MutationServerLocation(),
                            MutatorName = "boolean"
                        }
                    ]
                }
            }
        });

        scope.Includes(file, mutant).ShouldBeTrue();
    }

    [TestMethod]
    public void ExplicitEmptyFilesShouldSelectNoMutants()
    {
        var (file, mutant) = CreateMutant("src/Calculator.cs", "true");

        MutationServerScope.ForDiscovery(
                _basePath,
                new DiscoverParams { Files = [] })
            .Includes(file, mutant)
            .ShouldBeFalse();
        MutationServerScope.ForDiscovery(
                _basePath,
                new DiscoverParams { Files = [] })
            .IncludesFile(file)
            .ShouldBeFalse();
        MutationServerScope.ForMutationTest(
                _basePath,
                new MutationTestParams { Files = [] })
            .Includes(file, mutant)
            .ShouldBeFalse();
        MutationServerScope.ForMutationTest(
                _basePath,
                new MutationTestParams { Files = [] })
            .IncludesFile(file)
            .ShouldBeFalse();
    }

    [TestMethod]
    public void OmittedFilesShouldSelectAllMutants()
    {
        var (file, mutant) = CreateMutant("src/Calculator.cs", "true");

        MutationServerScope.ForDiscovery(_basePath, new DiscoverParams())
            .Includes(file, mutant)
            .ShouldBeTrue();
        MutationServerScope.ForDiscovery(_basePath, new DiscoverParams())
            .IncludesFile(file)
            .ShouldBeTrue();
        MutationServerScope.ForMutationTest(_basePath, new MutationTestParams())
            .Includes(file, mutant)
            .ShouldBeTrue();
        MutationServerScope.ForMutationTest(_basePath, new MutationTestParams())
            .IncludesFile(file)
            .ShouldBeTrue();
    }

    [TestMethod]
    public void InvalidMutantTargetsShouldBeIgnored()
    {
        var (file, mutant) = CreateMutant("src/Calculator.cs", "true");
        var scope = MutationServerScope.ForMutationTest(_basePath, new MutationTestParams
        {
            Mutants = new Dictionary<string, DiscoveredFile>
            {
                [""] = null,
                ["src/Calculator.cs"] = new DiscoveredFile { Mutants = null }
            }
        });

        scope.Includes(file, mutant).ShouldBeFalse();
        scope.IncludesFile(file).ShouldBeFalse();
    }

    [TestMethod]
    public void InvalidMutantsShouldNotRemoveValidTargets()
    {
        var (file, mutant) = CreateMutant("src/Calculator.cs", "true");
        var protocolId = MutationServerMutantIdentity.GetId(_basePath, file, mutant);
        var scope = MutationServerScope.ForMutationTest(_basePath, new MutationTestParams
        {
            Mutants = new Dictionary<string, DiscoveredFile>
            {
                ["src/Calculator.cs"] = new DiscoveredFile
                {
                    Mutants =
                    [
                        null,
                        new DiscoveredMutant { Id = "" },
                        new DiscoveredMutant { Id = protocolId }
                    ]
                }
            }
        });

        scope.Includes(file, mutant).ShouldBeTrue();
        scope.IncludesFile(file).ShouldBeTrue();
    }

    private (CsharpFileLeaf File, Mutant Mutant) CreateMutant(string relativePath, string expression)
    {
        var fullPath = Path.GetFullPath(relativePath, _basePath);
        var syntaxTree = CSharpSyntaxTree.ParseText(
            $"class Calculator {{ bool Calculate() => {expression}; }}",
            path: fullPath);
        var originalNode = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().Single();
        var mutant = new Mutant
        {
            Id = 42,
            ResultStatus = MutantStatus.Pending,
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                DisplayName = "boolean",
                Description = "Boolean literal"
            }
        };
        var file = new CsharpFileLeaf
        {
            FullPath = fullPath,
            RelativePath = relativePath,
            SourceCode = syntaxTree.ToString(),
            SyntaxTree = syntaxTree,
            Mutants = [mutant]
        };

        return (file, mutant);
    }
}
