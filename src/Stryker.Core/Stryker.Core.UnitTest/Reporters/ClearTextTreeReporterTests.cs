using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Shouldly;
using Spectre.Console.Testing;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ClearTextTreeReporterTests : TestBase
    {
        [Fact]
        public void ClearTextTreeReporter_ShouldPrintFullTree()
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

            var console = new TestConsole().EmitAnsiSequences();
            var target = new ClearTextTreeReporter(new StrykerOptions(), console);

            var folder = new CsharpFolderComposite()
            {
                FullPath = "C://ProjectFolder",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "ProjectFolder/Order.cs",
                FullPath = "C://ProjectFolder/Order.cs",
                Mutants = new Collection<Mutant>() {
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation }
                }
            });
            var folder2 = new CsharpFolderComposite()
            {
                RelativePath = "Subdir",
                FullPath = "C://ProjectFolder/SubDir",
            };
            folder.Add(folder2);
            folder2.Add(new CsharpFileLeaf()
            {
                RelativePath = "ProjectFolder/SubDir/OrderItem.cs",
                FullPath = "C://ProjectFolder/SubDir/OrderItem.cs",
                Mutants = new Collection<Mutant>()
            });
            folder2.Add(new CsharpFileLeaf()
            {
                RelativePath = "ProjectFolder/SubDir/CustomerOrdersWithItemsSpecification.cs",
                FullPath = "C://ProjectFolder/SubDir/CustomerOrdersWithItemsSpecification.cs",
                Mutants = new Collection<Mutant>() {
                    new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = mutation }
                }
            });
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "ProjectFolder/Zzz.cs",
                FullPath = "C://ProjectFolder/Zzz.cs",
                Mutants = new Collection<Mutant>() {
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation },
                    new Mutant() { ResultStatus = MutantStatus.Killed, Mutation = mutation }
                }
            });

            target.OnAllMutantsTested(folder, It.IsAny<TestProjectsInfo>());

            console.Output.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
All files [4/5 ({4.0 / 5.0:P2})]
├── Order.cs [2/2 ({1:P2})]
│   ├── [Killed] This name should display on line 1
│   │   ├── [-] 0 + 8
│   │   └── [+] 0 -8
│   └── [Killed] This name should display on line 1
│       ├── [-] 0 + 8
│       └── [+] 0 -8
├── Subdir [0/1 ({0:P2})]
│   ├── OrderItem.cs [0/0 (N/A)]
│   └── CustomerOrdersWithItemsSpecification.cs [0/1 ({0:P2})]
│       └── [Survived] This name should display on line 1
│           ├── [-] 0 + 8
│           └── [+] 0 -8
└── Zzz.cs [2/2 ({1:P2})]
    ├── [Killed] This name should display on line 1
    │   ├── [-] 0 + 8
    │   └── [+] 0 -8
    └── [Killed] This name should display on line 1
        ├── [-] 0 + 8
        └── [+] 0 -8
");
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintOnReportDone()
        {
            var console = new TestConsole().EmitAnsiSequences();
            var target = new ClearTextTreeReporter(new StrykerOptions(), console);

            var folder = new CsharpFolderComposite()
            {
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>() { }
            });

            target.OnAllMutantsTested(folder, It.IsAny<TestProjectsInfo>());

            console.Output.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
All files [0/0 (N/A)]
└── SomeFile.cs [0/0 (N/A)]
");
            console.Output.DarkGraySpanCount().ShouldBe(2);
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintKilledMutation()
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

            var console = new TestConsole().EmitAnsiSequences();
            var target = new ClearTextTreeReporter(new StrykerOptions(), console);

            var folder = new CsharpFolderComposite()
            {
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>() { new Mutant()
                {
                    ResultStatus = MutantStatus.Killed, Mutation = mutation }
                }
            });

            target.OnAllMutantsTested(folder, It.IsAny<TestProjectsInfo>());

            console.Output.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
All files [1/1 ({1:P2})]
└── SomeFile.cs [1/1 ({1:P2})]
    └── [Killed] This name should display on line 1
        ├── [-] 0 + 8
        └── [+] 0 -8
");
            console.Output.GreenSpanCount().ShouldBe(3);
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintSurvivedMutation()
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

            var console = new TestConsole().EmitAnsiSequences();
            var target = new ClearTextTreeReporter(new StrykerOptions(), console);

            var folder = new CsharpFolderComposite()
            {
                RelativePath = "RootFolder",
                FullPath = "C://RootFolder",
            };
            folder.Add(new CsharpFileLeaf()
            {
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>() { new Mutant() {
                ResultStatus = MutantStatus.Survived, Mutation = mutation } }
            });

            target.OnAllMutantsTested(folder, It.IsAny<TestProjectsInfo>());

            console.Output.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
All files [0/1 ({0:P2})]
└── SomeFile.cs [0/1 ({0:P2})]
    └── [Survived] This name should display on line 1
        ├── [-] 0 + 8
        └── [+] 0 -8
");

            // All percentages should be red and the [Survived] too
            console.Output.RedSpanCount().ShouldBe(3);
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintRedUnderThresholdBreak()
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

            var options = new StrykerOptions { Thresholds = new Thresholds { High = 80, Low = 70, Break = 0 } };
            var console = new TestConsole().EmitAnsiSequences();
            var target = new ClearTextTreeReporter(options, console);

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

            target.OnAllMutantsTested(folder, It.IsAny<TestProjectsInfo>());

            console.Output.RedSpanCount().ShouldBe(4);
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintYellowBetweenThresholdLowAndThresholdBreak()
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

            var options = new StrykerOptions { Thresholds = new Thresholds { High = 90, Low = 70, Break = 0 } };
            var console = new TestConsole().EmitAnsiSequences();
            var target = new ClearTextTreeReporter(options, console);

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

            target.OnAllMutantsTested(folder, It.IsAny<TestProjectsInfo>());

            console.Output.YellowSpanCount().ShouldBe(2);
        }

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintGreenAboveThresholdHigh()
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

            var console = new TestConsole().EmitAnsiSequences();
            var target = new ClearTextTreeReporter(new StrykerOptions(), console);

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

            target.OnAllMutantsTested(folder, It.IsAny<TestProjectsInfo>());

            console.Output.GreenSpanCount().ShouldBe(3);
        }
    }
}
