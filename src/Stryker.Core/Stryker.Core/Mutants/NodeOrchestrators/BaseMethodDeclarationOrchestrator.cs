﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class BaseMethodDeclarationOrchestrator : NodeSpecificOrchestrator<BaseMethodDeclarationSyntax>
    {
        protected override SyntaxNode OrchestrateMutation(BaseMethodDeclarationSyntax node, MutationContext context)
        {
            var newContext = context.Clone();
            var mutatedNode = (BaseMethodDeclarationSyntax) base.OrchestrateMutation(node, newContext);

            if (mutatedNode.Body == null)
            {
                if (!newContext.HasBlockLevelMutant)
                {
                    return mutatedNode;
                }

                // we convert the mutated node to the body form
                mutatedNode = MutantPlacer.ConvertExpressionToBody(mutatedNode);

                // we convert the original node to the body form
                StatementSyntax mutatedBlock = mutatedNode.Body;

                var converter = mutatedNode.NeedsReturn()
                    ? (Func<Mutation, StatementSyntax>) ((toConvert) =>
                        SyntaxFactory.ReturnStatement(node.ExpressionBody!.Expression.InjectMutation(toConvert)))
                    : (toConvert) =>
                        SyntaxFactory.ExpressionStatement(node.ExpressionBody!.Expression.InjectMutation(toConvert));

                mutatedBlock =
                    MutantPlacer.PlaceIfControlledMutations(mutatedBlock,
                        newContext.StatementLevelControlledMutations.Union(newContext.BlockLevelControlledMutations).
                        Select( m => (m.Id, converter(m.Mutation))));
                mutatedNode = mutatedNode.ReplaceNode(mutatedNode.Body!, 
                    SyntaxFactory.Block(mutatedBlock));
            }

            // If method return type is void skip the node
            if (!mutatedNode.NeedsReturn())
            {
                return mutatedNode;
            }

            if (mutatedNode.Body.ContainsNodeThatVerifies(x => x.IsKind(SyntaxKind.YieldReturnStatement), false))
            {
                // not need to add yield return at the end of an enumeration method
                return mutatedNode;
            }

            if (mutatedNode.Body!.Statements.Last().Kind() == SyntaxKind.ReturnStatement)
            {
                return mutatedNode;
            }

            var returnType = mutatedNode.ReturnType();

            // the GenericNameSyntax node can be encapsulated by QualifiedNameSyntax nodes
            var genericReturn = returnType.DescendantNodesAndSelf().OfType<GenericNameSyntax>().FirstOrDefault();
            if (mutatedNode.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)))
            {
                if (genericReturn != null)
                {
                    // if the method is async and returns a generic task, make the return default return the underlying type
                    returnType = genericReturn.TypeArgumentList.Arguments.First();
                }
                else
                {
                    // if the method is async but returns a non-generic task, don't add the return default
                    return mutatedNode;
                }
            }

            var newBody = mutatedNode.Body!.AddStatements(
                MutantPlacer.AnnotateHelper(
                    SyntaxFactory.ReturnStatement(SyntaxFactory.DefaultExpression(returnType).WithLeadingTrivia(SyntaxFactory.Space))));
            mutatedNode = mutatedNode.ReplaceNode(mutatedNode.Body, newBody);

            return mutatedNode;
        }

        public BaseMethodDeclarationOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}