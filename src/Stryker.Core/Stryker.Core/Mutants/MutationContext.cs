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
    private readonly MutationControl _control;
    private int _depth;

    public PendingMutations(MutationControl control) => _control = control;

    public bool CanBeAggregatedWith(MutationControl control) => Store.Count == 0 && _control == control;

    public void Aggregate() => _depth++;

    public void StoreMutations(IEnumerable<Mutant> store) => Store.AddRange(store.Where( m => m.ResultStatus == MutantStatus.Pending));

    public static bool StoreMutationsAtDesiredLevel(IEnumerable<PendingMutations> list, IEnumerable<Mutant> store, MutationControl level)
    {
        foreach(var item in list)
        {
            if (item._control == level)
            {
                item.StoreMutations(store);
                return true;
            }
        }   

        if (level == MutationControl.Block)
        {
            Logger.LogError($"There is not statement to control {store.Count()} mutations. They are dropped.");
            foreach (var mutant in store)
            {
                mutant.ResultStatus = MutantStatus.CompileError;
                mutant.ResultStatusReason = "Could not be injected in code.";
            }
        }
        else if (level == MutationControl.Statement)
        {
            return StoreMutationsAtDesiredLevel(list, store, MutationControl.Block);
        }
        return false;
    }

    public static bool HasStatementLevelMutations(IEnumerable<PendingMutations> list)
    {
        foreach(var item in list)
        {
            if (item._control == MutationControl.Statement)
            {
                return item.Store.Count>0;
            }
        }   
        return false;
    }

    public bool Leave()
    {
        if (_depth <= 0)
        {
            return true;
        }

        _depth--;
        return false;
    }
    
    public virtual SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode) => mutatedNode;

    public void Transfer(PendingMutations old) => Store.AddRange(old.Store);
}

internal class PendingMutationsForExpression : PendingMutations
{
    public PendingMutationsForExpression() : base(MutationControl.Expression)
    {
    }

    public override SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode)
    {
        // we can only inject at expression level
        if (originalNode is not ExpressionSyntax originalExpression || Store.Count == 0)
        {
            return mutatedNode;
        }

        var result = placer.PlaceExpressionControlledMutations((ExpressionSyntax)mutatedNode, Store.Select(m => (m, originalExpression.InjectMutation(m.Mutation))));
        Store.Clear();
        return result;
    }
}

internal class PendingMutationsForMemberAccess: PendingMutations
{
    public PendingMutationsForMemberAccess() : base(MutationControl.MemberAccess)
    {}

    public override SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode) => mutatedNode;

}

internal class PendingMutationsForMember: PendingMutations
{
    public PendingMutationsForMember() : base(MutationControl.Member)
    {}
}

internal class PendingMutationsForStatement : PendingMutations
{
    public PendingMutationsForStatement() : base(MutationControl.Statement)
    {}

    public override SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode)
    {
        if (Store.Count == 0 || mutatedNode is not StatementSyntax node)
        {
            return mutatedNode;
        }
        var result = placer.PlaceStatementControlledMutations(node, Store.Select(m => (m, ((StatementSyntax) originalNode).InjectMutation(m.Mutation))));
        Store.Clear();
        return result;
    }
}

internal class PendingMutationsForBlock : PendingMutations
{
    public PendingMutationsForBlock() : base(MutationControl.Block)
    {}

    public override SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode)
    {
        if (Store.Count == 0 || mutatedNode is not StatementSyntax node)
        {
            return mutatedNode;
        }
        var result = placer.PlaceStatementControlledMutations(node, Store.Select(m => (m, ((StatementSyntax) originalNode).InjectMutation(m.Mutation))));
        Store.Clear();
        return result;
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
    private readonly Stack<PendingMutations> _pendingMutations = new();

    /// <summary>
    /// Mutation context must be created once when starting a mutation process.
    /// </summary>
    /// <param name="mutantOrchestrator"></param>
    public MutationContext(CsharpMutantOrchestrator mutantOrchestrator)
    {
        _mainOrchestrator = mutantOrchestrator;
        _pendingMutations.Push(new PendingMutationsForMember());
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
    public bool HasStatementLevelMutant => PendingMutations.HasStatementLevelMutations(_pendingMutations);

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
        if (_pendingMutations.Count>0 && _pendingMutations.Peek().CanBeAggregatedWith(control))
        {
            _pendingMutations.Peek().Aggregate();
        }
        else
        {
            switch(control)
            {
                case MutationControl.MemberAccess:
                    _pendingMutations.Push(new PendingMutationsForMemberAccess());
                    break;
                case MutationControl.Expression:
                    _pendingMutations.Push(new PendingMutationsForExpression());
                    break;
                case MutationControl.Statement:
                    _pendingMutations.Push(new PendingMutationsForStatement());
                    break;
                case MutationControl.Block:
                    _pendingMutations.Push(new PendingMutationsForBlock());
                    break;
                case MutationControl.Member:
                    _pendingMutations.Push(new PendingMutationsForMember());
                    break;
            }
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
        if (_pendingMutations.Peek().Leave())
        {
            // we need to store pending mutations at the higher level
            var old = _pendingMutations.Pop();
            _pendingMutations.Peek().Transfer(old);
        }

        return this;
    }

    /// <summary>
    /// Register new statement level mutations
    /// </summary>
    /// <param name="mutants"></param>
    public void AddExpressionLevel(IEnumerable<Mutant> mutants) => _pendingMutations.Peek().StoreMutations(mutants);

    /// <summary>
    /// Register new statement level mutations
    /// </summary>
    /// <param name="mutants"></param>
    public void AddStatementLevel(IEnumerable<Mutant> mutants) => PendingMutations.StoreMutationsAtDesiredLevel(_pendingMutations.ToList(), mutants, MutationControl.Statement);

    /// <summary>
    /// Register new block level mutations
    /// </summary>
    /// <param name="mutants"></param>
    public void AddBlockLevel(IEnumerable<Mutant> mutants) => PendingMutations.StoreMutationsAtDesiredLevel(_pendingMutations.ToList(), mutants, MutationControl.Block);

    /// <summary>
    /// Injects pending expression level mutations.
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="sourceNode">Source node, used to generate mutations</param>
    /// <returns>A mutated node containing the mutations.</returns>
    /// <remarks>Do not inject mutation(s) if in a subexpression</remarks>
    public ExpressionSyntax InjectExpressionLevel(ExpressionSyntax mutatedNode, ExpressionSyntax sourceNode) => (ExpressionSyntax) _pendingMutations.Peek().Inject(_mainOrchestrator.Placer, sourceNode, mutatedNode);

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
        => (StatementSyntax)_pendingMutations.Peek().Inject(_mainOrchestrator.Placer, sourceNode, mutatedNode);

    /// <summary>
    /// Injects pending block level mutations.
    /// </summary>
    /// <param name="mutatedNode">Target node that will contain the mutations</param>
    /// <param name="originalNode">Source node, used to generate mutations</param>
    /// <returns>A mutated node containing the mutations.</returns>
    public StatementSyntax InjectBlockLevel(StatementSyntax mutatedNode, StatementSyntax originalNode) =>
        (StatementSyntax)_pendingMutations.Peek().Inject(_mainOrchestrator.Placer, originalNode, mutatedNode);

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

        
        if (PendingMutations.HasStatementLevelMutations(_pendingMutations))
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
