using System;
using System.Linq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class IgnoreLinqExpressionInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new IgnoreLinqExpressionInput();
            target.HelpText.ShouldBe(@"The given linq expression will be excluded for this mutation testrun.
    This argument takes a json array as value. Example: ['linq.FirstOrDefault', 'linq.First'] | default: []");
        }

        [Fact]
        public void ShouldValidateOnlyLinqOptions()
        {
            var target = new IgnoreLinqExpressionInput { SuppliedInput = new[] { "gibberish" } };
            var result = target.Validate();
            result.ShouldBeEmpty();
        }


        [Theory]
        [InlineData("linq.nothing")]
        [InlineData("linq.test")]
        public void ShouldValidateExcludedLinqMutation(string method)
        {
            var target = new IgnoreLinqExpressionInput
            {
                SuppliedInput = new[] { method }
            };

            var ex = Should.Throw<InputException>(() => target.Validate());

            ex.Message.ShouldBe($"Invalid excluded linq expression ({method.Split(".").Last()}). The excluded linq expression options are [{string.Join(", ", Enum.GetNames<LinqExpression>())}]");
        }

        [Fact]
        public void ShouldHaveDefault()
        {
            var target = new IgnoreLinqExpressionInput { SuppliedInput = new string[] { } };

            var result = target.Validate();

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldReturnMultipleMutators()
        {
            var target = new IgnoreLinqExpressionInput
            {
                SuppliedInput = new[] {
                "linq.FirstOrDefault",
                "linq.First",
                }
            };

            var result = target.Validate();

            result.Count().ShouldBe(2);
            result.First().ShouldBe(LinqExpression.FirstOrDefault);
            result.Last().ShouldBe(LinqExpression.First);
        }
    }
}
