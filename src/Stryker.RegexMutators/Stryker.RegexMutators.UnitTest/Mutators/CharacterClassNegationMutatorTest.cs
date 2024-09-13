using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.CharacterClass;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public class CharacterClassNegationMutatorTest
{
    [TestMethod]
    public void ShouldNegateUnnegatedCharacterClass()
    {
        // Arrange
        var characters = new List<RegexNode> { new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c') };
        var characterSet = new CharacterClassCharacterSetNode(characters);
        var subtractionCharacterSet = new CharacterClassCharacterSetNode(new CharacterNode('a'));
        var subtraction = new CharacterClassNode(subtractionCharacterSet, false);
        var characterClass = new CharacterClassNode(characterSet, subtraction, false);
        var childNodes = new List<RegexNode> { new CharacterNode('x'), characterClass, new CharacterNode('y') };
        var root = new ConcatenationNode(childNodes);
        var target = new CharacterClassNegationMutator();

        // Act
        var result = target.ApplyMutations(characterClass, root);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(characterClass);
        mutation.ReplacementNode.ToString().ShouldBe("[^abc-[a]]");
        mutation.ReplacementPattern.ShouldBe("x[^abc-[a]]y");
        mutation.DisplayName.ShouldBe("Regex character class negation mutation");
        mutation.Description.ShouldBe("Character class \"[abc-[a]]\" was replaced with \"[^abc-[a]]\" at offset 1.");
    }

    [TestMethod]
    public void ShouldUnnegateNegatedCharacterClass()
    {
        // Arrange
        var characters = new List<RegexNode> { new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c') };
        var characterSet = new CharacterClassCharacterSetNode(characters);
        var subtractionCharacterSet = new CharacterClassCharacterSetNode(new CharacterNode('a'));
        var subtraction = new CharacterClassNode(subtractionCharacterSet, false);
        var characterClass = new CharacterClassNode(characterSet, subtraction, true);
        var childNodes = new List<RegexNode> { new CharacterNode('x'), characterClass, new CharacterNode('y') };
        var root = new ConcatenationNode(childNodes);
        var target = new CharacterClassNegationMutator();

        // Act
        var result = target.ApplyMutations(characterClass, root);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(characterClass);
        mutation.ReplacementNode.ToString().ShouldBe("[abc-[a]]");
        mutation.ReplacementPattern.ShouldBe("x[abc-[a]]y");
        mutation.DisplayName.ShouldBe("Regex character class negation mutation");
        mutation.Description.ShouldBe("Character class \"[^abc-[a]]\" was replaced with \"[abc-[a]]\" at offset 1.");
    }

    [TestMethod]
    public void MutateShouldNotMutateNonCharacterClassNode()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var root = new ConcatenationNode(characterNode);
        var target = new CharacterClassNegationMutator();

        // Act
        var result = target.Mutate(characterNode, root);

        // Assert
        result.ShouldBeEmpty();
    }
}
