using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Helpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants.NodeOrchestrators;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Mutates abstract syntax trees using mutators and places all mutations inside the abstract syntax tree.
    /// Orchestrator: to arrange or manipulate, especially by means of clever or thorough planning or maneuvering.
    /// </summary>
    public class CsharpMutantOrchestrator : MutantOrchestrator<SyntaxNode>
    {
        private readonly TypeBasedStrategy<SyntaxNode, INodeMutator> _specificOrchestrator =
            new TypeBasedStrategy<SyntaxNode, INodeMutator>();

        public IEnumerable<IMutator> Mutators { get; }
        private ILogger Logger { get; }

        /// <param name="mutators">The mutators that should be active during the mutation process</param>
        public CsharpMutantOrchestrator(IEnumerable<IMutator> mutators = null, IStrykerOptions options = null) : base(options)
        {
            Mutators = mutators ?? new List<IMutator>
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
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<CsharpMutantOrchestrator>();

            _specificOrchestrator.RegisterHandlers(new List<INodeMutator>
            {
                new ForStatementOrchestrator(this),
                new AssignmentStatementOrchestrator(this),
                new PostfixUnaryExpressionOrchestrator(this),
                new StaticFieldDeclarationOrchestrator(this),
                new StaticConstructorOrchestrator(this),
                new PropertyDeclarationOrchestrator(this),
                new ArrayInitializerOrchestrator(this),
                new MemberDeclarationOrchestrator<MemberDeclarationSyntax, MemberDeclarationSyntax>(this),
                new BaseMethodDeclarationOrchestrator<BaseMethodDeclarationSyntax>(this),
                new AccessorSyntaxOrchestrator(this),
                new LocalDeclarationOrchestrator(this),
                new StatementSpecificOrchestrator<StatementSyntax>(this),
                new BlockOrchestrator(this),
                new ExpressionSpecificOrchestrator<ExpressionSyntax>(this),
                new SyntaxNodeOrchestrator(this)
            });
        }

        /// <summary>
        /// Recursively mutates a single SyntaxNode
        /// </summary>
        /// <param name="currentNode">The current root node</param>
        /// <returns>Mutated node</returns>
        public override SyntaxNode Mutate(SyntaxNode input)
        {
            var mutationContext = new MutationContext(this);
            var mutation = Mutate(input, mutationContext);
            return mutation;
        }

        // recursive version
        public SyntaxNode Mutate(SyntaxNode currentNode, MutationContext context)
        {
            // don't mutate immutable nodes
            if (!SyntaxHelper.CanBeMutated(currentNode))
            {
                return currentNode;
            }

            // search for node specific handler
            var nodeHandler = _specificOrchestrator.FindHandler(currentNode);
            return nodeHandler.Mutate(currentNode, context);
        }

        public IEnumerable<Mutant> GenerateMutationsForNode(SyntaxNode current, MutationContext context)
        {
            var mutations = new List<Mutant>();
            foreach (var mutator in Mutators)
            {
                foreach (var mutation in mutator.Mutate(current, _options))
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
                    mutations.Add(newMutant);
                }
            }

            return mutations;
        }
    }
}
