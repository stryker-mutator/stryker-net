using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stryker.Core.Mutants
{
    public interface IMutantOrchestrator
    {
        SyntaxNode Mutate(SyntaxNode rootNode);
        /// <summary>
        /// Gets the stored mutants and resets the mutant list to an empty collection
        /// </summary>
        /// <returns>Mutants</returns>
        IEnumerable<Mutant> GetLatestMutantBatch();
    }

    /// <summary>
    /// Mutates abstract syntax trees using mutators and places all mutations inside the abstract syntax tree.
    /// Orchestrator: to arrange or manipulate, especially by means of clever or thorough planning or maneuvering.
    /// </summary>
    public class MutantOrchestrator : IMutantOrchestrator
    {
        private ICollection<Mutant> Mutants { get; set; }
        private int MutantCount { get; set; }
        private IEnumerable<IMutator> Mutators { get; set; }
        private ILogger Logger { get; set; }

        private static readonly Type[] ExpressionStatementNeedingIf =
        {
            typeof(AssignmentExpressionSyntax), 
            typeof(PostfixUnaryExpressionSyntax), 
            typeof(PrefixUnaryExpressionSyntax)
        };
        
        /// <param name="mutators">The mutators that should be active during the mutation process</param>
        public MutantOrchestrator(IEnumerable<IMutator> mutators = null)
        {
            Mutators = mutators ?? new List<IMutator>()
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
                    new InterpolatedStringMutator(),
                    new NegateConditionMutator(),
                };
            Mutants = new Collection<Mutant>();
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutantOrchestrator>();
        }

        /// <summary>
        /// Gets the stored mutants and resets the mutant list to an empty collection
        /// </summary>
        /// <returns>Mutants</returns>
        public IEnumerable<Mutant> GetLatestMutantBatch()
        {
            var tempMutants = Mutants;
            Mutants = new Collection<Mutant>();
            return tempMutants;
        }

        /// <summary>
        /// Recursively mutates a single SyntaxNode
        /// </summary>
        /// <param name="currentNode">The current root node</param>
        /// <returns>Mutated node</returns>
        public SyntaxNode Mutate(SyntaxNode currentNode)
        {
            return Mutate(currentNode, new MutationContext());
        }

        private SyntaxNode Mutate(SyntaxNode currentNode, MutationContext context)
        {
            // don't mutate immutable nodes
            if (!SyntaxHelper.CanBeMutated(currentNode))
            {
                return currentNode;
            }

            context = context.UpdateContext(currentNode);
            switch (currentNode)
            {
                // apply statement specific strategies (where applicable)
                case ExpressionStatementSyntax tentativeAssignment when tentativeAssignment.Expression is AssignmentExpressionSyntax assign:
                    return MutateAssignment(tentativeAssignment, assign, context);
                case ExpressionStatementSyntax tentativeAssignment when tentativeAssignment.Expression is PostfixUnaryExpressionSyntax || tentativeAssignment.Expression is PrefixUnaryExpressionSyntax:
                    return MutateUnaryStatement(tentativeAssignment, context);
                case ExpressionStatementSyntax expressionStatement:
                    // we must skip the expression statement part
                    return currentNode.ReplaceNode(expressionStatement.Expression, Mutate(expressionStatement.Expression, context));
                case IfStatementSyntax ifStatement:
                    return MutateIfStatement(ifStatement, context);
                case ForStatementSyntax forStatement:
                    return MutateForStatement(forStatement, context);
            }
            return MutateExpression(currentNode, context);
        }

        private SyntaxNode MutateAssignment(ExpressionStatementSyntax tentativeAssignment, AssignmentExpressionSyntax assign, MutationContext context)
        {
            var expressionCopy = tentativeAssignment.TrackNodes(tentativeAssignment, assign, assign.Right);
            // mutate +=, *=, ...
            var result = MutateSubExpressionWithIfStatements(tentativeAssignment, expressionCopy, assign, context);
            // mutate the part right to the equal sign
            return result.ReplaceNode(result.GetCurrentNode(assign.Right), Mutate(assign.Right, context));
        }

        private SyntaxNode MutateIfStatement(IfStatementSyntax ifStatement, MutationContext context)
        {
            var mutatedIf = ifStatement.Else != null
                ? ifStatement.TrackNodes(ifStatement.Condition, ifStatement.Statement, ifStatement.Else)
                : ifStatement.TrackNodes(ifStatement.Condition, ifStatement.Statement);

            if (!ifStatement.Condition.ContainsDeclarations())
            {
                mutatedIf = mutatedIf.ReplaceNode(mutatedIf.GetCurrentNode(ifStatement.Condition),
                    Mutate(ifStatement.Condition, context));
            }

            if (ifStatement.Else != null)
            {
                mutatedIf = mutatedIf.ReplaceNode(mutatedIf.GetCurrentNode(ifStatement.Else), Mutate(ifStatement.Else, context));
            }

            return mutatedIf.ReplaceNode(mutatedIf.GetCurrentNode(ifStatement.Statement), Mutate(ifStatement.Statement, context));
        }

        private SyntaxNode MutateForStatement(ForStatementSyntax forStatement, MutationContext context)
        {
            // for needs special treatments for its incrementors
            StatementSyntax forWithMutantIncrementors = forStatement.TrackNodes(forStatement);

            foreach (var incrementor in forStatement.Incrementors)
            {
                forWithMutantIncrementors = MutateSubExpressionWithIfStatements(forStatement, forWithMutantIncrementors, incrementor, context);
            }

            var originalFor = forWithMutantIncrementors.GetCurrentNode(forStatement);

            // mutate condition, if any
            ForStatementSyntax mutatedFor;
            StatementSyntax statementPart;
            if (forStatement.Condition == null)
            {
                mutatedFor = forStatement;
                statementPart = forStatement.Statement;
            }
            else
            {
                mutatedFor = forStatement.TrackNodes(forStatement.Condition, forStatement.Statement);
                mutatedFor = mutatedFor.ReplaceNode(mutatedFor.GetCurrentNode(forStatement.Condition),
                    Mutate(forStatement.Condition, context));
                statementPart = mutatedFor.GetCurrentNode(forStatement.Statement);
            }

            // mutate the statement/block
            mutatedFor = mutatedFor.ReplaceNode(statementPart, Mutate(forStatement.Statement, context));
            // and now we replace it
            return forWithMutantIncrementors.ReplaceNode(originalFor, mutatedFor);
        }
    
        private SyntaxNode MutateUnaryStatement(ExpressionStatementSyntax expressionStatement, MutationContext context)
        {
            var expressionCopy = expressionStatement.TrackNodes(expressionStatement, expressionStatement.Expression);
            return MutateSubExpressionWithIfStatements(expressionStatement, expressionCopy, expressionStatement.Expression, context);
        }

        private SyntaxNode MutateExpression(SyntaxNode currentNode, MutationContext context)
        {
            
            var replaceNode = MutateExpressions(currentNode, context);
            if (replaceNode != null)
            {
                return replaceNode;
            }
            
            // Nothing to mutate, dig further
            var children = currentNode.ChildNodes().ToList();
            var childCopy = currentNode.TrackNodes(children);
            foreach (var child in children)
            {
                var originalNode = childCopy.GetCurrentNode(child);
                var mutatedNode = Mutate(child, context);
                // if mutation was successful
                if (!mutatedNode.IsEquivalentTo(originalNode))
                {
                    childCopy = childCopy.ReplaceNode(originalNode, mutatedNode);
                }
            }
            return childCopy;
        }

        private SyntaxNode MutateExpressions(SyntaxNode currentNode, MutationContext context)
        {
            
            switch (currentNode)
            {
                case InvocationExpressionSyntax invocationExpression when invocationExpression.ArgumentList.Arguments.Count == 0:
                {
                    var mutant = FindMutants(invocationExpression, context).FirstOrDefault();
                    if (mutant != null)
                    {
                        Mutants.Add(mutant);
                        return MutantPlacer.PlaceWithConditionalExpression(invocationExpression, mutant.Mutation.ReplacementNode as ExpressionSyntax, mutant.Id);
                    }
                    break;
                }
            }
            
            var expressions = SyntaxHelper.EnumerateSubExpressions(currentNode).Where(x => x != null).ToList();
            if (!expressions.Any())
            {
                return null;
            }
            
            var currentNodeCopy = currentNode.TrackNodes(expressions);
            foreach (var expressionSyntax in expressions)
            {
                var currentExpressionSyntax = currentNodeCopy.GetCurrentNode(expressionSyntax);
                if (expressionSyntax is InvocationExpressionSyntax)
                {
                    // chained invocations, we will recurse
                    var mutant = Mutate(expressionSyntax, context);
                    currentNodeCopy = currentNodeCopy.ReplaceNode(currentExpressionSyntax, mutant);
                    continue;
                }
 // attempts to mutate the expression as a whole
                    currentNodeCopy = currentNodeCopy.ReplaceNode(currentExpressionSyntax,
                        MutateWithConditionalExpressions(expressionSyntax, context));
            }

            return currentNodeCopy;

        }

        private IEnumerable<Mutant> FindMutants(SyntaxNode current, MutationContext context)
        {
            return Mutators.SelectMany(mutator => ApplyMutator(current, mutator, context));
        }

        private StatementSyntax MutateSubExpressionWithIfStatements(StatementSyntax originalNode, StatementSyntax nodeToReplace, SyntaxNode subExpression, MutationContext context)
        {
            var ast = nodeToReplace;
            // The mutations should be placed using an IfStatement
            foreach (var mutant in FindMutants(subExpression, context))
            {
                var mutatedNode = ApplyMutant(originalNode, mutant);
                ast = MutantPlacer.PlaceWithIfStatement(ast, mutatedNode, mutant.Id);
            }
            return ast;
        }

        private SyntaxNode MutateWithConditionalExpressions(ExpressionSyntax currentNode, MutationContext context)
        {
            var expressionAst = currentNode.TrackNodes(currentNode.ChildNodes().Append(currentNode));
            foreach (var childNode in currentNode.ChildNodes())
            {
                var mutatedChild = Mutate(childNode, context);
                if (mutatedChild != childNode)
                {
                    expressionAst = expressionAst.ReplaceNode(expressionAst.GetCurrentNode(childNode), mutatedChild);
                }
            }

            foreach (var mutant in FindMutants(currentNode, context))
            {
                expressionAst = MutantPlacer.PlaceWithConditionalExpression(expressionAst, ApplyMutant(currentNode, mutant), mutant.Id);
            }
            return expressionAst;
        }

        /// <summary>
        /// Mutates one single SyntaxNode using a mutator
        /// </summary>
        private IEnumerable<Mutant> ApplyMutator(SyntaxNode syntaxNode, IMutator mutator, MutationContext context)
        {
            var mutations = mutator.Mutate(syntaxNode);
            foreach (var mutation in mutations)
            {
                Logger.LogDebug("Mutant {0} created {1} -> {2} using {3}", MutantCount, mutation.OriginalNode, mutation.ReplacementNode, mutator.GetType());
                yield return new Mutant()
                {
                    Id = MutantCount++,
                    Mutation = mutation,
                    ResultStatus = MutantStatus.NotRun,
                    IsStaticValue = context.InStaticValue
                };
            }
        }

        private T ApplyMutant<T>(T node, Mutant mutant) where T: SyntaxNode
        {
            Mutants.Add(mutant);
            return node.ReplaceNode(mutant.Mutation.OriginalNode, mutant.Mutation.ReplacementNode);
        }
    }
}
