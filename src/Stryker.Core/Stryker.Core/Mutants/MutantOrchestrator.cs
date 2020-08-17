using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Helpers;
using Stryker.Core.Mutants.NodeOrchestrator;

namespace Stryker.Core.Mutants
{
    public interface IMutantOrchestrator
    {
        SyntaxNode Mutate(SyntaxNode rootNode);

        /// <summary>
        /// Gets the stored mutants and resets the mutant list to an empty collection
        /// </summary>
        /// <returns>Mutants</returns>
        IReadOnlyCollection<Mutant> GetLatestMutantBatch();
    }

    /// <summary>
    /// Mutates abstract syntax trees using mutators and places all mutations inside the abstract syntax tree.
    /// Orchestrator: to arrange or manipulate, especially by means of clever or thorough planning or maneuvering.
    /// </summary>
    public class MutantOrchestrator : IMutantOrchestrator
    {
        private readonly StrykerOptions _options;

        private readonly TypeBasedStrategy<SyntaxNode, INodeMutator> _specificOrchestrator =
            new TypeBasedStrategy<SyntaxNode, INodeMutator>();

        private ICollection<Mutant> Mutants { get; set; }
        private int MutantCount { get; set; }
        internal IEnumerable<IMutator> Mutators { get; }
        private ILogger Logger { get; }

        internal bool MustInjectCoverageLogic =>
            _options != null && _options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest) &&
            !_options.Optimizations.HasFlag(OptimizationFlags.CaptureCoveragePerTest);

        /// <param name="mutators">The mutators that should be active during the mutation process</param>
        public MutantOrchestrator(IEnumerable<IMutator> mutators = null, StrykerOptions options = null)
        {
            _options = options;
            Mutators = mutators ?? new List<IMutator>()
            {
                // the default list of mutators
                new BinaryExpressionMutator(),
                new BooleanMutator(),
                new AssignmentExpressionMutator(),
                new PrefixUnaryMutator(),
                new PostfixUnaryMutator(),
                new CheckedMutator(),
                new LinqMutator(),
                new StringMutator(),
                new StringEmptyMutator(),
                new InterpolatedStringMutator(),
                new NegateConditionMutator(),
                new InitializerMutator(),
                new ObjectCreationMutator(),
                new ArrayCreationMutator(),
                new RegexMutator()
            };
            Mutants = new Collection<Mutant>();
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutantOrchestrator>();

            _specificOrchestrator.RegisterHandlers(new List<INodeMutator>
            {
                new IfStatementOrchestrator(),
                new ForStatementOrchestrator(),
                new AssignmentStatementOrchestrator(),
                new PostfixUnaryExpressionOrchestrator(),
                new StaticFieldDeclarationOrchestrator(),
                new StaticConstructorOrchestrator(),
                new StaticPropertyOrchestrator(),
                new ArrayInitializerOrchestrator(),
                new ExpressionSyntaxOrchestrator(),
                new MethodDeclarationOrchestrator(),
                new SyntaxNodeOrchestrator()
            });
        }

        /// <summary>
        /// Gets the stored mutants and resets the mutant list to an empty collection
        /// </summary>
        /// <returns>Mutants</returns>
        public IReadOnlyCollection<Mutant> GetLatestMutantBatch()
        {
            var tempMutants = Mutants;
            Mutants = new Collection<Mutant>();
            return (IReadOnlyCollection<Mutant>) tempMutants;
        }

        /// <summary>
        /// Recursively mutates a single SyntaxNode
        /// </summary>
        /// <param name="currentNode">The current root node</param>
        /// <returns>Mutated node</returns>
        public SyntaxNode Mutate(SyntaxNode currentNode)
        {
            return Mutate(currentNode, new MutationContext(this));
        }

        // recursive version
        internal SyntaxNode Mutate(SyntaxNode currentNode, MutationContext context)
        {
            // don't mutate immutable nodes
            if (!SyntaxHelper.CanBeMutated(currentNode))
            {
                return currentNode;
            }

            // search for node specific handler
            var result = this._specificOrchestrator.FindHandler(currentNode);
            return result.Mutate(currentNode, context);
        }

        internal IEnumerable<Mutant> GenerateMutantsForNode(SyntaxNode current, MutationContext context)
        {
            foreach (var mutator in Mutators)
            {
                foreach (var mutation in mutator.Mutate(current))
                {
                    var id = MutantCount;
                    Logger.LogDebug("Mutant {0} created {1} -> {2} using {3}", id, mutation.OriginalNode,
                        mutation.ReplacementNode, mutator.GetType());
                    var newMutant = new Mutant
                    {
                        Id = id,
                        Mutation = mutation,
                        ResultStatus = MutantStatus.NotRun,
                        IsStaticValue = context.InStaticValue
                    };
                    var duplicate = false;
                    // check if we have a duplicate
                    foreach (var mutant in Mutants)
                    {
                        if (mutant.Mutation.OriginalNode != mutation.OriginalNode ||
                            !SyntaxFactory.AreEquivalent(mutant.Mutation.ReplacementNode, newMutant.Mutation.ReplacementNode))
                        {
                            continue;
                        }
                        Logger.LogDebug($"Mutant {id} discarded as it is a duplicate of {mutant.Id}");
                        duplicate = true;
                        break;
                    }

                    if (duplicate)
                    {
                        continue;
                    }

                    Mutants.Add(newMutant);
                    MutantCount++;
                    yield return newMutant;
                }
            }
        }

        internal StatementSyntax PlaceMutantWithinIfControls(in StatementSyntax node, in StatementSyntax mutated, IEnumerable<Mutant> mutationsControlledByIfs)
        {
            var syntax = node;
            return mutationsControlledByIfs.Aggregate(mutated, (current, mutant) => MutantPlacer.PlaceWithIfStatement(current, InjectMutation(syntax, mutant), mutant.Id));
        }

        internal ExpressionSyntax PlaceMutantWithinConditionalControls(in ExpressionSyntax node, in ExpressionSyntax mutated, IEnumerable<Mutant> expressionMutations)
        {
            var syntax = node;
            return expressionMutations.Aggregate(mutated, (current, mutant) => MutantPlacer.PlaceWithConditionalExpression(current, InjectMutation(syntax, mutant), mutant.Id));
        }

        // inject the mutation within the control structure
        private T InjectMutation<T>(in T node, Mutant mutant) where T : SyntaxNode
        {
            return node.ReplaceNode(mutant.Mutation.OriginalNode, mutant.Mutation.ReplacementNode);
        }

    }
}