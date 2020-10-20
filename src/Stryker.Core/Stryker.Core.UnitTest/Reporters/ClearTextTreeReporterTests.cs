﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ClearTextTreeReporterTests
    {

        [Fact]
        public void ClearTextTreeReporter_ShouldPrintOnReportDone()
        {
            var textWriter = new StringWriter();
            var target = new ClearTextTreeReporter(new StrykerOptions(), textWriter);

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

            textWriter.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
RootFolder [0/0 (N/A)]
└── SomeFile.cs [0/0 (N/A)]
");
            textWriter.DarkGraySpanCount().ShouldBe(2);
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

            var textWriter = new StringWriter();
            var target = new ClearTextTreeReporter(new StrykerOptions(), textWriter);

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

            textWriter.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
RootFolder [1/1 ({1:P2})]
└── SomeFile.cs [1/1 ({1:P2})]
    └── [Killed] This name should display on line 1
        ├── [-] 0 + 8
        └── [+] 0 -8
");
            textWriter.GreenSpanCount().ShouldBe(3);
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

            var textWriter = new StringWriter();
            var target = new ClearTextTreeReporter(new StrykerOptions(), textWriter);

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

            textWriter.RemoveAnsi().ShouldBeWithNewlineReplace($@"

All mutants have been tested, and your mutation score has been calculated
RootFolder [0/1 ({0:P2})]
└── SomeFile.cs [0/1 ({0:P2})]
    └── [Survived] This name should display on line 1
        ├── [-] 0 + 8
        └── [+] 0 -8
");

            // All percentages should be red and the [Survived] too
            textWriter.RedSpanCount().ShouldBe(3);
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

            var textWriter = new StringWriter();
            var target = new ClearTextTreeReporter(new StrykerOptions(thresholdHigh: 80, thresholdLow: 70, thresholdBreak: 0), textWriter);

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

            textWriter.RedSpanCount().ShouldBe(4);
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

            var textWriter = new StringWriter();
            var target = new ClearTextTreeReporter(new StrykerOptions(thresholdHigh: 90, thresholdLow: 70, thresholdBreak: 0), textWriter);

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

            textWriter.YellowSpanCount().ShouldBe(2);
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

            var textWriter = new StringWriter();
            var target = new ClearTextTreeReporter(new StrykerOptions(), textWriter);

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

            textWriter.GreenSpanCount().ShouldBe(3);
        }
    }
}
