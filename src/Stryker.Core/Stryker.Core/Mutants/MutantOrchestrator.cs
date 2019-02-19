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
            var expressions = GetExpressionSyntax(currentNode).Where(x => x != null);
            if (expressions.Any())
            {
                var currentNodeCopy = currentNode.TrackNodes(expressions);
                foreach (var expressionSyntax in expressions)
                {
                    var currentExpressionSyntax = currentNodeCopy.GetCurrentNode(expressionSyntax);
                    if (currentNode is ExpressionStatementSyntax expressionStatement)
                    {
                        if (GetExpressionSyntax(expressionStatement) is var subExpressionSyntax && subExpressionSyntax != null)
                        {
                            // The expression of a ExpressionStatement cannot be mutated directly
                            return currentNodeCopy.ReplaceNode(currentExpressionSyntax, Mutate(currentExpressionSyntax));
                        }
                        else
                        {
                            // If the EpxressionStatement does not contain a expression that can be mutated with conditional expression...
                            // it should be mutated with if statements
                            return MutateWithIfStatements(currentNode as ExpressionStatementSyntax);
                        }
                    }
                    else if (expressionSyntax is ParenthesizedLambdaExpressionSyntax lambda)
                    {
                        return currentNode.ReplaceNode(lambda.Body, Mutate(lambda.Body));
                    } else
                    {
                        // The mutations should be placed using a ConditionalExpression
                        currentNodeCopy = currentNodeCopy.ReplaceNode(currentExpressionSyntax, MutateWithConditionalExpressions(currentExpressionSyntax));
                    }
                }
                return currentNodeCopy;
            }

            if (currentNode is StatementSyntax statement && currentNode.Kind() != SyntaxKind.Block)
            {
                // Expression kinds that contain a body and can't have their scope changed should be mutated recursively
                if (currentNode is LocalFunctionStatementSyntax localFunction)
                {
                    return localFunction.ReplaceNode(localFunction.Body, Mutate(localFunction.Body));
                }
                if (currentNode is IfStatementSyntax ifStatement)
                {
                    ifStatement = ifStatement.ReplaceNode(ifStatement.Condition, MutateWithConditionalExpressions(ifStatement.Condition));
                    if (ifStatement.Else != null)
                    {
                        ifStatement = ifStatement.ReplaceNode(ifStatement.Else, Mutate(ifStatement.Else));
                    }
                    return ifStatement.ReplaceNode(ifStatement.Statement, Mutate(ifStatement.Statement));
                }
                return MutateWithIfStatements(statement);
            }
            else
            {
                // No statement found yet, search deeper in the tree for statements to mutate
                var children = currentNode.ChildNodes().ToList();
                var childCopy = currentNode.TrackNodes(children);
                foreach (var child in children)
                {
                    var mutatedNode = Mutate(child);
                    var originalNode = childCopy.GetCurrentNode(child);
                    if (!mutatedNode.IsEquivalentTo(originalNode))
                    {
                        childCopy = childCopy.ReplaceNode(originalNode, mutatedNode);
                    }
                }
                return childCopy;
            }
        }

        private IEnumerable<Mutant> FindMutants(SyntaxNode current)
        {
            foreach (var mutator in _mutators)
            {
                foreach (var mutation in ApplyMutator(current, mutator))
                {
                    yield return mutation;
                }
            }
            foreach (var mutant in current.ChildNodes().SelectMany(FindMutants))
            {
                yield return mutant;
            }
        }

        private SyntaxNode MutateWithIfStatements(StatementSyntax currentNode)
        {
            var ast = currentNode;
            StatementSyntax statement = currentNode as StatementSyntax;
            // The mutations should be placed using an IfStatement
            foreach (var mutant in currentNode.ChildNodes().SelectMany(FindMutants))
            {
                _mutants.Add(mutant);
                StatementSyntax mutatedNode = ApplyMutant(statement, mutant);
                ast = MutantPlacer.PlaceWithIfStatement(ast, mutatedNode, mutant.Id);
            }
            return ast;
        }

        private SyntaxNode MutateWithConditionalExpressions(ExpressionSyntax currentNode)
        {
            ExpressionSyntax expressionAst = currentNode;
            foreach (var mutant in FindMutants(currentNode))
            {
                _mutants.Add(mutant);
                ExpressionSyntax mutatedNode = ApplyMutant(currentNode, mutant);
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

        private T ApplyMutant<T>(T node, Mutant mutant) where T: SyntaxNode
        {
            var mutatedNode = node.ReplaceNode(mutant.Mutation.OriginalNode, mutant.Mutation.ReplacementNode);
            return mutatedNode;
        }

        private IEnumerable<ExpressionSyntax> GetExpressionSyntax(SyntaxNode node)
        {
            switch (node.GetType().Name)
            {
                case nameof(LocalDeclarationStatementSyntax):
                    var localDeclarationStatement = node as LocalDeclarationStatementSyntax;
                    foreach(var expression in localDeclarationStatement.Declaration.Variables.Select(x => x.Initializer?.Value))
                    {
                        yield return expression;
                    }
                    yield break;
                case nameof(AssignmentExpressionSyntax):
                    var assignmentExpression = node as AssignmentExpressionSyntax;
                    yield return assignmentExpression.Right;
                    yield break;
                case nameof(ReturnStatementSyntax):
                    var returnStatement = node as ReturnStatementSyntax;
                    yield return returnStatement.Expression;
                    yield break;
                case nameof(LocalFunctionStatementSyntax):
                    var localFunction = node as LocalFunctionStatementSyntax;
                    yield return localFunction.ExpressionBody?.Expression;
                    yield break;
                case nameof(ExpressionStatementSyntax):
                    var expressionStatement = node as ExpressionStatementSyntax;
                    yield return expressionStatement.Expression;
                    yield break;
                case nameof(InvocationExpressionSyntax):
                    var invocationExpression = node as InvocationExpressionSyntax;
                    foreach(var expression in invocationExpression.ArgumentList.Arguments.Select(x => x.Expression))
                    {
                        yield return expression;
                    }
                    yield break;
            }
        }
    }
}
