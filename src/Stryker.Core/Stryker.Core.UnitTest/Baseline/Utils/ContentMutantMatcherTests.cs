using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Core.Baseline.Utils;
using Stryker.Core.DiffProviders;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.Core.UnitTest.Baseline.Utils;

[TestClass]
public class ContentMutantMatcherTests : TestBase
{
    private const string Source = "class C { void M() { var foo = \"bar\"; } }";

    private static (Mutant mutant, JsonMutant baselineMutant, DiffResult diff) CreateUnchangedScenario()
    {
        var tree = CSharpSyntaxTree.ParseText(Source);
        var literal = tree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().First();

        var mutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = literal,
                DisplayName = "String literal"
            }
        };

        var baselineMutant = new JsonMutant
        {
            MutatorName = "String literal",
            Location = new Location(literal.GetLocation().GetMappedLineSpan())
        };

        var diff = new DiffResult([new DiffChange { Operation = DiffOperation.Equal, Text = Source }], Source, Source);

        return (mutant, baselineMutant, diff);
    }

    [TestMethod]
    public void MatchByLocation_ReturnsMutant_WhenLocationAndMutatorNameMatch()
    {
        // Arrange
        var (mutant, baselineMutant, diff) = CreateUnchangedScenario();
        var target = new ContentMutantMatcher();

        // Act
        var results = target.MatchByLocation([mutant], baselineMutant, diff);

        // Assert
        results.ShouldHaveSingleItem().ShouldBe(mutant);
    }

    [TestMethod]
    public void MatchByLocation_ReturnsEmpty_WhenMutatorNameDiffers()
    {
        // Arrange
        var (mutant, baselineMutant, diff) = CreateUnchangedScenario();
        var differentBaselineMutant = new JsonMutant
        {
            MutatorName = "Some other mutator",
            Location = baselineMutant.Location
        };
        var target = new ContentMutantMatcher();

        // Act
        var results = target.MatchByLocation([mutant], differentBaselineMutant, diff);

        // Assert
        results.ShouldBeEmpty();
    }

    [TestMethod]
    public void MatchByLocation_ReturnsEmpty_WhenTheMutantsCodeChanged()
    {
        // Arrange
        var (mutant, baselineMutant, _) = CreateUnchangedScenario();
        var target = new ContentMutantMatcher();

        // The code around (and including) the mutant changed, so nothing maps back to an unchanged region.
        var changedDiff = new DiffMatchPatchProvider().GetContentDiff(Source, "class C { void M() { var foo = \"baz\"; } }");

        // Act
        var results = target.MatchByLocation([mutant], baselineMutant, changedDiff);

        // Assert
        results.ShouldBeEmpty();
    }

    [TestMethod]
    public void MatchByLocation_ReturnsAllMutants_WhenMultipleMutantsShareTheSameLocation()
    {
        // Arrange - simulates duplicate mutations at the same location (e.g. #1296): matching is
        // location-based, so multiple matches are no longer ambiguous and are all returned.
        var (firstMutant, baselineMutant, diff) = CreateUnchangedScenario();
        var secondMutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = firstMutant.Mutation.OriginalNode,
                DisplayName = firstMutant.Mutation.DisplayName
            }
        };
        var target = new ContentMutantMatcher();

        // Act
        var results = target.MatchByLocation([firstMutant, secondMutant], baselineMutant, diff);

        // Assert
        results.Count().ShouldBe(2);
    }
}
