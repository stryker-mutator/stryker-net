using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Enumerations;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class LinqMutator : Mutator<InvocationExpressionSyntax>, IMutator
    {

        private Dictionary<LinqExpression, LinqExpression> _kindsToMutate { get; }

        public LinqMutator()
        {
            _kindsToMutate = new Dictionary<LinqExpression, LinqExpression>
            {
                { LinqExpression.FirstOrDefault, LinqExpression.SingleOrDefault },
                { LinqExpression.SingleOrDefault, LinqExpression.FirstOrDefault },
                { LinqExpression.First, LinqExpression.Last },
                { LinqExpression.Last, LinqExpression.First },
                { LinqExpression.All, LinqExpression.Any },
                { LinqExpression.Any, LinqExpression.All },
                { LinqExpression.Skip, LinqExpression.Take },
                { LinqExpression.Take, LinqExpression.Skip },
                { LinqExpression.SkipWhile, LinqExpression.TakeWhile },
                { LinqExpression.TakeWhile, LinqExpression.SkipWhile },
                { LinqExpression.Min, LinqExpression.Max },
                { LinqExpression.Max, LinqExpression.Min },
                { LinqExpression.Sum, LinqExpression.Count },
                { LinqExpression.Count, LinqExpression.Sum }
            };
        }

        public override IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax node)
        {
            // Determine if it's an linq node
            bool isLinqNode = node.DescendantNodes().Any(
                d => d.Kind().Equals(SyntaxKind.IdentifierName));

            if (!isLinqNode)
            {
                yield break;
            }

            IEnumerable<IdentifierNameSyntax> identifierNodes =
                node.DescendantNodes()
                    .Where(d => d.Kind().Equals(SyntaxKind.IdentifierName))
                    .Cast<IdentifierNameSyntax>();

            foreach (var identifierNode in identifierNodes)
            {
                if (Enum.TryParse(identifierNode.Identifier.ValueText, out LinqExpression expression) &&
                    _kindsToMutate.TryGetValue(expression, out LinqExpression replacementExpression))
                {
                    var replacement = SyntaxFactory.IdentifierName(replacementExpression.ToString());

                    yield return new Mutation()
                    {
                        DisplayName = $"{identifierNode.Identifier.ValueText} to {replacement} mutation",
                        OriginalNode = node,
                        ReplacementNode = replacement,
                        Type = nameof(LinqMutator)
                    };

                }
            }

        }
    }
}
