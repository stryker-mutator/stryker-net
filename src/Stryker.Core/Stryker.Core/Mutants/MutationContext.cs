using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutants;

/// <summary>
/// Describe the (syntax tree) context during mutation and ensure proper mutation injection.
/// It has several responsibilities:
/// 1) It is in charge of storing mutations as they are generated, inject them at the appropriate syntax level.  
/// 2) it also tracks mutator disabled via comments and restores them at adequate times
/// </summary>
internal class MutationContext
{
    // main orchestrator
    // the orchestrator is used to perform actual mutation injections
    private readonly CsharpMutantOrchestrator _mainOrchestrator;
    // pending mutation stacks. An entry is pushed in the stack when entering a member or function and popping it when leaving
    private readonly Stack<MutationStore> _pendingMutations = new();

    /// <summary>
    /// Mutation context must be created once when starting a mutation process.
    /// </summary>
    /// <param name="mutantOrchestrator"></param>
    public MutationContext(CsharpMutantOrchestrator mutantOrchestrator)
    {
        _mainOrchestrator = mutantOrchestrator;
        Enter(MutationControl.Member);
    }

    private MutationContext(MutationContext parent)
    {
        _mainOrchestrator = parent._mainOrchestrator;
        InStaticValue = parent.InStaticValue;
        _pendingMutations = parent._pendingMutations;
        FilteredMutators = parent.FilteredMutators;
        FilterComment = parent.FilterComment;
    }

    /// <summary>
    ///  True when inside a static initializer, fields or accessor.
    /// </summary>
    public bool InStaticValue { get; set; }

    /// <summary>
    /// True if orchestrator have to inject static usage tracing
    /// </summary>
    public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

    internal Mutator[] FilteredMutators { get; private set; }

    internal string FilterComment { get; set; }

    private MutationStore CurrentStore => _pendingMutations.Peek();

    /// <summary>
    /// true if there are pending statement or block level mutations
    /// </summary>
    public bool HasStatementLevelMutant => CurrentStore.HasStatementLevel;

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
    /// <returns>An handler for this node.</returns>
    public INodeMutator FindHandler(SyntaxNode node) => _mainOrchestrator.GetHandler(node);

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
    /// <remarks>You must use a <see cref="Leave(MutationControl)"/>when leaving the context.</remarks>
    public MutationContext Enter(MutationControl control)
    {
        switch (control)
        {
            case MutationControl.MemberAccess:
                CurrentStore.MemberAccessLength++;
                return this;
            case MutationControl.Statement:
                CurrentStore.EnterStatement();
                return this;
            case MutationControl.Block:
                CurrentStore.EnterBlock();
                return new MutationContext(this);
            case MutationControl.Member:
                _pendingMutations.Push(new MutationStore(_mainOrchestrator.Placer));
                CurrentStore.EnterBlock();
                break;
        }

        return this;
    }

    /// <summary>
    /// Call this when leaving a control syntax structure
    /// </summary>
    /// <param name="control">type of structure (see <see cref="MutationControl"/>)</param>
    /// <remarks>A call must match a previous call to <see cref="Enter(MutationControl)"/></remarks>
    public MutationContext Leave(MutationControl control)
    {
        switch (control)
        {
            case MutationControl.MemberAccess:
                CurrentStore.MemberAccessLength--;
                break;
            case MutationControl.Statement:
                CurrentStore.LeaveStatement();
                break;
            case MutationControl.Block:
                CurrentStore.LeaveBlock();
                break;
            case MutationControl.Member:
                CurrentStore.LeaveBlock();
                _pendingMutations.Pop();
                break;
        }
        return this;
    }

    /// <summary>
    /// Register new statement level mutations
    /// </summary>
    /// <param name="mutants"></param>
    public void AddExpressionLevel(IEnumerable<Mutant> mutants) =>
        CurrentStore.StoreMutations(mutants, MutationControl.Expression);

    /// <summary>
    /// Register new statement level mutations
    /// </summary>
    /// <param name="mutants"></param>
    public void AddStatementLevel(IEnumerable<Mutant> mutants) =>
        CurrentStore.StoreMutations(mutants, MutationControl.Statement);

    /// <summary>
    /// Register new block level mutations
    /// </summary>
    /// <param name="mutants"></param>
    public void AddBlockLevel(IEnumerable<Mutant> mutants) =>
        CurrentStore.StoreMutations(mutants, MutationControl.Block);

    /// <summary>
    /// Injects pending expression level mutations.
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="sourceNode">Source node, used to generate mutations</param>
    /// <returns>A mutated node containing the mutations.</returns>
    /// <remarks>Do not inject mutation(s) if in a subexpression</remarks>
    public ExpressionSyntax InjectExpressionLevel(ExpressionSyntax mutatedNode, ExpressionSyntax sourceNode)
        => CurrentStore.MemberAccessLength > 0
            ? mutatedNode
            : CurrentStore.PlaceExpressionMutations(mutatedNode, sourceNode.InjectMutation);

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
    /// Injects pending statement level mutations.
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="sourceNode">Source node, used to generate mutations</param>
    /// <returns>A mutated node containing the mutations.</returns>
    public StatementSyntax InjectStatementLevel(StatementSyntax mutatedNode, StatementSyntax sourceNode)
        => CurrentStore.PlaceStatementMutations(mutatedNode, sourceNode.InjectMutation);

    /// <summary>
    /// Injects pending block level mutations.
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="originalNode">Source node, used to generate mutations</param>
    /// <returns>A mutated node containing the mutations.</returns>
    public StatementSyntax InjectBlockLevel(StatementSyntax mutatedNode, StatementSyntax originalNode) =>
        CurrentStore.PlaceBlockMutations(mutatedNode, originalNode.InjectMutation);

    /// <summary>s
    /// Injects pending block level mutations for expression body method or functions
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="originalNode">Source node, used to generate mutations</param>
    /// <param name="needReturn">Set to true if the method has a return value. Expressions are transformed to return statement.</param>
    /// <returns>A mutated node containing the mutations.</returns>
    public StatementSyntax InjectBlockLevelExpressionMutation(BlockSyntax mutatedNode,
        ExpressionSyntax originalNode, bool needReturn)
    {
        var wrapper = needReturn
            ? (Func<ExpressionSyntax, StatementSyntax>)SyntaxFactory.ReturnStatement
            : SyntaxFactory.ExpressionStatement;

        if (CurrentStore.HasStatementLevel)
        {
            return CurrentStore.PlaceBlockMutations(
                CurrentStore.PlaceStatementMutations(mutatedNode, m => wrapper(originalNode.InjectMutation(m))),
                m => wrapper(originalNode.InjectMutation(m)));
        }

        return CurrentStore.PlaceBlockMutations(mutatedNode, m => wrapper(originalNode.InjectMutation(m)));
    }

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
