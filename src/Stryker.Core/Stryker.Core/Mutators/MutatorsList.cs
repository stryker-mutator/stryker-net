using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Mutators
{
    public static class MutatorsList
    {
        public static Dictionary<string, IEnumerable<IMutator>> _mutationToMutatorMapping { get; set; } = new Dictionary<string, IEnumerable<IMutator>>()
            {
                { "Binairy expressions", new IMutator[] { new BinaryExpressionMutator() } },
                { "Boolean", new IMutator[] { new BooleanMutator() } },
                { "Assignment statements", new IMutator[] { new AssignmentStatementMutator() } },
                { "Unary operators", new IMutator[] { new PrefixUnaryMutator(), new PostfixUnaryMutator() } },
                { "Update operators", new IMutator[] { new BinaryExpressionMutator() } },
                { "Binairy expressions", new IMutator[] { new BinaryExpressionMutator() } },
                { "Checked statements", new IMutator[] { new CheckedMutator() } },
                { "Linq expressions", new IMutator[] { new LinqMutator() } },
                { "String mutator", new IMutator[] { new InterpolatedStringMutator(), new StringMutator() } }
            };
        
        public static IEnumerable<IMutator> GetMutators(string[] mutationsToExclude)
        {
            return new List<IMutator>()
                {
                    // the default list of mutators
                    new BinaryExpressionMutator(),
                    new BooleanMutator(),
                    new AssignmentStatementMutator(),
                    new PrefixUnaryMutator(),
                    new PostfixUnaryMutator(),
                    new CheckedMutator(),
                    new LinqMutator(),
                    new StringMutator(),
                    new InterpolatedStringMutator()
                };
        }
    }
}
