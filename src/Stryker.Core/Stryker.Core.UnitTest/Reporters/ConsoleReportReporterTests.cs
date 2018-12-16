﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using Stryker.Core.Options;
using Stryker.Core.Exceptions;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ConsoleReportReporterTests
    {
        
        [Fact]
        public void ConsoleReportReporter_ShouldPrintOnReportDone()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.DarkGray(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });

            var target = new ConsoleReportReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite() { Name = "RootFolder" };
            folder.Add(new FileLeaf() { Name = "SomeFile.cs", Mutants = new Collection<Mutant>() { } });

            target.OnAllMutantsTested(folder);

            output.ToString().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
- {Path.DirectorySeparatorChar}RootFolder [0/0 (- %)]
--- SomeFile.cs [0/0 (- %)]
");
            chalkMock.Verify(x => x.DarkGray(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void ConsoleReportReporter_ShouldPrintKilledMutation()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Green(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });

            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = MutatorType.Arithmetic
            };

            var target = new ConsoleReportReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite() { Name = "RootFolder" };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>() { new Mutant() {
                ResultStatus = MutantStatus.Killed, Mutation = mutation } }
            });

            target.OnAllMutantsTested(folder);

            output.ShouldBeWithNewlineReplace(
$@"

All mutants have been tested, and your mutation score has been calculated
- {Path.DirectorySeparatorChar}RootFolder [1/1 (100.00 %)]
--- SomeFile.cs [1/1 (100.00 %)]
[Killed] This name should display on line 1: '0 + 8' ==> '0 -8'
");
            chalkMock.Verify(x => x.Green(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public void ConsoleReportReporter_ShouldPrintSurvivedMutation()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Red(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });

            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = MutatorType.Arithmetic
            };
            
            var target = new ConsoleReportReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite() { Name = "RootFolder" };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>() { new Mutant() {
                ResultStatus = MutantStatus.Survived, Mutation = mutation } }
            });

            target.OnAllMutantsTested(folder);

            output.ShouldBeWithNewlineReplace(
$@"

All mutants have been tested, and your mutation score has been calculated
- {Path.DirectorySeparatorChar}RootFolder [0/1 (0.00 %)]
--- SomeFile.cs [0/1 (0.00 %)]
[Survived] This name should display on line 1: '0 + 8' ==> '0 -8'
");
            // All percentages should be red and the [Survived] too
            chalkMock.Verify(x => x.Red(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public void ConsoleReportReporter_ShouldPrintRedUnderThresholdBreak()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Red(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Green(It.IsAny<string>())).Callback((string text) => { output += text; });

            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = MutatorType.Arithmetic
            };

            var target = new ConsoleReportReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite() { Name = "RootFolder" };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation }
                }
            });

            target.OnAllMutantsTested(folder);
            
            chalkMock.Verify(x => x.Red(It.IsAny<string>()), Times.Exactly(4));
        }

        [Fact]
        public void ConsoleReportReporter_ShouldPrintYellowBetweenThresholdLowAndThresholdBreak()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Red(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Green(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Yellow(It.IsAny<string>())).Callback((string text) => { output += text; });

            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = MutatorType.Arithmetic
            };

            var target = new ConsoleReportReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite() { Name = "RootFolder" };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation }
                }
            });

            target.OnAllMutantsTested(folder);

            chalkMock.Verify(x => x.Yellow(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void ConsoleReportReporter_ShouldPrintGreenAboveThresholdHigh()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Green(It.IsAny<string>())).Callback((string text) => { output += text; });

            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = MutatorType.Arithmetic
            };

            var target = new ConsoleReportReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite() { Name = "RootFolder" };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                }
            });

            target.OnAllMutantsTested(folder);

            chalkMock.Verify(x => x.Green(It.IsAny<string>()), Times.Exactly(3));
        }
    }
}
