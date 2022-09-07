using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    /// <summary>
    /// Mutator that will mutate the access to <c>string.Empty</c> to a string that is not empty.
    /// </summary>
    /// <remarks>
    /// Will only apply the mutation to the lowercase <c>string</c> since that is a reserved keyword in c# and can be distinguished from any variable or member access.
    /// </remarks>
    public class StringEmptyMutator: MutatorBase<MemberAccessExpressionSyntax>
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        /// <inheritdoc />
        public override IEnumerable<Mutation> ApplyMutations(MemberAccessExpressionSyntax node)
        {
            if (node.Expression is PredefinedTypeSyntax typeSyntax &&
                typeSyntax.Keyword.ValueText == "string" &&
                node.Name.Identifier.ValueText == nameof(string.Empty))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode =
                        SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal("Stryker was here!")),
                    DisplayName = "String mutation",
                    Type = Mutator.String
                };
            }
        }
    }
}
