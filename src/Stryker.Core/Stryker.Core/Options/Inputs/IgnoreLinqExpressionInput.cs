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
            if (SuppliedInput is { } && SuppliedInput.Any(w => w.ToLower().StartsWith("linq")))
            {
                var excluded = new List<LinqExpression>();

                // Get all LinqExpression
                var linqExpressions = Enum.GetValues(typeof(LinqExpression))
                    .Cast<LinqExpression>();

                var linqMethodsInput = SuppliedInput
                                        .Select(s => s.Split("."))
                                        .Where(s => s.Length == 2);

                if (!linqMethodsInput.Any())
                    throw new InputException($"Invalid excluded linq expression. The excluded linq expression options are [{string.Join(", ", linqExpressions.Select(x => "linq."+ x.ToString()))}]");


                foreach (var mutatorToExclude in linqMethodsInput)
                {
                    // Find any LinqExpression that matches the name passed by the user
                    var linqMethod = linqExpressions.FirstOrDefault(
                        x => x.ToString().ToLower().Equals(mutatorToExclude[1].ToLower()));
                    if (linqMethod != LinqExpression.None)
                        excluded.Add(linqMethod);
                    else
                        throw new InputException($"Invalid excluded linq expression ({mutatorToExclude[1]}). The excluded linq expression options are [{string.Join(", ", linqExpressions.Select(x => x.ToString()))}]");
                }

               return excluded;
            }
            return Enumerable.Empty<LinqExpression>();
        }
    }
}
