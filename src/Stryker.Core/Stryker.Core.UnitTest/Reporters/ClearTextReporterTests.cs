using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ClearTextReporterTests : TestBase
    {

        [Fact]
        public void ClearTextReporter_ShouldPrintOnReportDone()
        {
            var textWriter = new StringWriter();
            var target = new ClearTextReporter(new StrykerOptions(), textWriter);

            var rootFolder = new CsharpFolderComposite();

            var folder = new CsharpFolderComposite()
            {
                RelativePath = "FolderA",
                FullPath = "C://Project/FolderA",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "FolderA/SomeFile.cs",
                FullPath = "C://Project/FolderA/SomeFile.cs",
                Mutants = new Collection<Mutant>() { }
            });

            rootFolder.Add(folder);

            target.OnAllMutantsTested(rootFolder.ToReadOnly(), It.IsAny<TestProjectsInfo>());

            textWriter.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
┌─────────────────────┬──────────┬──────────┬───────────┬────────────┬──────────┬─────────┐
│ File                │  % score │ # killed │ # timeout │ # survived │ # no cov │ # error │
├─────────────────────┼──────────┼──────────┼───────────┼────────────┼──────────┼─────────┤
│ All files           │      N/A │        0 │         0 │          0 │        0 │       0 │
│ FolderA/SomeFile.cs │      N/A │        0 │         0 │          0 │        0 │       0 │
└─────────────────────┴──────────┴──────────┴───────────┴────────────┴──────────┴─────────┘
");
            textWriter.DarkGraySpanCount().ShouldBe(2);
        }

        [Fact]
        public void ClearTextReporter_ShouldPrintKilledMutation()
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = Mutator.Arithmetic
            };

            var textWriter = new StringWriter();
            var target = new ClearTextReporter(new StrykerOptions(), textWriter);

            var rootFolder = new CsharpFolderComposite();

            var folder = new CsharpFolderComposite()
            {
                RelativePath = "FolderA",
                FullPath = "C://Project/FolderA",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "FolderA/SomeFile.cs",
                FullPath = "C://Project/FolderA/SomeFile.cs",
                Mutants = new Collection<Mutant>() {
                    new Mutant() {
                        ResultStatus = MutantStatus.Killed,
                        Mutation = mutation
                    }
                }
            });

            rootFolder.Add(folder);

            target.OnAllMutantsTested(rootFolder.ToReadOnly(), It.IsAny<TestProjectsInfo>());

            textWriter.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
┌─────────────────────┬──────────┬──────────┬───────────┬────────────┬──────────┬─────────┐
│ File                │  % score │ # killed │ # timeout │ # survived │ # no cov │ # error │
├─────────────────────┼──────────┼──────────┼───────────┼────────────┼──────────┼─────────┤
│ All files           │   {100:N2} │        1 │         0 │          0 │        0 │       0 │
│ FolderA/SomeFile.cs │   {100:N2} │        1 │         0 │          0 │        0 │       0 │
└─────────────────────┴──────────┴──────────┴───────────┴────────────┴──────────┴─────────┘
");
            textWriter.GreenSpanCount().ShouldBe(2);
        }

        [Fact]
        public void ClearTextReporter_ShouldPrintSurvivedMutation()
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = Mutator.Arithmetic
            };

            var textWriter = new StringWriter();
            var target = new ClearTextReporter(new StrykerOptions(), textWriter);

            var rootFolder = new CsharpFolderComposite();

            var folder = new CsharpFolderComposite()
            {
                RelativePath = "FolderA",
                FullPath = "C://Project/FolderA",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "FolderA/SomeFile.cs",
                FullPath = "C://Project/FolderA/SomeFile.cs",
                Mutants = new Collection<Mutant>() {
                    new Mutant() {
                        ResultStatus = MutantStatus.Survived,
                        Mutation = mutation
                    }
                }
            });
            rootFolder.Add(folder);

            target.OnAllMutantsTested(rootFolder.ToReadOnly(), It.IsAny<TestProjectsInfo>());

            textWriter.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
┌─────────────────────┬──────────┬──────────┬───────────┬────────────┬──────────┬─────────┐
│ File                │  % score │ # killed │ # timeout │ # survived │ # no cov │ # error │
├─────────────────────┼──────────┼──────────┼───────────┼────────────┼──────────┼─────────┤
│ All files           │     {0:N2} │        0 │         0 │          1 │        0 │       0 │
│ FolderA/SomeFile.cs │     {0:N2} │        0 │         0 │          1 │        0 │       0 │
└─────────────────────┴──────────┴──────────┴───────────┴────────────┴──────────┴─────────┘
");
            // All percentages should be red and the [Survived] too
            textWriter.RedSpanCount().ShouldBe(2);
        }

        [Fact]
        public void ClearTextReporter_ShouldPrintRedUnderThresholdBreak()
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = Mutator.Arithmetic
            };

            var textWriter = new StringWriter();
            var options = new StrykerOptions { Thresholds = new Thresholds { High = 80, Low = 70, Break = 0 } };
            var target = new ClearTextReporter(options, textWriter);

            var folder = new CsharpFolderComposite()
            {
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation }
                }
            });

            target.OnAllMutantsTested(folder.ToReadOnly(), It.IsAny<TestProjectsInfo>());

            textWriter.RedSpanCount().ShouldBe(2);
        }

        [Fact]
        public void ClearTextReporter_ShouldPrintYellowBetweenThresholdLowAndThresholdBreak()
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = Mutator.Arithmetic
            };

            var textWriter = new StringWriter();
            var options = new StrykerOptions { Thresholds = new Thresholds { High = 90, Low = 70, Break = 0 } };
            var target = new ClearTextReporter(options, textWriter);

            var folder = new CsharpFolderComposite()
            {
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation }
                }
            });

            target.OnAllMutantsTested(folder.ToReadOnly(), It.IsAny<TestProjectsInfo>());

            textWriter.YellowSpanCount().ShouldBe(2);
        }

        [Fact]
        public void ClearTextReporter_ShouldPrintGreenAboveThresholdHigh()
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = Mutator.Arithmetic
            };

            var textWriter = new StringWriter();
            var target = new ClearTextReporter(new StrykerOptions(), textWriter);

            var folder = new CsharpFolderComposite()
            {
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                }
            });

            target.OnAllMutantsTested(folder.ToReadOnly(), It.IsAny<TestProjectsInfo>());

            textWriter.GreenSpanCount().ShouldBe(2);
        }
    }
}
