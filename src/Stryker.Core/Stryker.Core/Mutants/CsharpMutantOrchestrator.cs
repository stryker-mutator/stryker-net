using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Helpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Stryker.Core.Mutants
{
    /// <inheritdoc/>
    public class CsharpMutantOrchestrator : BaseMutantOrchestrator<SyntaxNode>
    {
        private readonly TypeBasedStrategy<SyntaxNode, INodeMutator> _specificOrchestrator =
            new();

        private ILogger Logger { get; }

        /// <summary>
        /// <param name="mutators">The mutators that should be active during the mutation process</param>
        /// </summary>
        public CsharpMutantOrchestrator(MutantPlacer placer, IEnumerable<IMutator> mutators = null, StrykerOptions options = null) : base(options)
        {
            Placer = placer;
            Mutators = mutators ?? new List<IMutator>
            {
                // the default list of mutators
                new BinaryExpressionMutator(),
                new BlockMutator(),
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
                new StatementMutator(),
                new RegexMutator(),
                new NullCoalescingExpressionMutator(),
                new MathMutator(),
                new SwitchExpressionMutator(),
                new IsPatternExpressionMutator()
            };
            Mutants = new Collection<Mutant>();
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<CsharpMutantOrchestrator>();

            _specificOrchestrator.RegisterHandlers(new List<INodeMutator>
            {
                new DontMutateOrchestrator<AttributeListSyntax>(),
                new DontMutateOrchestrator<ParameterListSyntax>(),
                new DontMutateOrchestrator<EnumMemberDeclarationSyntax>(),
                new DontMutateOrchestrator<RecursivePatternSyntax>(),
                new DontMutateOrchestrator<FieldDeclarationSyntax>(
                    (t) => t.Modifiers.Any(x => x.IsKind(SyntaxKind.ConstKeyword))),
                new AssignmentStatementOrchestrator(),
                new PostfixUnaryExpressionOrchestrator(),
                new PrefixUnaryExpressionOrchestrator(),
                new StaticFieldDeclarationOrchestrator(),
                new StaticConstructorOrchestrator(),
                new PropertyDeclarationOrchestrator(),
                new ArrayInitializerOrchestrator(),
                new LocalFunctionStatementOrchestrator(),
                new AnonymousFunctionExpressionOrchestrator(),
                new BaseMethodDeclarationOrchestrator<BaseMethodDeclarationSyntax>(),
                new AccessorSyntaxOrchestrator(),
                new LocalDeclarationOrchestrator(),
                new StatementSpecificOrchestrator<StatementSyntax>(),
                new BlockOrchestrator(),
                new ExpressionSpecificOrchestrator<ExpressionSyntax>(),
                new SyntaxNodeOrchestrator()
            });
        }

        public IEnumerable<IMutator> Mutators { get; }
        public MutantPlacer Placer { get; }

        /// <summary>
        /// Recursively mutates a single SyntaxNode
        /// </summary>
        /// <param name="input">The current root node</param>
        /// <returns>Mutated node</returns>
        public override SyntaxNode Mutate(SyntaxNode input) =>
            // search for node specific handler
            GetHandler(input).Mutate(input, new MutationContext(this));

        internal INodeMutator GetHandler(SyntaxNode currentNode) => _specificOrchestrator.FindHandler(currentNode);

        internal IEnumerable<Mutant> GenerateMutationsForNode(SyntaxNode current, MutationContext context)
        {
            var mutations = new List<Mutant>();
            foreach (var mutator in Mutators)
            {
                foreach (var mutation in mutator.Mutate(current, _options))
                {
                    var newMutant = CreateNewMutant(mutation, mutator, context);

                    // Skip if the mutant is a duplicate
                    if (IsMutantDuplicate(newMutant, mutation))
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

        /// <summary>
        /// Creates a new mutant for the given mutation, mutator and context. Returns null if the mutant
        /// is a duplicate.
        /// </summary>
        private Mutant CreateNewMutant(Mutation mutation, IMutator mutator, MutationContext context)
        {
            var id = MutantCount;
            Logger.LogDebug("Mutant {0} created {1} -> {2} using {3}", id, mutation.OriginalNode,
                mutation.ReplacementNode, mutator.GetType());
            var mutantIgnored = context.FilteredMutators?.Contains(mutation.Type) ?? false;
            return new Mutant
            {
                Id = id,
                Mutation = mutation,
                ResultStatus = mutantIgnored ? MutantStatus.Ignored : MutantStatus.Pending,
                IsStaticValue = context.InStaticValue,
                ResultStatusReason = mutantIgnored ? context.FilterComment : null
            };
        }

        /// <summary>
        /// Returns true if the new mutant is a duplicate of a mutant already listed in Mutants.
        /// </summary>
        private bool IsMutantDuplicate(Mutant newMutant, Mutation mutation)
        {
            foreach (var mutant in Mutants)
            {
                if (mutant.Mutation.OriginalNode != mutation.OriginalNode ||
                    !SyntaxFactory.AreEquivalent(mutant.Mutation.ReplacementNode, newMutant.Mutation.ReplacementNode))
                {
                    continue;
                }
                Logger.LogDebug("Mutant {newMutant} discarded as it is a duplicate of {mutant}", newMutant.Id, mutant.Id);
                return true;
            }
            return false;
        }
    }
}
