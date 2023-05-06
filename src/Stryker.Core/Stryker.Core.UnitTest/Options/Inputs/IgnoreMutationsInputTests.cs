using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Inputs;
using Stryker.Core.UnitTest.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class IgnoreMutationsInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new IgnoreMutationsInput();
        target.HelpText.ShouldBe(@"The given mutators will be excluded for this mutation testrun.
    This argument takes a json array as value. Example: ['string', 'logical'] | default: []");
    }

    [Fact]
    public void ShouldValidateExcludedMutation()
    {
        var target = new IgnoreMutationsInput { SuppliedInput = new[] { "gibberish" } };

        var ex = Should.Throw<InputException>(() => target.Validate<Mutator>());

        ex.Message.ShouldBe($"Invalid excluded mutation (gibberish). The excluded mutations options are [Statement, Arithmetic, Block, Equality, Boolean, Logical, Assignment, Unary, Update, Checked, Linq, String, Bitwise, Initializer, Regex, NullCoalescing, Math]");
    }

    [Fact]
    public void ShouldHaveDefault()
    {
        var target = new IgnoreMutationsInput { SuppliedInput = new string[] { } };

        var result = target.Validate<Mutator>();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldIgnoreMutatorWithOptions()
    {
        var target = new IgnoreMutationsInput { SuppliedInput = new string[] { "linq.Sum", "string.empty", "logical.equal" } };

        var result = target.Validate<Mutator>();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldReturnMultipleMutators()
    {
        var target = new IgnoreMutationsInput { SuppliedInput = new[] {
            Mutator.String.ToString(),
            Mutator.Regex.ToString(),
        } };

        var result = target.Validate<Mutator>();

        result.Count().ShouldBe(2);
        result.First().ShouldBe(Mutator.String);
        result.ElementAt(1).ShouldBe(Mutator.Regex);
    }


    private IEnumerable<LinqExpression> AllLinqExpressions { get; } = Enum.GetValues(typeof(LinqExpression))
                .Cast<LinqExpression>()
                .Where(w => w != LinqExpression.None);


    [Fact]
    public void ShouldReturnEmptyLinqExpressionsWithNonLinqOptions()
    {
        var target = new IgnoreMutationsInput { SuppliedInput = new[] { "gibberish" } };
        var linqExpressions = target.ValidateLinqExpressions();
        linqExpressions.ShouldBeEmpty();
    }


    [Theory]
    [InlineData("linq.nothing")]
    [InlineData("linq.test")]
    [InlineData("linq.first.default")]
    public void ShouldValidateExcludedLinqExpression(string method)
    {
        var target = new IgnoreMutationsInput
        {
            SuppliedInput = new[] { method }
        };

        var ex = Should.Throw<InputException>(() => target.ValidateLinqExpressions());

        ex.Message.ShouldBe($"Invalid excluded linq expression ({string.Join(".", method.Split(".").Skip(1))}). The excluded linq expression options are [{string.Join(", ", AllLinqExpressions)}]");
    }

    [Fact]
    public void ShouldHaveDefaultLinqExpressions()
    {
        var target = new IgnoreMutationsInput { SuppliedInput = new string[] { } };

        var linqExpressions = target.ValidateLinqExpressions();

        linqExpressions.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldReturnMultipleLinqExpressions()
    {
        var target = new IgnoreMutationsInput
        {
            SuppliedInput = new[] {
            "linq.FirstOrDefault",
            "linq.First",
            }
        };

        var linqExpressions = target.ValidateLinqExpressions();

        linqExpressions.Count().ShouldBe(2);
        linqExpressions.First().ShouldBe(LinqExpression.FirstOrDefault);
        linqExpressions.Last().ShouldBe(LinqExpression.First);
    }

    [Fact]
    public void ShouldIgnoreIncorrectFormatWhenValidateLinqExpressions()
    {
        var target = new IgnoreMutationsInput
        {
            SuppliedInput = new[] {
            "linq.Max",
            "linq.Sum",
            "test",
            }
        };

        var linqExpressions = target.ValidateLinqExpressions();

        linqExpressions.Count().ShouldBe(2);
        linqExpressions.First().ShouldBe(LinqExpression.Max);
        linqExpressions.Last().ShouldBe(LinqExpression.Sum);
    }

    /// <summary>
    /// This test is needed as other mutators also have "statement" in their name. It should pick the right mutator.
    /// </summary>
    [Fact]
    public void ShouldIgnoreStatementMutator()
    {
        var target = new IgnoreMutationsInput
        {
            SuppliedInput = new[] { "statement" }
        };

        var mutators = target.Validate<Mutator>();

        mutators.ShouldHaveSingleItem().ShouldBe(Mutator.Statement);
    }

    [Fact]
    public void ShouldIgnoreBasedOnEitherDescription()
    {
        var targetWithFirstDescription = new IgnoreMutationsInput
        {
            SuppliedInput = new[] { "Multi-description mutator" }
        };
        var targetWithSecondDescription = new IgnoreMutationsInput
        {
            SuppliedInput = new[] { "Two descriptions mutator" }
        };

        var mutatorsWithFirstDescription = targetWithFirstDescription.Validate<TestMutator>();
        var mutatorsWithSecondDescription = targetWithSecondDescription.Validate<TestMutator>();

        mutatorsWithFirstDescription.ShouldHaveSingleItem().ShouldBe(TestMutator.MultipleDescriptions);
        mutatorsWithSecondDescription.ShouldHaveSingleItem().ShouldBe(TestMutator.MultipleDescriptions);
    }
}
