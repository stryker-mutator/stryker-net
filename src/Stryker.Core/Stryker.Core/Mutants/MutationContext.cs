using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Helpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants.CsharpNodeOrchestrators;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutants;


internal class PendingMutations
{
    protected static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationStore>();
    protected List<Mutant> Store = new();
    protected PendingMutations Parent;
    private MutationControl _control;
    protected int _depth;

    public PendingMutations(MutationControl control, PendingMutations parent)
    {
        _control = control;
        Parent = parent;
    }

    public virtual void StoreMutations(IList<Mutant> store) { }

    public virtual void StoreStatementLevelMutations(IList<Mutant> store)
    {
        var pendingMutations = this;
        while(pendingMutations is not PendingMutationsForStatement and not null)
        {
            pendingMutations = pendingMutations.Parent;
        }
        // if we reach the root, we log and error and drop the mutations
        if (pendingMutations is null)
        {
            StoreBlockLevelMutations(store);
            return;
        }
        pendingMutations.Store.AddRange(store);
    }

    public void StoreBlockLevelMutations(IList<Mutant> store)
    {
        var pendingMutations = this;
        while (pendingMutations is not PendingMutationsForBlock and not null)
        {
            pendingMutations = pendingMutations.Parent;
        }
        // if we reach the root, we log and error and drop the mutations
        if (pendingMutations is null)
        {
            Logger.LogError($"There is not block to control {store.Count} mutations. They are dropped.");
            foreach (var mutant in store)
            {
                mutant.ResultStatus = MutantStatus.CompileError;
                mutant.ResultStatusReason = "Could not be injected in code.";
            }
            return;
        }
        pendingMutations.Store.AddRange(store);
        return;
    }

    public PendingMutations Enter(MutationControl control)
    {
        if (control == _control)
        {
            _depth++;
            return this;
        }
        switch(control)
        {
            case MutationControl.MemberAccess:
                return new PendingMutationsForMemberAccess(this);
            case MutationControl.Expression:
                return new PendingMutationsForExpression(this);
            case MutationControl.Statement:
                return new PendingMutationsForStatement(this);
            case MutationControl.Block:
                return new PendingMutationsForBlock(this);
            case MutationControl.Member:
                return new PendingMutationsForMember(this);
            default:
                return null;
        }
    }

    public PendingMutations Leave()
    {
        if (_depth <= 0)
        {
            Parent?.StoreMutations(Store);
            return Parent;
        }

        _depth--;
        return this;
    }

    public virtual SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode) => mutatedNode;
}

internal class PendingMutationsForExpression : PendingMutations
{
    public PendingMutationsForExpression(PendingMutations parent) : base(MutationControl.Expression, parent)
    {
    }

    public override void StoreMutations(IList<Mutant> store)
    {
        Store.AddRange(store);
    }

    public override SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode)
    {
        // we can only inject at expression level
        if (originalNode is not ExpressionSyntax originalExpression || Store.Count == 0)
        {
            return mutatedNode;
        }

        var result = placer.PlaceExpressionControlledMutations(originalExpression, Store.Select(m => (m, (ExpressionSyntax)originalNode.InjectMutation(m.Mutation))));
        Store.Clear();
        return result;
    }
    }

internal class PendingMutationsForMemberAccess: PendingMutations
{
    public PendingMutationsForMemberAccess(PendingMutations parent) : base(MutationControl.MemberAccess, parent)
    {}

    public override SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode) => mutatedNode;

    // mutations are never handled at this level
    public override void StoreMutations(IList<Mutant> store) => Parent?.StoreMutations(store);
}

internal class PendingMutationsForMember: PendingMutations
{
    public PendingMutationsForMember(PendingMutations parent) : base(MutationControl.MemberAccess, parent)
    {}

    // mutations are never handled at this level
    public override void StoreMutations(IList<Mutant> store) => Parent?.StoreMutations(store);
}

internal class PendingMutationsForStatement : PendingMutations
{
    public PendingMutationsForStatement(PendingMutations parent) : base(MutationControl.Statement, parent)
    {}

    public override void StoreMutations(IList<Mutant> store)
    {
        Store.AddRange(store);
    }
}

