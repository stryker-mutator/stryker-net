using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.Core.UnitTest.Reporters.Json;

[TestClass]
public class JsonMutantTests : TestBase
{
    [TestMethod]
    public void JsonMutant_UsesNeutralLocation()
    {
        // The Roslyn node sits on line 1 while the neutral fields say line 7: the neutral fields must win.
        var mutant = new Mutant
        {
            Id = 1,
            Mutation = new CsharpMutation
            {
                DisplayName = "Arithmetic",
                OriginalNode = SyntaxFactory.ParseExpression("a + b"),
                ReplacementNode = SyntaxFactory.ParseExpression("a - b"),
                Location = new MutationLocation("file.cs", 7, 3, 8, 12, 40, 45),
                OriginalText = "x + y",
                ReplacementText = "x - y",
            }
        };

        var json = new JsonMutant(mutant);

        json.Location.Start.Line.ShouldBe(7);
        json.Location.Start.Column.ShouldBe(3);
        json.Location.End.Line.ShouldBe(8);
        json.Location.End.Column.ShouldBe(12);
        json.Replacement.ShouldBe("x - y");
    }

    [TestMethod]
    public void JsonMutant_WithoutLocation_FallsBackToFileStart()
    {
        var mutant = new Mutant { Id = 1, Mutation = new Mutation { DisplayName = "External", ReplacementText = "b" } };

        var json = new JsonMutant(mutant);

        json.Location.Start.Line.ShouldBe(1);
        json.Location.Start.Column.ShouldBe(1);
        json.Location.End.Line.ShouldBe(1);
        json.Location.End.Column.ShouldBe(1);
    }
}
