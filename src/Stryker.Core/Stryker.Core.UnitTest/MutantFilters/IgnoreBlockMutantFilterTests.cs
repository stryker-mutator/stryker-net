using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.MutantFilters;
using Shouldly;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class IgnoreBlockMutantFilterTests : TestBase
    {
        [Fact]
        public static void ShouldHaveName()
        {
            var sut = new IgnoreBlockMutantFilter();
            sut.DisplayName.ShouldBe("block already covered filter");
        }

        [Fact]
        public void Type_ShouldBeIgnoreBlockRemoval()
        {
            // Arrange
            var sut = new IgnoreBlockMutantFilter();

            // Assert
            sut.Type.ShouldBe(MutantFilter.IgnoreBlockRemoval);
        }

        [Fact]
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

        [Fact]
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
}
