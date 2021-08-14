using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;

namespace Stryker.Core.Options.Inputs
{
    public class IgnoreLinqExpressionInput : Input<IEnumerable<string>>
    {
        public override IEnumerable<string> Default => Enumerable.Empty<string>();

        protected override string Description => @"The given linq expression will be excluded for this mutation testrun.
    This argument takes a json array as value. Example: ['linq.FirstOrDefault', 'linq.First']";

        public IEnumerable<LinqExpression> Validate()
        {
            if (SuppliedInput is { } && SuppliedInput.Any(w => w.ToLower().StartsWith("linq.")))
            {
                var excludedLinqExpressions = new List<LinqExpression>();

                // Get all LinqExpression
                var linqExpressions = Enum.GetValues(typeof(LinqExpression))
                    .Cast<LinqExpression>()
                    .Where(w => w != LinqExpression.None);

                var linqExpressionsInput = SuppliedInput
                                        .Select(s => s.Split("."))
                                        .Where(s => s.Length == 2);

                if (!linqExpressionsInput.Any())
                    throw new InputException($"Invalid excluded linq expression. The excluded linq expression options are [{string.Join(", ", linqExpressions.Select(x => "linq."+ x.ToString()))}]");


                foreach (var linqExpressionToExclude in linqExpressionsInput)
                {
                    // Find any LinqExpression that matches the name passed by the user
                    var linqExpression = linqExpressions.FirstOrDefault(
                        x => x.ToString().ToLower().Equals(linqExpressionToExclude[1].ToLower()));
                    if (linqExpression != LinqExpression.None)
                        excludedLinqExpressions.Add(linqExpression);
                    else
                        throw new InputException($"Invalid excluded linq expression ({linqExpressionToExclude[1]}). The excluded linq expression options are [{string.Join(", ", linqExpressions.Select(x => x.ToString()))}]");
                }

               return excludedLinqExpressions;
            }
            return Enumerable.Empty<LinqExpression>();
        }
    }
}
