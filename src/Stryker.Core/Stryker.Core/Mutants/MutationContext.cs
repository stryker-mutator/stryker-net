using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutants;

/// <summary>
/// Describe the (syntax tree) context during mutation and ensure proper mutation injection.
/// It has several responsibilities:
/// 1) It is in charge of storing mutations as they are generated, inject them at the appropriate syntax level.  
/// 2) it also tracks mutator disabled via comments and restores them at adequate times
/// </summary>
///
internal class MutationContext
{
    // main orchestrator
    // the orchestrator is used to perform actual mutation injections
    private readonly CsharpMutantOrchestrator _mainOrchestrator;
    // pending mutation stacks. An entry is pushed in the stack when entering a member or function and popping it when leaving
    private readonly MutationStore _mutation;

    /// <summary>
    /// Mutation context must be created once when starting a mutation process.
    /// </summary>
    /// <param name="mutantOrchestrator"></param>
    public MutationContext(CsharpMutantOrchestrator mutantOrchestrator)
    {
        _mainOrchestrator = mutantOrchestrator;
        _mutation = new MutationStore(mutantOrchestrator.Placer);
    }

    private MutationContext(MutationContext parent)
    {
        _mainOrchestrator = parent._mainOrchestrator;
        InStaticValue = parent.InStaticValue;
        _mutation = parent._mutation;
        FilteredMutators = parent.FilteredMutators;
        FilterComment = parent.FilterComment;
    }

    /// <summary>
    ///  True when inside a static initializer, fields or accessor.
    /// </summary>
    public bool InStaticValue { get; private init; }

    /// <summary>
    /// True if orchestrator have to inject static usage tracing
    /// </summary>
    public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

    internal Mutator[] FilteredMutators { get; private set; }

    internal string FilterComment { get; set; }

    /// <summary>
    /// true if there are pending statement or block level mutations
    /// </summary>
    public bool HasLeftOverMutations => _mutation.HasPendingMutations();

    /// <summary>
    /// Call this to generate mutations using active mutators.
    /// </summary>
    /// <param name="node"><see cref="SyntaxNode"/> to mutate.</param>
    /// <param name="semanticModel">current semantic model</param>
    /// <returns>A list of mutants.</returns>
    public IEnumerable<Mutant> GenerateMutantsForNode(SyntaxNode node, SemanticModel semanticModel) =>
        _mainOrchestrator.GenerateMutationsForNode(node, semanticModel, this);

    /// <summary>
    /// Find the appropriate node handler for the given node.
    /// </summary>
    /// <param name="node">handler for which to find an orchestrator.</param>
    /// <returns>A handler for this node.</returns>
    public INodeOrchestrator FindHandler(SyntaxNode node) => _mainOrchestrator.GetHandler(node);

    /// <summary>
    /// Call this to signal mutation occurs in static method or fields
    /// </summary>
    /// <returns>A new context</returns>
    public MutationContext EnterStatic() => new(this) { InStaticValue = true };

    /// <summary>
    /// Call this when beginning of a syntax structure that can control mutations (expression, statement, block)
    /// </summary>
    /// <param name="control">type of structure (see <see cref="MutationControl"/>)</param>
    /// <returns>The context to use moving forward.</returns>
    /// <remarks>You must use <see cref="Leave()"/>when leaving the context.</remarks>
    public MutationContext Enter(MutationControl control)
    {
        _mutation.Enter(control);
        return control is MutationControl.Block or MutationControl.Member ? new MutationContext(this): this;
    }

    /// <summary>
    /// Call this when leaving a control syntax structure
    /// </summary>
    /// <remarks>A call must match a previous call to <see cref="Enter(MutationControl)"/></remarks>
    public MutationContext Leave()
    {
        _mutation.Leave();
        return this;
    }

    /// <summary>
    /// Register mutations
    /// </summary>
    /// <param name="mutants"></param>
    public MutationContext AddMutations(IEnumerable<Mutant> mutants)
    {
        _mutation.StoreMutations(mutants);
        return this;
    }

    /// <summary>
    /// Register new block level mutations
    /// </summary>
    /// <param name="mutants"></param>
    /// <param name="control"></param>
    public MutationContext AddMutations(IEnumerable<Mutant> mutants, MutationControl control)
    {
        _mutation.StoreMutationsAtDesiredLevel(mutants, control);
        return this;
    }

    /// <summary>
    /// Inject a static context marker in the given block
    /// </summary>
    /// <param name="block">Block in which inserts the marker</param>
    /// <returns>the updated block</returns>
    public BlockSyntax PlaceStaticContextMarker(BlockSyntax block) =>
        _mainOrchestrator.Placer.PlaceStaticContextMarker(block);

    /// <summary>
    /// Inject a static context marker in the given expression
    /// </summary>
    /// <param name="expression">expression in which inserts the marker</param>
    /// <returns>the updated expression</returns>
    public ExpressionSyntax PlaceStaticContextMarker(ExpressionSyntax expression) =>
        _mainOrchestrator.Placer.PlaceStaticContextMarker(expression);

    /// <summary>
    /// Injects pending expression level mutations.
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="sourceNode">Source node, used to generate mutations</param>
    /// <returns>A mutated node containing the mutations.</returns>
    /// <remarks>Do not inject mutation(s) if in a subexpression</remarks>
    public ExpressionSyntax InjectMutations(ExpressionSyntax mutatedNode, ExpressionSyntax sourceNode) => _mutation.Inject(mutatedNode, sourceNode);

    /// <summary>
    /// Injects pending statement level mutations.
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="sourceNode">Source node, used to generate mutations</param>
    /// <returns>A mutated node containing the mutations.</returns>
    public StatementSyntax InjectMutations(StatementSyntax mutatedNode, StatementSyntax sourceNode)
        => _mutation.Inject(mutatedNode, sourceNode);

    /// <summary>
    /// Injects pending block level mutations.
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="sourceNode">Source node, used to generate mutations</param>
    /// <returns>A mutated node containing the mutations.</returns>
    public BlockSyntax InjectMutations(BlockSyntax mutatedNode, BlockSyntax sourceNode) =>
        _mutation.Inject(mutatedNode, sourceNode);

    /// <summary>s
    /// Injects pending block level mutations for expression body method or functions
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="originalNode">Source node, used to generate mutations</param>
    /// <param name="needReturn">Set to true if the method has a return value. Expressions are transformed to return statement.</param>
    /// <returns>A mutated node containing the mutations.</returns>
    public BlockSyntax InjectMutations(BlockSyntax mutatedNode,
        ExpressionSyntax originalNode, bool needReturn) =>
        _mutation.Inject(mutatedNode, originalNode, needReturn);

    /// <summary>
    /// Enable/Disable a list of mutators.
    /// </summary>
    /// <param name="mode">true to disable mutators, false to (re)enable some</param>
    /// <param name="filteredMutators">list of mutators</param>
    /// <param name="newContext">true to create a new context</param>
    /// <param name="comment">comment used for ignored mutators</param>
    /// <returns>a context with an updated list of active mutators</returns>
    public MutationContext FilterMutators(bool mode, Mutator[] filteredMutators, bool newContext, string comment)
    {
        var result = newContext ? new MutationContext(this) : this;
        if (mode)
        {
            result.FilteredMutators = filteredMutators;
        }
        else if (result.FilteredMutators is not null)
        {
            result.FilteredMutators = result.FilteredMutators.Where(t => !filteredMutators.Contains(t)).ToArray();
        }

        if (mode)
        {
            result.FilterComment = comment;
        }

        return result;
    }
}
