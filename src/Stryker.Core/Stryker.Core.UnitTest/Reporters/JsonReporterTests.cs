using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using static Stryker.Core.Reporters.JsonReporter;

namespace Stryker.Core.UnitTest.Reporters
{
    public class JsonReporterTests
    {

        [Fact]
        public void JsonReportComponent_ShouldGenerateFileTreeForJsonSerialization()
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = MutatorType.Arithmetic
            };

            var folder = new FolderComposite() { Name = "RootFolder" };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant()
                    {
                        Id = 55,
                        ResultStatus = MutantStatus.Killed,
                        Mutation = mutation
                    }
                },
                SourceCode = "void M(){ int i = 0 + 8; }"
            });
            folder.Add(new FileLeaf()
            {
                Name = "SomeOtherFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant()
                    {
                        Id = 56,
                        ResultStatus = MutantStatus.Survived,
                        Mutation = mutation
                    }
                },
                SourceCode = "void M(){ int i = 0 + 8; }"
            });

            var result = JsonReportComponent.FromProjectComponent(folder, new ThresholdOptions(80, 60, 0));

            result.Name.ShouldBe("RootFolder");
            result.Health.ShouldBe("warning");
            result.MutationScore.ShouldBe(50);
            result.CompileErrors.ShouldBe(0);
            result.SurvivedMutants.ShouldBe(1);
            result.SkippedMutants.ShouldBe(0);
            result.TimeoutMutants.ShouldBe(0);
            result.KilledMutants.ShouldBe(1);
            result.TotalMutants.ShouldBe(2);
            result.ThresholdHigh.ShouldBe(80);
            result.ThresholdLow.ShouldBe(60);
            result.ThresholdBreak.ShouldBe(0);
            result.Source.ShouldBe(null);

            result.Mutants.ShouldBeEmpty();
            result.ChildResults.Count.ShouldBe(2);

            result.ChildResults[0].Name.ShouldBe("SomeFile.cs");
            result.ChildResults[0].Health.ShouldBe("ok");
            result.ChildResults[0].MutationScore.ShouldBe(100);
            result.ChildResults[0].CompileErrors.ShouldBe(0);
            result.ChildResults[0].SurvivedMutants.ShouldBe(0);
            result.ChildResults[0].SkippedMutants.ShouldBe(0);
            result.ChildResults[0].TimeoutMutants.ShouldBe(0);
            result.ChildResults[0].KilledMutants.ShouldBe(1);
            result.ChildResults[0].TotalMutants.ShouldBe(1);
            result.ChildResults[0].ThresholdHigh.ShouldBe(80);
            result.ChildResults[0].ThresholdLow.ShouldBe(60);
            result.ChildResults[0].ThresholdBreak.ShouldBe(0);
            result.ChildResults[0].Source.ShouldBe("void M(){ int i = 0 + 8; }");

            result.ChildResults[0].Mutants.ShouldHaveSingleItem();
            result.ChildResults[0].Mutants[0].Id.ShouldBe(55);
            result.ChildResults[0].Mutants[0].MutatorName.ShouldBe("This name should display");
            result.ChildResults[0].Mutants[0].Replacement.ShouldBe(mutation.ReplacementNode.ToString());
            result.ChildResults[0].Mutants[0].Span.ShouldBe(new[] { 18, 23 });
            result.ChildResults[0].Mutants[0].Status.ShouldBe("Killed");

            result.ChildResults[1].Name.ShouldBe("SomeOtherFile.cs");
            result.ChildResults[1].Health.ShouldBe("danger");
            result.ChildResults[1].MutationScore.ShouldBe(0);
            result.ChildResults[1].CompileErrors.ShouldBe(0);
            result.ChildResults[1].SurvivedMutants.ShouldBe(1);
            result.ChildResults[1].SkippedMutants.ShouldBe(0);
            result.ChildResults[1].TimeoutMutants.ShouldBe(0);
            result.ChildResults[1].KilledMutants.ShouldBe(0);
            result.ChildResults[1].TotalMutants.ShouldBe(1);
            result.ChildResults[1].ThresholdHigh.ShouldBe(80);
            result.ChildResults[1].ThresholdLow.ShouldBe(60);
            result.ChildResults[1].ThresholdBreak.ShouldBe(0);
            result.ChildResults[1].Source.ShouldBe("void M(){ int i = 0 + 8; }");

            result.ChildResults[1].Mutants.ShouldHaveSingleItem();
            result.ChildResults[1].Mutants[0].Id.ShouldBe(56);
            result.ChildResults[1].Mutants[0].MutatorName.ShouldBe("This name should display");
            result.ChildResults[1].Mutants[0].Replacement.ShouldBe(mutation.ReplacementNode.ToString());
            result.ChildResults[1].Mutants[0].Span.ShouldBe(new[] { 18, 23 });
            result.ChildResults[1].Mutants[0].Status.ShouldBe("Survived");
        }
    }
}
