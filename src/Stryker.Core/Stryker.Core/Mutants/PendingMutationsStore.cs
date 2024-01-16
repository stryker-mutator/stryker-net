using System;
using System.Collections.Generic;
using System.Linq;
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

    public bool HasPendingMutations() => _pendingMutations.Count > 0 && _pendingMutations.Peek().Store.Count>0;

    public void Enter(MutationControl control)
    {
        if (_pendingMutations.Count > 0 && _pendingMutations.Peek().Aggregate(control))
        {
            return;
        }
        _pendingMutations.Push(new PendingMutations(control));
    }
    public void Leave()
    {
        if (!_pendingMutations.Peek().Leave()) return;
        // we need to store pending mutations at the higher level
        var old = _pendingMutations.Pop();
        if (_pendingMutations.Count > 0)
        {
            _pendingMutations.Peek().StoreMutations(old.Store);
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

    private PendingMutations FindControl(MutationControl control) => _pendingMutations.FirstOrDefault(item => item.Control >= control);

    public bool StoreMutationsAtDesiredLevel(IEnumerable<Mutant> store, MutationControl level)
    {
        var controller = FindControl(level);

        if (controller != null)
        {
            controller.StoreMutations(store);
            return true;
        }
        Logger.LogError($"There is no structure to control {store.Count()} mutations. They are dropped.");
        foreach (var mutant in store)
        {
            mutant.ResultStatus = MutantStatus.CompileError;
            mutant.ResultStatusReason = "Could not be injected in code.";
        }
        return false;
    }

    public bool StoreMutations(IEnumerable<Mutant> store)
    {
        if (_pendingMutations.Count == 0)
        {
            return false;
        }
        _pendingMutations.Peek().StoreMutations(store);
        return true;
    }

    public ExpressionSyntax Inject(ExpressionSyntax mutatedNode, ExpressionSyntax sourceNode)
    {
        if (_pendingMutations.Peek().Control == MutationControl.MemberAccess)
        {
            // never inject at member access level, there is no known control structure
            return mutatedNode;
        }
        var store = _pendingMutations.Peek().Store;
        var result = _mutantPlacer.PlaceExpressionControlledMutations(mutatedNode,
             store.Select(m => (m, sourceNode.InjectMutation(m.Mutation))));
        store.Clear();
        return result;
    }

    public StatementSyntax Inject(StatementSyntax mutatedNode, StatementSyntax sourceNode)
    {
        var store = _pendingMutations.Peek().Store;
        var result = _mutantPlacer.PlaceStatementControlledMutations(mutatedNode,
            store.Select(m => (m, sourceNode.InjectMutation(m.Mutation))));
        store.Clear();
        return result;
    }

    public BlockSyntax Inject(BlockSyntax mutatedNode, BlockSyntax sourceNode)
    {
        var store = _pendingMutations.Peek().Store;
        var result = _mutantPlacer.PlaceStatementControlledMutations(mutatedNode,
            store.Select(m => (m, ((StatementSyntax)sourceNode).InjectMutation(m.Mutation))));
        store.Clear();

        return result as BlockSyntax ?? SyntaxFactory.Block(result);
    }

    public BlockSyntax Inject(BlockSyntax mutatedNode, ExpressionSyntax originalNode, bool needReturn)
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

        public PendingMutations(MutationControl control) => Control = control;

        public bool Aggregate(MutationControl control)
        {
            if (Store.Count != 0 || Control != control) return false;
            _depth++;
            return true;
        }

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
    }
}
