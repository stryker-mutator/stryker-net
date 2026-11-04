using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;

namespace Stryker.Core.UnitTest.Mutants;

[TestClass]
public class MutationNeutralFieldsTests : MutantOrchestratorTestsBase
{
    [TestMethod]
    public void MutationLocation_FromRoslynSpan_Is1Based()
    {
        var span = new FileLinePositionSpan("file.cs", new LinePositionSpan(new LinePosition(0, 0), new LinePosition(2, 4)));

        var location = MutationLocation.FromRoslyn(span, 0, 10);

        location.FilePath.ShouldBe("file.cs");
        location.StartLine.ShouldBe(1);
        location.StartColumn.ShouldBe(1);
        location.EndLine.ShouldBe(3);
        location.EndColumn.ShouldBe(5);
        location.StartOffset.ShouldBe(0);
        location.EndOffset.ShouldBe(10);
    }

    [TestMethod]
    public void CsharpMutation_NeutralFields_MatchRoslynNodeLocationAndText()
    {
        const string source = """
            class C
            {
                int M(int a, int b)
                {
                    var s = "text";
                    return a + b;
                }
            }
            """;
        var syntaxTree = CSharpSyntaxTree.ParseText(source, path: "C.cs");
        var compilation = CSharpCompilation.Create(null)
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(syntaxTree);

        Target.Mutate(syntaxTree, compilation.GetSemanticModel(syntaxTree));
        var mutations = Target.GetLatestMutantBatch().Select(m => m.Mutation).ToList();

        mutations.ShouldNotBeEmpty();
        foreach (var mutation in mutations)
        {
            var expected = mutation.OriginalNode.GetLocation().GetMappedLineSpan();
            mutation.Location.ShouldNotBeNull();
            mutation.Location.FilePath.ShouldBe(expected.Path);
            mutation.Location.StartLine.ShouldBe(expected.StartLinePosition.Line + 1);
            mutation.Location.StartColumn.ShouldBe(expected.StartLinePosition.Character + 1);
            mutation.Location.EndLine.ShouldBe(expected.EndLinePosition.Line + 1);
            mutation.Location.EndColumn.ShouldBe(expected.EndLinePosition.Character + 1);
            mutation.OriginalText.ShouldBe(mutation.OriginalNode.ToString());
            mutation.ReplacementText.ShouldBe(mutation.ReplacementNode.ToString());
        }
    }
}
