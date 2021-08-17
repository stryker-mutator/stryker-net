using System;
using System.Collections.Generic;
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
        private IEnumerable<LinqExpression> AllLinqExpressions { get; } = Enum.GetValues(typeof(LinqExpression))
                    .Cast<LinqExpression>()
                    .Where(w => w != LinqExpression.None);


        [Fact]
        public void ShouldReturnEmptyWithNonLinqOptions()
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
        public void ShouldHaveDefault()
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
        public void ShouldOnlyValidateInputStartingWithLinq()
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
    }
}
