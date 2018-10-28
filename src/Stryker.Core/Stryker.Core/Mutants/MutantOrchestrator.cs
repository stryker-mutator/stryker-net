using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using System;
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
        public MutantOrchestrator(IEnumerable<IMutator> mutators)
        {
            _mutators = mutators;
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
            if (GetExpressionSyntax(currentNode) is var expressionSyntax && expressionSyntax != null) {
                // The mutations should be placed using a ConditionalExpression
                var expression = GetExpressionSyntax(currentNode);
                SyntaxNode ast = currentNode;
                foreach (var mutant in FindMutants(expression))
                {
                    _mutants.Add(mutant);
                    var mutatedNode = ApplyMutant(expression, mutant);
                    ast = ast.ReplaceNode(expression, MutantPlacer.PlaceWithConditionalExpression(expression, mutatedNode, mutant.Id));
                }
                return ast;
            }
            else if (currentNode is StatementSyntax && currentNode.Kind() != SyntaxKind.Block)
            {
                // The mutations should be placed using a IfStatement
                var statement = currentNode as StatementSyntax;
                StatementSyntax ast = statement as StatementSyntax;

                foreach (var mutant in currentNode.ChildNodes().SelectMany(FindMutants))
                {
                    _mutants.Add(mutant);
                    var mutatedNode = ApplyMutant(statement, mutant);
                    ast = MutantPlacer.PlaceWithIfStatement(ast, mutatedNode, mutant.Id);
                }
                return ast;
            }
            else
            {
                // No statement found yet, search deeper in the tree for statements to mutate
                var children = currentNode.ChildNodes().ToList();
                var mutatedChildren = currentNode.ChildNodes().Select(Mutate).ToList();
                var editor = new SyntaxEditor(currentNode, new AdhocWorkspace());
                for (int i = 0; i < children.Count; i++)
                {
                    var original = children[i];
                    var mutated = mutatedChildren[i];
                    if (!original.IsEquivalentTo(mutated))
                    {
                        editor.ReplaceNode(children[i], mutatedChildren[i]);
                    }
                }
                return editor.GetChangedRoot();
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

        private ExpressionSyntax GetExpressionSyntax(SyntaxNode node)
        {
            switch (node.GetType().Name)
            {
                case nameof(LocalDeclarationStatementSyntax):
                    var localDeclarationStatement = node as LocalDeclarationStatementSyntax;
                    return localDeclarationStatement.Declaration.Variables.First().Initializer.Value;
                case nameof(AssignmentExpressionSyntax):
                    var assignmentExpression = node as AssignmentExpressionSyntax;
                    return assignmentExpression.Right;
                case nameof(ReturnStatementSyntax):
                    var returnStatement = node as ReturnStatementSyntax;
                    return returnStatement.Expression;
                case nameof(LocalFunctionStatementSyntax):
                    var localFunction = node as LocalFunctionStatementSyntax;
                    return localFunction.ExpressionBody?.Expression;
                default:
                    return null;
            }
        }
    }
}
