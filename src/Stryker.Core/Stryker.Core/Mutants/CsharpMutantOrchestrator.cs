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

namespace Stryker.Core.Mutants;

/// <inheritdoc/>
public class CsharpMutantOrchestrator : BaseMutantOrchestrator<SyntaxTree, SemanticModel>
{
    private readonly TypeBasedStrategy<SyntaxNode, INodeOrchestrator> _specificOrchestrator =
        new();

    private ILogger Logger { get; }

    /// <summary>
    /// <param name="mutators">The mutators that should be active during the mutation process</param>
    /// </summary>
    public CsharpMutantOrchestrator(MutantPlacer placer, IEnumerable<IMutator> mutators = null, StrykerOptions options = null) : base(options)
    {
        Placer = placer;
        Mutators = mutators ?? DefaultMutatorList();
        Mutants = new Collection<Mutant>();
        Logger = ApplicationLogging.LoggerFactory.CreateLogger<CsharpMutantOrchestrator>();

        // declare node specific orchestrators. Note that order is relevant, they should be declared from more specific to more generic one
        _specificOrchestrator.RegisterHandlers(BuildOrchestratorList());
    }

    private static List<INodeOrchestrator> BuildOrchestratorList() =>
        new()
        {
            // Those node types describe compile time constants and thus cannot be mutated at run time
            // attributes
            new DoNotMutateOrchestrator<AttributeListSyntax>(),
            // parameter list
            new DoNotMutateOrchestrator<ParameterListSyntax>(),
            // enum values
            new DoNotMutateOrchestrator<EnumMemberDeclarationSyntax>(),
            // pattern marching
            new DoNotMutateOrchestrator<RecursivePatternSyntax>(),
            new DoNotMutateOrchestrator<UsingDirectiveSyntax>(),
            // constants and constant fields
            new DoNotMutateOrchestrator<FieldDeclarationSyntax>(t => t.Modifiers.Any(x => x.IsKind(SyntaxKind.ConstKeyword))),
            new DoNotMutateOrchestrator<LocalDeclarationStatementSyntax>(t => t.IsConst),
            // ensure pre/post increment/decrement mutations are mutated at statement level
            new MutateAtStatementLevelOrchestrator<PostfixUnaryExpressionSyntax>( t => t.Parent is ExpressionStatementSyntax or ForStatementSyntax),
            new MutateAtStatementLevelOrchestrator<PrefixUnaryExpressionSyntax>( t => t.Parent is ExpressionStatementSyntax or ForStatementSyntax),
            // prevent mutations to happen within member access expression
            new MemberAccessExpressionOrchestrator<MemberAccessExpressionSyntax>(),
            new MemberAccessExpressionOrchestrator<MemberBindingExpressionSyntax>(),
            new MemberAccessExpressionOrchestrator<SimpleNameSyntax>(),
            // ensure static constructs are marked properly
            new StaticFieldDeclarationOrchestrator(),
            new StaticConstructorOrchestrator(),
            // ensure array initializer mutations are controlled at statement level
            new MutateAtStatementLevelOrchestrator<InitializerExpressionSyntax>( t => t.Kind() == SyntaxKind.ArrayInitializerExpression && t.Expressions.Count > 0),
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
            new MemberDefinitionOrchestrator<MemberDeclarationSyntax>(),
            new MutateAtStatementLevelOrchestrator<AssignmentExpressionSyntax>(),
            new BlockOrchestrator(),
            new StatementSpecificOrchestrator<StatementSyntax>(),
            new ExpressionSpecificOrchestrator<ExpressionSyntax>(),
            new SyntaxNodeOrchestrator()
        };

    private static List<IMutator> DefaultMutatorList() =>
        new()
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

    internal INodeOrchestrator GetHandler(SyntaxNode currentNode) => _specificOrchestrator.FindHandler(currentNode);

    internal IEnumerable<Mutant> GenerateMutationsForNode(SyntaxNode current, SemanticModel semanticModel, MutationContext context)
    {
        var mutations = new List<Mutant>();
        foreach (var mutator in Mutators)
        {
            foreach (var mutation in mutator.Mutate(current, semanticModel, Options))
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