internal class PendingMutationsForBlock : PendingMutations
{
    public PendingMutationsForBlock(PendingMutations parent) : base(MutationControl.Block, parent)
    {
    }

    public override void StoreMutations(IList<Mutant> store)
    {
        Store.AddRange(store);
    }
}

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
    private readonly Stack<MutationStore> _pendingMutationsStore = new();
    private PendingMutations _pendingMutations;

    /// <summary>
    /// Mutation context must be created once when starting a mutation process.
    /// </summary>
    /// <param name="mutantOrchestrator"></param>
    public MutationContext(CsharpMutantOrchestrator mutantOrchestrator)
    {
        _mainOrchestrator = mutantOrchestrator;
        _pendingMutations = new PendingMutationsForMember(null);
        _pendingMutationsStore.Push(new MutationStore(_mainOrchestrator.Placer));
        CurrentStore.EnterBlock();
    }

    private MutationContext(MutationContext parent)
    {
        _mainOrchestrator = parent._mainOrchestrator;
        InStaticValue = parent.InStaticValue;
        _pendingMutationsStore = parent._pendingMutationsStore;
        _pendingMutations = parent._pendingMutations;
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

    private MutationStore CurrentStore => _pendingMutationsStore.Peek();

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
    /// <returns>A handler for this node.</returns>
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
        _pendingMutations = _pendingMutations.Enter(control);
        switch (control)
        {
            case MutationControl.MemberAccess:
                return this;
            case MutationControl.Statement:
                CurrentStore.EnterStatement();
                return this;
            case MutationControl.Block:
                CurrentStore.EnterBlock();
                return new MutationContext(this);
            case MutationControl.Member:
                _pendingMutationsStore.Push(new MutationStore(_mainOrchestrator.Placer));
                CurrentStore.EnterBlock();
                return new MutationContext(this);
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
        var next = _pendingMutations.Leave();
        if (next == null)
        {

        }
        _pendingMutations = next;
        switch (control)
        {
            case MutationControl.MemberAccess:
                break;
            case MutationControl.Statement:
                CurrentStore.LeaveStatement();
                break;
            case MutationControl.Block:
                CurrentStore.LeaveBlock();
                break;
            case MutationControl.Member:
                CurrentStore.LeaveBlock();
                _pendingMutationsStore.Pop();
                break;
        }
        return this;
    }

    /// <summary>
    /// Register new statement level mutations
    /// </summary>
    /// <param name="mutants"></param>
    public void AddExpressionLevel(IEnumerable<Mutant> mutants)
    {
        if (!mutants.Any())
        {
            return;
        }
        CurrentStore.StoreMutations(mutants, MutationControl.Expression);
        _pendingMutations.StoreMutations(mutants.ToList());
    }

    /// <summary>
    /// Register new statement level mutations
    /// </summary>
    /// <param name="mutants"></param>
    public void AddStatementLevel(IEnumerable<Mutant> mutants)
    {
        CurrentStore.StoreMutations(mutants, MutationControl.Statement);
        _pendingMutations.StoreStatementLevelMutations(mutants.ToList());
    }

    /// <summary>
    /// Register new block level mutations
    /// </summary>
    /// <param name="mutants"></param>
    public void AddBlockLevel(IEnumerable<Mutant> mutants)
    {
        CurrentStore.StoreMutations(mutants, MutationControl.Block);
        _pendingMutations.StoreBlockLevelMutations(mutants.ToList());
    }

    /// <summary>
    /// Injects pending expression level mutations.
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="sourceNode">Source node, used to generate mutations</param>
    /// <returns>A mutated node containing the mutations.</returns>
    /// <remarks>Do not inject mutation(s) if in a subexpression</remarks>
    public ExpressionSyntax InjectExpressionLevel(ExpressionSyntax mutatedNode, ExpressionSyntax sourceNode)
    {
        return (ExpressionSyntax) _pendingMutations.Inject(_mainOrchestrator.Placer, sourceNode, mutatedNode);
        /*
        return _memberAccessLength > 0
            ? mutatedNode
            : CurrentStore.PlaceExpressionMutations(mutatedNode, sourceNode.InjectMutation);*/
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
