using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
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
        /// <param name="mutantFactory">An instance of the mutantFactory, use the same for every file to keep the mutation count increment</param>
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
            if (currentNode is StatementSyntax && currentNode.Kind() != SyntaxKind.Block)
            {
                // This is a statement, meaning that we can insert mutants here
                var statement = currentNode as StatementSyntax;
                StatementSyntax ast = statement as StatementSyntax;

                // this is a temporary fix for mutating LocalDeclarationStatements because mutating withing these statements breaks the application
                // TODO: backlog item Ternary assignment statements mutations
                if (!(statement is LocalDeclarationStatementSyntax))
                {
                    foreach (var mutant in currentNode.ChildNodes().SelectMany(FindMutants))
                    {
                        _mutants.Add(mutant);
                        ast = MutantIf(ast, ApplyMutant(statement, mutant), mutant.Id);
                    }
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
                    if (!children[i].IsEquivalentTo(mutatedChildren[i]))
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

        private StatementSyntax ApplyMutant(StatementSyntax statement, Mutant mutant)
        {
            var editor = new SyntaxEditor(statement, new AdhocWorkspace());
            editor.ReplaceNode(mutant.Mutation.OriginalNode, mutant.Mutation.ReplacementNode);
            return editor.GetChangedRoot() as StatementSyntax;
        }

        /// <summary>
        /// Places an IfStatementSyntax node around the mutated node and places the original node in the ElseClause block.
        /// </summary>
        private IfStatementSyntax MutantIf(StatementSyntax original, StatementSyntax mutated, int mutantId)
        {
            // place the mutated statement inside the if statement
            IfStatementSyntax mutantIf = SyntaxFactory.IfStatement(
                condition: SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression,
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("System"),
                            SyntaxFactory.IdentifierName("Environment")
                        ),
                        SyntaxFactory.IdentifierName("GetEnvironmentVariable")),
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                        new List<ArgumentSyntax>() {
                        SyntaxFactory.Argument(SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal("ActiveMutation"))).Expression)
                        }
                    ))
                ),
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(mutantId.ToString()))),
                statement: SyntaxFactory.Block(mutated),
                @else: SyntaxFactory.ElseClause(SyntaxFactory.Block(original)))
                // Mark this node as a MutantIf node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation("MutantIf", mutantId.ToString()));

            return mutantIf;
        }
    }
}
