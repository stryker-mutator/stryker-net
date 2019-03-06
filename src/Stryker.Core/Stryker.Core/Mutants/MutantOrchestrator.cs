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
        private ICollection<Mutant> _mutants { get; set; }
        private int _mutantCount { get; set; } = 0;
        private IEnumerable<IMutator> _mutators { get; set; }
        private ILogger _logger { get; set; }

        private static readonly Type[] expressionStatementNeedingIf = { typeof(AssignmentExpressionSyntax), typeof(PostfixUnaryExpressionSyntax), typeof(PrefixUnaryExpressionSyntax)};
        
        /// <param name="mutators">The mutators that should be active during the mutation process</param>
        public MutantOrchestrator(IEnumerable<IMutator> mutators = null)
        {
            _mutators = mutators ?? new List<IMutator>()
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
            _mutants = new Collection<Mutant>();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutantOrchestrator>();
        }

        /// <summary>
        /// Gets the stored mutants and resets the mutant list to an empty collection
        /// </summary>
        /// <returns>Mutants</returns>
        public IEnumerable<Mutant> GetLatestMutantBatch()
        {
            var tempMutants = _mutants;
            _mutants = new Collection<Mutant>();
            return tempMutants;
        }

        /// <summary>
        /// Recursively mutates a single SyntaxNode
        /// </summary>
        /// <param name="currentNode">The current root node</param>
        /// <returns>Mutated node</returns>
        public SyntaxNode Mutate(SyntaxNode currentNode)
        {
            // don't mutate immutable nodes
            if (!CanBeMutated(currentNode))
            {
                return currentNode;
            }
            // apply statement specific strategies (where applicable)
            if (currentNode is ExpressionStatementSyntax tentativeAssignment)
            {
                return MutateExpressionStatement(tentativeAssignment);
            }
            else if (currentNode is IfStatementSyntax ifStatement)
            {
                return MutateIfStatement(ifStatement);
            }
            else if (currentNode is ForStatementSyntax forStatement)
            {
                return MutateForStatement(forStatement);
            }
            // process expressions
            var replaceNode = MutateExpressions(currentNode);
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
                var mutatedNode = Mutate(child);
                if (!mutatedNode.IsEquivalentTo(originalNode))
                {
                    childCopy = childCopy.ReplaceNode(originalNode, mutatedNode);
                }
            }
            return childCopy;
        }

        private bool CanBeMutated(SyntaxNode node)
        {
            // don't mutate attributes or their arguments
            if (node is AttributeListSyntax)
            {
                return false;
            }
            // don't mutate parameters
            if (node is ParameterSyntax)
            {
                return false;
            }
            // don't mutate constant fields
            if (node is FieldDeclarationSyntax field)
            {
                if (field.Modifiers.Any(x => x.Kind() == SyntaxKind.ConstKeyword))
                {
                    return false;
                }
            }

            return true;
        }

        private SyntaxNode MutateExpressionStatement(ExpressionStatementSyntax expressionStatement)
        {
            if (expressionStatementNeedingIf.Contains(expressionStatement.Expression.GetType()))
            {
                var expressionCopy = expressionStatement.TrackNodes(expressionStatement, expressionStatement.Expression);
                SyntaxNode result = MutateSubExpressionWithIfStatements(expressionStatement, expressionCopy, expressionStatement.Expression);
                if (!(expressionStatement.Expression is AssignmentExpressionSyntax))
                {
                    return result;
                }

                return result.ReplaceNode(result.GetCurrentNode(expressionStatement), MutateExpressions(expressionStatement));
            }

            return MutateExpressions(expressionStatement);
        }

        private SyntaxNode MutateIfStatement(IfStatementSyntax ifStatement)
        {
            var mutatedIf = ifStatement.Else != null
                ? ifStatement.TrackNodes(ifStatement.Condition, ifStatement.Statement, ifStatement.Else)
                : ifStatement.TrackNodes(ifStatement.Condition, ifStatement.Statement);

            if (!ifStatement.Condition.ContainsDeclarations())
            {
                mutatedIf = mutatedIf.ReplaceNode(mutatedIf.GetCurrentNode(ifStatement.Condition),
                    MutateExpressions(ifStatement.Condition));
            }

            if (ifStatement.Else != null)
            {
                mutatedIf = mutatedIf.ReplaceNode(mutatedIf.GetCurrentNode(ifStatement.Else), Mutate(ifStatement.Else));
            }

            return mutatedIf.ReplaceNode(mutatedIf.GetCurrentNode(ifStatement.Statement), Mutate(ifStatement.Statement));
        }

        private SyntaxNode MutateForStatement(ForStatementSyntax forStatement)
        {
            // for needs special treatments for its incrementors
            StatementSyntax forWithMutantIncrementors = forStatement.TrackNodes(forStatement);

            foreach (var incrementor in forStatement.Incrementors)
            {
                forWithMutantIncrementors = MutateSubExpressionWithIfStatements(forStatement, forWithMutantIncrementors, incrementor);
            }

            var originalFor = forWithMutantIncrementors.GetCurrentNode(forStatement);
            // now we generate a mutant for the remainder of the for statement
            var mutatedFor = forStatement.TrackNodes(forStatement.Condition, forStatement.Statement);
            mutatedFor = mutatedFor.ReplaceNode(mutatedFor.GetCurrentNode(forStatement.Condition),
                Mutate(forStatement.Condition));
            mutatedFor = mutatedFor.ReplaceNode(mutatedFor.GetCurrentNode(forStatement.Statement),
                Mutate(forStatement.Statement));
            // and now we replace it
            return forWithMutantIncrementors.ReplaceNode(originalFor, mutatedFor);
        }

        private SyntaxNode MutateExpressions(SyntaxNode currentNode)
        {
            if (currentNode is ExpressionStatementSyntax expressionStatement)
            {
                // The subExpression of a ExpressionStatement cannot be mutated directly
                var mutant = Mutate(expressionStatement.Expression);
                return currentNode.ReplaceNode(expressionStatement.Expression, mutant);
            }
            var expressions = GetExpressionSyntax(currentNode).Where(x => x != null);
            if (expressions.Any())
            {
                var currentNodeCopy = currentNode.TrackNodes(expressions);
                foreach (var expressionSyntax in expressions)
                {
                    var currentExpressionSyntax = currentNodeCopy.GetCurrentNode(expressionSyntax);
                    if (expressionSyntax is InvocationExpressionSyntax )
                    {
                        // chained invocations, we will recurse
                        var mutant = Mutate(expressionSyntax);
                        currentNodeCopy = currentNodeCopy.ReplaceNode(currentExpressionSyntax, mutant);
                    } 
                    SyntaxNode mutationCandidate = null;
                    
                    if (expressionSyntax is ParenthesizedLambdaExpressionSyntax lambda)
                    {
                        mutationCandidate = lambda.Body;
                    }
                    else if (expressionSyntax is MemberAccessExpressionSyntax memberAccess)
                    {
                        mutationCandidate = memberAccess.Expression;
                    }
                    else if (expressionSyntax is AnonymousFunctionExpressionSyntax anonymousFunction)
                    {
                        mutationCandidate = anonymousFunction.Body;
                    }

                    if (mutationCandidate != null)
                    {
                        // we can  mutate a part of expression
                        var subNodeCopy = expressionSyntax.TrackNodes(mutationCandidate);
                        var sub = subNodeCopy.ReplaceNode(subNodeCopy.GetCurrentNode(mutationCandidate), Mutate(mutationCandidate));
                        currentNodeCopy = currentNodeCopy.ReplaceNode(currentExpressionSyntax, sub);
                    }
                    else
                    {
                        // attempts to mutate the expression as a whole
                        currentNodeCopy = currentNodeCopy.ReplaceNode(currentExpressionSyntax,
                            MutateWithConditionalExpressions(expressionSyntax));
                    }
                }

                return currentNodeCopy;
            }

            return null;
        }

        private IEnumerable<Mutant> FindMutantsRecursive(SyntaxNode current)
        {
            foreach (var mutant in FindMutants(current))
            {
                yield return mutant;
            }
            foreach (var mutant in current.ChildNodes().SelectMany(FindMutantsRecursive))
            {
                yield return mutant;
            }
        }

        private IEnumerable<Mutant> FindMutants(SyntaxNode current)
        {
            return _mutators.SelectMany(mutator => ApplyMutator(current, mutator));
        }

        private StatementSyntax MutateSubExpressionWithIfStatements(StatementSyntax originalNode, StatementSyntax nodeToReplace, SyntaxNode subExpression)
        {
            var ast = nodeToReplace;
            // The mutations should be placed using an IfStatement
            foreach (var mutant in FindMutants(subExpression))
            {
                _mutants.Add(mutant);
                var mutatedNode = ApplyMutant(originalNode, mutant);
                ast = MutantPlacer.PlaceWithIfStatement(ast, mutatedNode, mutant.Id);
            }
            return ast;
        }

        private SyntaxNode MutateWithConditionalExpressions(ExpressionSyntax currentNode)
        {
            var expressionAst = currentNode;
            foreach (var mutant in FindMutantsRecursive(currentNode))
            {
                _mutants.Add(mutant);
                var mutatedNode = ApplyMutant(currentNode, mutant);
                expressionAst = MutantPlacer.PlaceWithConditionalExpression(expressionAst, mutatedNode, mutant.Id);
            }
            return expressionAst;
        }

        /// <summary>
        /// Mutates one single SyntaxNode using a mutator
        /// </summary>
        private IEnumerable<Mutant> ApplyMutator(SyntaxNode syntaxNode, IMutator mutator)
        {
            var mutations = mutator.Mutate(syntaxNode);
            foreach (var mutation in mutations)
            {
                _logger.LogDebug("Mutant {0} created {1} -> {2} using {3}", _mutantCount, mutation.OriginalNode, mutation.ReplacementNode, mutator.GetType());
                yield return new Mutant()
                {
                    Id = _mutantCount++,
                    Mutation = mutation,
                    ResultStatus = MutantStatus.NotRun
                };
            }
        }

        private static T ApplyMutant<T>(T node, IReadOnlyMutant mutant) where T: SyntaxNode
        {
            return node.ReplaceNode(mutant.Mutation.OriginalNode, mutant.Mutation.ReplacementNode);
        }

        private IEnumerable<ExpressionSyntax> GetExpressionSyntax(SyntaxNode node)
        {
            switch (node.GetType().Name)
            {
                case nameof(AssignmentExpressionSyntax):
                    var assignementExpression = node as AssignmentExpressionSyntax;
                    yield return assignementExpression.Right;
                    yield break;
                case nameof(LocalDeclarationStatementSyntax):
                    var localDeclarationStatement = node as LocalDeclarationStatementSyntax;
                    foreach(var vars in localDeclarationStatement.Declaration.Variables.Select(x => x.Initializer?.Value))
                    {
                        yield return vars;
                    }
                    yield break;
                case nameof(ReturnStatementSyntax):
                    var returnStatement = node as ReturnStatementSyntax;
                    yield return returnStatement.Expression;
                    yield break;
                case nameof(LocalFunctionStatementSyntax):
                    var localFunction = node as LocalFunctionStatementSyntax;
                    yield return localFunction.ExpressionBody?.Expression;
                    yield break;
                case nameof(InvocationExpressionSyntax):
                    var invocationExpression = node as InvocationExpressionSyntax;
                    yield return invocationExpression.Expression;
                    foreach(var arg in invocationExpression.ArgumentList.Arguments.Select(x => x.Expression))
                    {
                        yield return arg;
                    }
                    yield break;
            }

            if (node is ExpressionSyntax expression)
            {
                yield return expression;
            }
        }
    }
}
