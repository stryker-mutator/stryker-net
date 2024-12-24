using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;

namespace Stryker.Core.UnitTest.MutantFilters;

[TestClass]
public class IgnoreBlockMutantFilterTests : TestBase
{
    [TestMethod]
    public void ShouldHaveName()
    {
        var sut = new IgnoreBlockMutantFilter();
        sut.DisplayName.ShouldBe("block already covered filter");
    }

    [TestMethod]
    public void Type_ShouldBeIgnoreBlockRemoval()
    {
        // Arrange
        var sut = new IgnoreBlockMutantFilter();

        // Assert
        sut.Type.ShouldBe(MutantFilter.IgnoreBlockRemoval);
    }

    [TestMethod]
    public void MutantFilter_WithMutationsInBlock_ShouldIgnoreBlockMutant()
    {
        // Arrange
        var source = @"
public void SomeMethod()
{
    var x = 1 + 1;
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
        var blockNode = syntaxTree.DescendantNodes().OfType<BlockSyntax>().First();
        var binaryExpressionNode = blockNode.DescendantNodes().OfType<ExpressionSyntax>().First();

        var blockMutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = blockNode,
                Type = Mutator.Block,
            }
        };
        var binaryExpressionMutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = binaryExpressionNode,
            }
        };

        var sut = new IgnoreBlockMutantFilter();

        // Act
        var filteredMutants = sut.FilterMutants(new[] { blockMutant, binaryExpressionMutant }, null, null);

        // Assert
        filteredMutants.ShouldContain(binaryExpressionMutant);
        filteredMutants.ShouldNotContain(blockMutant);
    }

    [TestMethod]
    public void MutantFilter_WithNoMutationsInBlock_ShouldNotIgnoreBlockMutant()
    {
        // Arrange
        var source = @"
public void SomeMethod()
{
    var x = 1 + 1;
}";
        var syntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
        var blockNode = syntaxTree.DescendantNodes().OfType<BlockSyntax>().First();
        var blockMutant = new Mutant
        {
            Mutation = new Mutation
            {
                OriginalNode = blockNode,
                Type = Mutator.Block,
            }
        };
        var sut = new IgnoreBlockMutantFilter();

        // Act
        var filteredMutants = sut.FilterMutants(new[] { blockMutant }, null, null);

        // Assert
        filteredMutants.ShouldContain(blockMutant);
        blockMutant.ResultStatus.ShouldNotBe(MutantStatus.Ignored);
    }
}
