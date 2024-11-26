using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Abstractions.Options;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;
using Stryker.Core.Mutators;
using Stryker.Utilities.Helpers;

namespace Stryker.Core.Mutants;

/// <inheritdoc/>
public class CsharpMutantOrchestrator : BaseMutantOrchestrator<SyntaxTree, SemanticModel>
{
    private static readonly TypeBasedStrategy<SyntaxNode, INodeOrchestrator> specificOrchestrator =
        new();

    private ILogger Logger { get; }

    static CsharpMutantOrchestrator() =>
    // declare node specific orchestrators. Note that order is relevant, they should be declared from more specific to more generic one
        specificOrchestrator.RegisterHandlers(BuildOrchestratorList());

    /// <summary>
    /// <param name="mutators">The mutators that should be active during the mutation process</param>
    /// </summary>
    public CsharpMutantOrchestrator(MutantPlacer placer, IEnumerable<IMutator> mutators = null, IStrykerOptions options = null) : base(options)
    {
        Placer = placer;
        Mutators = mutators ?? DefaultMutatorList();
        Mutants = new Collection<IMutant>();
        Logger = ApplicationLogging.LoggerFactory.CreateLogger<CsharpMutantOrchestrator>();
    }

    private static List<INodeOrchestrator> BuildOrchestratorList() =>
    [
        new DoNotMutateOrchestrator<AttributeListSyntax>(),
        // parameter list
        new DoNotMutateOrchestrator<ParameterListSyntax>(),
        // enum values
        new DoNotMutateOrchestrator<EnumMemberDeclarationSyntax>(),
        // pattern marching
        new DoNotMutateOrchestrator<RecursivePatternSyntax>(),
        new DoNotMutateOrchestrator<UsingDirectiveSyntax>(),
        // constants and constant fields
        new DoNotMutateOrchestrator<FieldDeclarationSyntax>(
            t => t.Modifiers.Any(x => x.IsKind(SyntaxKind.ConstKeyword))),
        new DoNotMutateOrchestrator<LocalDeclarationStatementSyntax>(t => t.IsConst),
        // ensure pre/post increment/decrement mutations are mutated at statement level
        new MutateAtStatementLevelOrchestrator<PostfixUnaryExpressionSyntax>(t =>
            t.Parent is ExpressionStatementSyntax or ForStatementSyntax),
        new MutateAtStatementLevelOrchestrator<PrefixUnaryExpressionSyntax>(t =>
            t.Parent is ExpressionStatementSyntax or ForStatementSyntax),
        // prevent mutations to happen within member access expression
        new MemberAccessExpressionOrchestrator<MemberAccessExpressionSyntax>(),
        new MemberAccessExpressionOrchestrator<MemberBindingExpressionSyntax>(),
        new MemberAccessExpressionOrchestrator<SimpleNameSyntax>(),
        new MemberAccessExpressionOrchestrator<PostfixUnaryExpressionSyntax>(t =>
            t.IsKind(SyntaxKind.SuppressNullableWarningExpression)),
        new ConditionalExpressionOrchestrator(),
        new ConstantPatternSyntaxOrchestrator(),
        // ensure static constructs are marked properly
        new StaticFieldDeclarationOrchestrator(),
        new StaticConstructorOrchestrator(),
        // ensure array initializer mutations are controlled at statement level
        new MutateAtStatementLevelOrchestrator<InitializerExpressionSyntax>(t =>
            t.Kind() == SyntaxKind.ArrayInitializerExpression && t.Expressions.Count > 0),
        // ensure properties are properly mutated (including expression to body conversion if required)
        new ExpressionBodiedPropertyOrchestrator(),
        // ensure method, lambda... are properly mutated (including expression to body conversion if required)
        new LocalFunctionStatementOrchestrator(),
        new AnonymousFunctionExpressionOrchestrator(),
        new BaseMethodDeclarationOrchestrator<BaseMethodDeclarationSyntax>(),
        new AccessorSyntaxOrchestrator(),
        // ensure declaration are mutated at the block level
        new LocalDeclarationOrchestrator(),
        new InvocationExpressionOrchestrator(),
        new NodeSpecificOrchestrator<GlobalStatementSyntax, GlobalStatementSyntax>(),
        new MemberDefinitionOrchestrator<MemberDeclarationSyntax>(),
        new MutateAtStatementLevelOrchestrator<AssignmentExpressionSyntax>(),
        new BlockOrchestrator(),
        new StatementSpecificOrchestrator<StatementSyntax>(),
        new ExpressionSpecificOrchestrator<ExpressionSyntax>(),
        new SyntaxNodeOrchestrator()
    ];

    private static List<IMutator> DefaultMutatorList() =>
    [
        new BinaryExpressionMutator(),
        new BlockMutator(),
        new BooleanMutator(),
        new ConditionalExpressionMutator(),
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
        new NullCoalescingExpressionMutator(),
        new MathMutator(),
        new SwitchExpressionMutator(),
        new IsPatternExpressionMutator(),
        new StringMethodMutator(),
        new CollectionExpressionMutator()
    ];

    private IEnumerable<IMutator> Mutators { get; }

    public MutantPlacer Placer { get; }

    /// <summary>
    /// Recursively mutates a syntax tree
    /// </summary>
    /// <param name="input">The syntax tree to mutate</param>
    /// <param name="semanticModel">Associated semantic model</param>
    /// <returns>Mutated tree</returns>
    public override SyntaxTree Mutate(SyntaxTree input, SemanticModel semanticModel) =>
        // search for node specific handler
        input.WithRootAndOptions(GetHandler(input.GetRoot()).Mutate(input.GetRoot(), semanticModel, new MutationContext(this)), input.Options);

    internal INodeOrchestrator GetHandler(SyntaxNode currentNode) => specificOrchestrator.FindHandler(currentNode);

    internal IEnumerable<Mutant> GenerateMutationsForNode(SyntaxNode current, SemanticModel semanticModel, MutationContext context)
    {
        var mutations = new List<Mutant>();
        foreach (var mutator in Mutators)
        {
            foreach (var mutation in mutator.Mutate(current, semanticModel, Options))
            {
                var newMutant = CreateNewMutant(mutation, context);
                // Skip if the mutant is a duplicate
                if (IsMutantDuplicate(newMutant, mutation))
                {
                    continue;
                }
                newMutant.Id = GetNextId();
                Logger.LogDebug("Mutant {MutantId} created {OriginalNode} -> {ReplacementNode} using {Mutator}", newMutant.Id, mutation.OriginalNode,
                    mutation.ReplacementNode, mutator.GetType());
                Mutants.Add(newMutant);
                mutations.Add(newMutant);
            }
        }

        return mutations;
    }

    /// <summary>
    /// Creates a new mutant for the given mutation, mutator and context. Returns null if the mutant
    /// is a duplicate.
    /// </summary>
    private Mutant CreateNewMutant(Mutation mutation, MutationContext context)
    {
        var mutantIgnored = context.FilteredMutators?.Contains(mutation.Type) ?? false;
        return new Mutant
        {
            Mutation = mutation,
            ResultStatus = mutantIgnored ? MutantStatus.Ignored : MutantStatus.Pending,
            IsStaticValue = context.InStaticValue,
            ResultStatusReason = mutantIgnored ? context.FilterComment : null
        };
    }

    /// <summary>
    /// Returns true if the new mutant is a duplicate of a mutant already listed in Mutants.
    /// </summary>
    private bool IsMutantDuplicate(IReadOnlyMutant newMutant, Mutation mutation)
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
