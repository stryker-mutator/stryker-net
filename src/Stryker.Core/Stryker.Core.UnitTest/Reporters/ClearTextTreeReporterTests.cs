using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ClearTextTreeReporterTests
    {

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintOnReportDone()
        {
            string output = "";
            var chalkMock = new Mock<IChalk>(MockBehavior.Strict);
            chalkMock.Setup(x => x.DarkGray(It.IsAny<string>())).Callback((string text) => { output += text; });
            chalkMock.Setup(x => x.Default(It.IsAny<string>())).Callback((string text) => { output += text; });

            var target = new ClearTextTreeReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite()
            {
                Name = "RootFolder",
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>() { }
            });

            target.OnAllMutantsTested(folder);

            output.ToString().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
RootFolder [0/0 (N/A)]
└── SomeFile.cs [0/0 (N/A)]
");
            chalkMock.Verify(x => x.DarkGray(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintKilledMutation()
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
                Type = Mutator.Arithmetic
            };

            var target = new ClearTextTreeReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite()
            {
                Name = "RootFolder",
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>() { new Mutant() {
                ResultStatus = MutantStatus.Killed, Mutation = mutation } }
            });

            target.OnAllMutantsTested(folder);

            output.ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
RootFolder [1/1 ({1:P2})]
└── SomeFile.cs [1/1 ({1:P2})]
    └── [Killed] This name should display on line 1
        ├── [-] 0 + 8
        └── [+] 0 -8
");
            chalkMock.Verify(x => x.Green(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintSurvivedMutation()
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
                Type = Mutator.Arithmetic
            };

            var target = new ClearTextTreeReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite()
            {
                Name = "RootFolder",
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>() { new Mutant() {
                ResultStatus = MutantStatus.Survived, Mutation = mutation } }
            });

            target.OnAllMutantsTested(folder);

            output.ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
RootFolder [0/1 ({0:P2})]
└── SomeFile.cs [0/1 ({0:P2})]
    └── [Survived] This name should display on line 1
        ├── [-] 0 + 8
        └── [+] 0 -8
");
            // All percentages should be red and the [Survived] too
            chalkMock.Verify(x => x.Red(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintRedUnderThresholdBreak()
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
                Type = Mutator.Arithmetic
            };

            var target = new ClearTextTreeReporter(new StrykerOptions(thresholdHigh: 80, thresholdLow: 70, thresholdBreak: 0), chalkMock.Object);

            var folder = new FolderComposite()
            {
                Name = "RootFolder",
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
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

            target.OnAllMutantsTested(folder);

            chalkMock.Verify(x => x.Red(It.IsAny<string>()), Times.Exactly(4));
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintYellowBetweenThresholdLowAndThresholdBreak()
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
                Type = Mutator.Arithmetic
            };

            var target = new ClearTextTreeReporter(new StrykerOptions(thresholdHigh: 90, thresholdLow: 70, thresholdBreak: 0), chalkMock.Object);

            var folder = new FolderComposite()
            {
                Name = "RootFolder",
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
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

            target.OnAllMutantsTested(folder);

            chalkMock.Verify(x => x.Yellow(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintGreenAboveThresholdHigh()
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
                Type = Mutator.Arithmetic
            };

            var target = new ClearTextTreeReporter(new StrykerOptions(), chalkMock.Object);

            var folder = new FolderComposite()
            {
                Name = "RootFolder",
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
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
