using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Helpers;
using Stryker.Core.Logging;

namespace Stryker.Core.Mutants;

internal class PendingMutationsStore
{
    protected static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationStore>();
    private readonly MutantPlacer _mutantPlacer;
    private readonly Stack<PendingMutations> _pendingMutations = new();

    public PendingMutationsStore(MutantPlacer mutantPlacer) => _mutantPlacer = mutantPlacer;

    public bool HasStatementLevelMutations()
    {
        foreach (var item in (IEnumerable<PendingMutations>)_pendingMutations)
        {
            if (item.Control == MutationControl.Member)
            {
                return item.Store.Count > 0;
            }
        }

        return false;
    }

    public void Enter(MutationControl control)
    {
        if (_pendingMutations.Count > 0 && _pendingMutations.Peek().CanBeAggregatedWith(control))
        {
            _pendingMutations.Peek().Aggregate();
            return;
        }
        switch (control)
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

    private PendingMutations FindControl(MutationControl control) => control == MutationControl.Block ? FindEnclosingBlock() : _pendingMutations.FirstOrDefault(item => item.Control == control);

    private PendingMutations FindEnclosingBlock() => _pendingMutations.FirstOrDefault(item => item.Control is MutationControl.Block or MutationControl.Member);

    public bool StoreMutationsAtDesiredLevel(IEnumerable<Mutant> store, MutationControl level)
    {
        var controller = FindControl(level) ?? FindEnclosingBlock();

        if (controller != null)
        {
            controller.StoreMutations(store);
            return true;
        }
        Logger.LogError($"There is not statement to control {store.Count()} mutations. They are dropped.");
        foreach (var mutant in store)
        {
            mutant.ResultStatus = MutantStatus.CompileError;
            mutant.ResultStatusReason = "Could not be injected in code.";
        }
        return false;
    }

    public ExpressionSyntax Inject(ExpressionSyntax mutatedNode, ExpressionSyntax sourceNode) =>
        (ExpressionSyntax)_pendingMutations.Peek().Inject(_mutantPlacer, sourceNode, mutatedNode);

    public StatementSyntax Inject(StatementSyntax mutatedNode, StatementSyntax sourceNode) =>
        (StatementSyntax)_pendingMutations.Peek().Inject(_mutantPlacer, sourceNode, mutatedNode);

    public BlockSyntax Inject(BlockSyntax mutatedNode, BlockSyntax sourceNode)
    {
        var control = (StatementSyntax)FindControl(MutationControl.Block)?.Inject(_mutantPlacer, sourceNode, mutatedNode);
        return control as BlockSyntax ?? SyntaxFactory.Block(control);
    }

    public void Leave()
    {
        if (!_pendingMutations.Peek().Leave()) return;
        // we need to store pending mutations at the higher level
        var old = _pendingMutations.Pop();
        if (_pendingMutations.Count > 0)
        {
            _pendingMutations.Peek().Transfer(old);
        }
        else
        {
            Logger.LogError("Some mutations failed to be inserted, they are dropped.");
            foreach (var mutant in old.Store)
            {
                mutant.ResultStatus = MutantStatus.CompileError;
                mutant.ResultStatusReason = "Could not be injected in code.";
            }
        }
    }

    public BlockSyntax InjectExpressionsAsBlock(ExpressionSyntax originalNode, BlockSyntax mutatedNode, bool needReturn)
    {
        var wrapper = needReturn
            ? (Func<ExpressionSyntax, StatementSyntax>)SyntaxFactory.ReturnStatement
            : SyntaxFactory.ExpressionStatement;

        var blockStore = _pendingMutations.Peek();
        if (blockStore.Store.Count == 0)
        {
            return mutatedNode;
        }

        var result = _mutantPlacer.PlaceStatementControlledMutations(mutatedNode,
            blockStore.Store.Select(m => (m, wrapper(originalNode.InjectMutation(m.Mutation)))));
        blockStore.Store.Clear();
        return result as BlockSyntax ?? SyntaxFactory.Block(result);
    }

    private class PendingMutations
    {
        public readonly MutationControl Control;
        private int _depth;
        public readonly List<Mutant> Store = new();

        protected PendingMutations(MutationControl control) => Control = control;

        public bool CanBeAggregatedWith(MutationControl control) => Store.Count == 0 && Control == control;

        public void Aggregate() => _depth++;

        public void StoreMutations(IEnumerable<Mutant> store) =>
            Store.AddRange(store.Where(m => m.ResultStatus == MutantStatus.Pending));


        public bool Leave()
        {
            if (_depth <= 0)
            {
                return true;
            }

            _depth--;
            return false;
        }

        public virtual SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode) =>
            mutatedNode;

        public void Transfer(PendingMutations old) => Store.AddRange(old.Store);
    }

    private class PendingMutationsForExpression : PendingMutations
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

            var result = placer.PlaceExpressionControlledMutations((ExpressionSyntax)mutatedNode,
                Store.Select(m => (m, originalExpression.InjectMutation(m.Mutation))));
            Store.Clear();
            return result;
        }
    }

    private class PendingMutationsForMemberAccess : PendingMutations
    {
        public PendingMutationsForMemberAccess() : base(MutationControl.MemberAccess)
        {
        }

        public override SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode) =>
            mutatedNode;
    }

    private class PendingMutationsForMember : PendingMutations
    {
        public PendingMutationsForMember() : base(MutationControl.Member)
        {
        }
    }

    private class PendingMutationsForStatement : PendingMutations
    {
        public PendingMutationsForStatement() : base(MutationControl.Statement)
        {
        }

        public override SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode)
        {
            if (Store.Count == 0 || mutatedNode is not StatementSyntax node)
            {
                return mutatedNode;
            }

            var result = placer.PlaceStatementControlledMutations(node,
                Store.Select(m => (m, ((StatementSyntax)originalNode).InjectMutation(m.Mutation))));
            Store.Clear();
            return result;
        }
    }

    private class PendingMutationsForBlock : PendingMutations
    {
        public PendingMutationsForBlock() : base(MutationControl.Block)
        {
        }

        public override SyntaxNode Inject(MutantPlacer placer, SyntaxNode originalNode, SyntaxNode mutatedNode)
        {
            if (Store.Count == 0 || mutatedNode is not BlockSyntax node)
            {
                return mutatedNode;
            }

            var result = placer.PlaceStatementControlledMutations(node,
                Store.Select(m => (m, ((StatementSyntax)originalNode).InjectMutation(m.Mutation))));
            Store.Clear();
            return result;
        }
    }
}
