using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System.Linq;
using Stryker.Core.Helpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants.NodeOrchestrators;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            new();

        public IEnumerable<IMutator> Mutators { get; }
        private ILogger Logger { get; }

        /// <param name="mutators">The mutators that should be active during the mutation process</param>
        public CsharpMutantOrchestrator(IEnumerable<IMutator> mutators = null, StrykerOptions options = null) : base(options)
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
                new StatementMutator(),
                new RegexMutator()
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
                    (t) => t.Modifiers.Any(x => x.Kind() == SyntaxKind.ConstKeyword)),
                new AssignmentStatementOrchestrator(),
                new PostfixUnaryExpressionOrchestrator(),
                new StaticFieldDeclarationOrchestrator(),
                new StaticConstructorOrchestrator(),
                new PropertyDeclarationOrchestrator(),
                new ArrayInitializerOrchestrator(),
                new BaseMethodDeclarationOrchestrator<BaseMethodDeclarationSyntax>(),
                new AccessorSyntaxOrchestrator(),
                new LocalDeclarationOrchestrator(),
                new StatementSpecificOrchestrator<StatementSyntax>(),
                new BlockOrchestrator(),
                new ExpressionSpecificOrchestrator<ExpressionSyntax>(),
                new SyntaxNodeOrchestrator()
            });
        }

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
                    var id = MutantCount;
                    Logger.LogDebug("Mutant {0} created {1} -> {2} using {3}", id, mutation.OriginalNode,
                        mutation.ReplacementNode, mutator.GetType());
                    var mutantIgnored = context.FilteredMutators?.Contains(mutation.Type) ?? false;
                    var newMutant = new Mutant
                    {
                        Id = id,
                        Mutation = mutation,
                        ResultStatus = mutantIgnored ? MutantStatus.Ignored : MutantStatus.NotRun,
                        IsStaticValue = context.InStaticValue,
                        ResultStatusReason = mutantIgnored ? context.FilterComment : "not run"
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
