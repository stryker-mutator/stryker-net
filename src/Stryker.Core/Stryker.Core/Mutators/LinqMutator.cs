using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    /// <summary>
    ///     Mutator Implementation for LINQ Mutations
    /// </summary>
    public class LinqMutator : Mutator<IdentifierNameSyntax>, IMutator
    {
        #region Private Properties

        /// <summary>
        ///     Dictionary which maps original linq expressions to the target mutation
        /// </summary>
        private Dictionary<LinqExpression, LinqExpression> _kindsToMutate { get; }

        #endregion

        #region Constructor

        /// <summary>
        ///     Constructor for the <see cref="LinqMutator"/>
        /// </summary>
        public LinqMutator()
        {
            _kindsToMutate = new Dictionary<LinqExpression, LinqExpression>
            {
                { LinqExpression.Distinct, LinqExpression.None },
                { LinqExpression.Reverse, LinqExpression.None },
                { LinqExpression.OrderBy, LinqExpression.None },
                { LinqExpression.OrderByDescending, LinqExpression.None },
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

        #endregion

        #region Mutation Overrides

        /// <summary>
        ///     Apply mutations to an <see cref="IdentifierNameSyntax"/>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override IEnumerable<Mutation> ApplyMutations(IdentifierNameSyntax node)
        {
            if (Enum.TryParse(node.Identifier.ValueText, out LinqExpression expression) &&
                _kindsToMutate.TryGetValue(expression, out LinqExpression replacementExpression))
            {
                SyntaxNode replacement = SyntaxFactory.IdentifierName(replacementExpression.ToString());
                string displayName = $"{node.Identifier.ValueText} to {replacement} mutation";

                if (replacementExpression.Equals(LinqExpression.None))
                {
                    replacement = SyntaxFactory.IdentifierName(string.Empty);
                    displayName = $"{node.Identifier.ValueText} to None mutation";
                }

                yield return new Mutation
                {
                    DisplayName = displayName,
                    OriginalNode = node,
                    ReplacementNode = replacement,
                    Type = nameof(LinqMutator)
                };
            }
        }

        #endregion

    }

    /// <summary>
    ///     Enumeration for the different kinds of linq expressions
    /// </summary>
    public enum LinqExpression
    {
        None,
        Distinct,
        Reverse,
        OrderBy,
        OrderByDescending,
        FirstOrDefault,
        SingleOrDefault,
        First,
        Last,
        All,
        Any,
        Skip,
        Take,
        SkipWhile,
        TakeWhile,
        Min,
        Max,
        Sum,
        Count
    }
}
