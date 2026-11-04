using System.Text;
using FSharp.Compiler.Syntax;
using Stryker.Abstractions;
using Stryker.Core.Mutants;
using Stryker.Fsharp.Mutators;
using Stryker.Fsharp.ProjectComponents;

namespace Stryker.Fsharp.Mutants;

/// <summary>
/// Walks the untyped FCS tree to find mutable expressions and records one text splice per mutant
/// (ADR mutation-mechanism). The walker is deliberately small: it reaches expressions through the node
/// kinds below and never enters a quotation.
/// </summary>
internal sealed class FsharpMutantOrchestrator(string helperNamespace, IEnumerable<IFsharpMutator>? mutators = null)
{
    private readonly IReadOnlyList<IFsharpMutator> _mutators = [.. mutators ?? [new BooleanMutator()]];
    private int _nextId;

    /// <summary>Mutates one file: attaches mutants and splices to the leaf. Returns the mutant count.</summary>
    public int Mutate(FsharpFileLeaf file)
    {
        var mutants = new List<Mutant>();
        file.Splices.Clear();
        if (file.ParseTree is ParsedInput.ImplFile impl)
        {
            foreach (var module in impl.Item.Contents)
            {
                VisitDeclarations(module.decls, file, mutants);
            }
        }

        file.Mutants = mutants;
        return mutants.Count;
    }

    /// <summary>Applies splices right-to-left by offset so earlier ranges stay valid.</summary>
    public static string ApplySplices(string source, IEnumerable<TextSplice> splices, string helperNamespace) =>
        ApplySplicesTracked(source, splices, helperNamespace).Text;

    /// <summary>
    /// Same as <see cref="ApplySplices"/>, and also returns where each spliced text lands in the result:
    /// every splice shifts all later ones by the length it added, accumulated left to right.
    /// </summary>
    public static AppliedSplices ApplySplicesTracked(string source, IEnumerable<TextSplice> splices, string helperNamespace)
    {
        var ordered = splices.OrderBy(s => s.Range.StartOffset).ToList();
        var result = new StringBuilder(source);
        foreach (var splice in Enumerable.Reverse(ordered))
        {
            var length = splice.Range.EndOffset - splice.Range.StartOffset;
            result.Remove(splice.Range.StartOffset, length);
            result.Insert(splice.Range.StartOffset, Wrap(splice, helperNamespace));
        }

        var shift = 0;
        var placed = new List<PlacedSplice>();
        foreach (var splice in ordered)
        {
            var wrappedLength = Wrap(splice, helperNamespace).Length;
            var start = splice.Range.StartOffset + shift;
            placed.Add(new PlacedSplice(splice.MutantId, start, start + wrappedLength));
            shift += wrappedLength - (splice.Range.EndOffset - splice.Range.StartOffset);
        }

        return new AppliedSplices(result.ToString(), placed);
    }

    private static string Wrap(TextSplice splice, string helperNamespace) =>
        $"(if {helperNamespace}.MutantControl.IsActive {splice.MutantId} then {splice.MutatedText} else {splice.OriginalText})";

    private void VisitDeclarations(IEnumerable<SynModuleDecl> declarations, FsharpFileLeaf file, List<Mutant> mutants)
    {
        foreach (var declaration in declarations)
        {
            switch (declaration)
            {
                case SynModuleDecl.Let let:
                    foreach (var binding in let.bindings)
                    {
                        VisitExpression(binding.expr, file, mutants);
                    }

                    break;
                case SynModuleDecl.NestedModule nested:
                    VisitDeclarations(nested.decls, file, mutants);
                    break;
                case SynModuleDecl.Expr expr:
                    VisitExpression(expr.expr, file, mutants);
                    break;
            }
        }
    }

    private void VisitExpression(SynExpr expression, FsharpFileLeaf file, List<Mutant> mutants)
    {
        switch (expression)
        {
            // An injected IsActive call would surface in the expression tree, so quotations are left alone.
            case SynExpr.Quote:
                return;
            case SynExpr.Paren paren:
                VisitExpression(paren.expr, file, mutants);
                return;
            case SynExpr.App app:
                VisitExpression(app.funcExpr, file, mutants);
                VisitExpression(app.argExpr, file, mutants);
                return;
            case SynExpr.Typed typed:
                VisitExpression(typed.expr, file, mutants);
                return;
            case SynExpr.Sequential seq:
                VisitExpression(seq.expr1, file, mutants);
                VisitExpression(seq.expr2, file, mutants);
                return;
            case SynExpr.IfThenElse ite:
                VisitExpression(ite.ifExpr, file, mutants);
                VisitExpression(ite.thenExpr, file, mutants);
                if (ite.elseExpr is { } elseExpr)
                {
                    VisitExpression(elseExpr.Value, file, mutants);
                }

                return;
            case SynExpr.LetOrUse letOrUse:
                foreach (var binding in letOrUse.Item.Bindings)
                {
                    VisitExpression(binding.expr, file, mutants);
                }

                VisitExpression(letOrUse.Item.Body, file, mutants);
                return;
        }

        foreach (var mutator in _mutators)
        {
            foreach (var mutation in mutator.ApplyMutations(expression, file.SourceCode))
            {
                var id = _nextId++;
                mutants.Add(new Mutant { Id = id, Mutation = mutation });
                file.Splices.Add(new TextSplice(id, mutation.Location, mutation.OriginalText, mutation.ReplacementText));
            }
        }
    }
}
