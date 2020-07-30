using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private IEnumerable<IMutator> Mutators { get; }
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
                new ExpressionStatementOrchestrator(),
                new StaticFieldDeclarationOrchestrator(),
                new StaticConstructorOrchestrator(),
                new StaticPropertyOrchestrator(),
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

        internal SyntaxNode MutateExpression(SyntaxNode currentNode, MutationContext context)
        {
            // Nothing to mutate, dig further
            var childCopy = currentNode.TrackNodes(currentNode.ChildNodes().ToList().Append(currentNode));
            var mutated = false;

            foreach (var child in currentNode.ChildNodes().ToList())
            {
                var mutatedChild = Mutate(child, context);
                if (child != mutatedChild)
                {
                    var currentChild = childCopy.GetCurrentNode(child);
                    childCopy = childCopy.ReplaceNode(currentChild, mutatedChild);
                    mutated = true;
                }
            }

            return mutated ? childCopy : currentNode;
        }

        private IEnumerable<Mutant> FindMutants(SyntaxNode current, MutationContext context)
        {
            return Mutators.SelectMany(mutator => ApplyMutator(current, mutator, context));
        }

        internal StatementSyntax MutateSubExpressionWithIfStatements(StatementSyntax originalNode,
            StatementSyntax nodeToReplace, SyntaxNode subExpression, MutationContext context)
        {
            // The mutations should be placed using an IfStatement

            return FindMutants(subExpression, context).Aggregate(nodeToReplace, (current, mutant) => MutantPlacer.PlaceWithIfStatement(current, ApplyMutant(originalNode, mutant), mutant.Id));
        }

        internal ExpressionSyntax MutateSubExpressionWithConditional(ExpressionSyntax originalNode,
            ExpressionSyntax currentNode, MutationContext context)
        {
            return FindMutants(originalNode, context).Aggregate(currentNode,
                (current, mutant) =>
                    MutantPlacer.PlaceWithConditionalExpression(current, ApplyMutant(originalNode, mutant), mutant.Id));
        }

        /// <summary>
        /// Mutates one single SyntaxNode using a mutator
        /// </summary>
        private IEnumerable<Mutant> ApplyMutator(SyntaxNode syntaxNode, IMutator mutator, MutationContext context)
        {
            var mutations = mutator.Mutate(syntaxNode);
            foreach (var mutation in mutations)
            {
                Logger.LogDebug("Mutant {0} created {1} -> {2} using {3}", MutantCount, mutation.OriginalNode,
                    mutation.ReplacementNode, mutator.GetType());
                yield return new Mutant()
                {
                    Id = MutantCount++,
                    Mutation = mutation,
                    ResultStatus = MutantStatus.NotRun,
                    IsStaticValue = context.InStaticValue
                };
            }
        }

        private T ApplyMutant<T>(T node, Mutant mutant) where T : SyntaxNode
        {
            Mutants.Add(mutant);
            return node.ReplaceNode(mutant.Mutation.OriginalNode, mutant.Mutation.ReplacementNode);
        }
    }
}