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
internal sealed class FsharpMutantOrchestrator(string helperNamespace, IEnumerable<IFsharpMutator>? mutators = null, MutationLevel mutationLevel = MutationLevel.Complete)
{
    private readonly IReadOnlyList<IFsharpMutator> _mutators = [.. (mutators ?? [new BooleanMutator(), new ArithmeticOperatorMutator(), new EqualityOperatorMutator()]).Where(m => m.MutationLevel <= mutationLevel)];
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

    /// <summary>Applies splices inner-first; see <see cref="ApplySplicesTracked"/>.</summary>
    public static string ApplySplices(string source, IEnumerable<TextSplice> splices, string helperNamespace) =>
        ApplySplicesTracked(source, splices, helperNamespace).Text;

    /// <summary>
    /// Applies splices and returns where each wrapped text lands in the result. Splices may nest: a splice
    /// contained in another is applied first, so the outer splice wraps text that already carries the inner guard,
    /// in its original branch and, where the mutated text preserves the inner original text, in its mutated branch.
    /// Siblings are applied right-to-left by offset. Partially overlapping splices are rejected.
    /// </summary>
    public static AppliedSplices ApplySplicesTracked(string source, IEnumerable<TextSplice> splices, string helperNamespace)
    {
        var ordered = Order(splices);
        var index = 0;
        var (text, placed) = Compose(source, 0, source.Length, ordered, ref index, helperNamespace);
        return new AppliedSplices(text, placed);
    }

    private static List<TextSplice> Order(IEnumerable<TextSplice> splices) =>
        splices.OrderBy(s => s.Range.StartOffset).ThenByDescending(s => s.Range.EndOffset).ToList();

    /// <summary>Text of source[start..end) with every splice found there applied; placements are relative to that text.</summary>
    private static (string Text, List<PlacedSplice> Placed) Compose(string source, int start, int end, List<TextSplice> ordered, ref int index, string helperNamespace)
    {
        var builder = new StringBuilder();
        var placed = new List<PlacedSplice>();
        var cursor = start;
        while (index < ordered.Count && ordered[index].Range.StartOffset < end)
        {
            var splice = ordered[index];
            if (splice.Range.StartOffset < cursor || splice.Range.EndOffset > end)
            {
                throw new InvalidOperationException($"Mutant {splice.MutantId} partially overlaps another mutant range");
            }

            index++;
            builder.Append(source, cursor, splice.Range.StartOffset - cursor);
            var (wrapped, wrappedPlaced) = ComposeOne(source, splice, ordered, ref index, helperNamespace);
            if (!splice.IsTrackValue)
            {
                placed.Add(new PlacedSplice(splice.MutantId, builder.Length, builder.Length + wrapped.Length));
            }

            placed.AddRange(wrappedPlaced.Select(p => Shift(p, builder.Length)));
            builder.Append(wrapped);
            cursor = splice.Range.EndOffset;
        }

        builder.Append(source, cursor, end - cursor);
        return (builder.ToString(), placed);
    }

    private static (string Wrapped, List<PlacedSplice> Placed) ComposeOne(string source, TextSplice splice, List<TextSplice> ordered, ref int index, string helperNamespace)
    {
        var firstChild = index;
        var (original, originalPlaced) = Compose(source, splice.Range.StartOffset, splice.Range.EndOffset, ordered, ref index, helperNamespace);
        if (splice.IsTrackValue)
        {
            // the explicit delegate type keeps F# inference happy; the wrapped text is not a mutant, so it is not placed
            var trackPrefix = $"{helperNamespace}.StaticValueTracker.Track(System.Func<_>(fun () -> ";
            return (trackPrefix + original + "))", originalPlaced.Select(p => Shift(p, trackPrefix.Length)).ToList());
        }

        var (mutated, mutatedPlaced) = ComposeMutated(source, splice, ordered.GetRange(firstChild, index - firstChild), helperNamespace);

        var prefix = $"(if {helperNamespace}.MutantControl.IsActive {splice.MutantId} then ";
        const string separator = " else ";
        var originalStart = prefix.Length + mutated.Length + separator.Length;
        var placed = mutatedPlaced.Select(p => Shift(p, prefix.Length))
            .Concat(originalPlaced.Select(p => Shift(p, originalStart)))
            .ToList();
        return (prefix + mutated + separator + original + ")", placed);
    }

    /// <summary>
    /// The mutated text with each contained splice re-applied where the mutated text preserves that splice's
    /// original source text (first occurrence after the previous one). A part the mutation discards carries no inner guard.
    /// </summary>
    private static (string Text, List<PlacedSplice> Placed) ComposeMutated(string source, TextSplice splice, List<TextSplice> contained, string helperNamespace)
    {
        var builder = new StringBuilder();
        var placed = new List<PlacedSplice>();
        var cursor = 0;
        var consumedUntil = int.MinValue;
        foreach (var child in contained)
        {
            if (child.Range.StartOffset < consumedUntil)
            {
                continue; // nested deeper: composed with its direct parent
            }

            consumedUntil = child.Range.EndOffset;
            var originalSlice = source.Substring(child.Range.StartOffset, child.Range.EndOffset - child.Range.StartOffset);
            var found = splice.MutatedText.IndexOf(originalSlice, cursor, StringComparison.Ordinal);
            if (found < 0)
            {
                continue;
            }

            builder.Append(splice.MutatedText, cursor, found - cursor);
            var subtree = Order(contained.Where(c => c.Range.StartOffset >= child.Range.StartOffset && c.Range.EndOffset <= child.Range.EndOffset));
            var index = 0;
            var (text, childPlaced) = Compose(source, child.Range.StartOffset, child.Range.EndOffset, subtree, ref index, helperNamespace);
            placed.AddRange(childPlaced.Select(p => Shift(p, builder.Length)));
            builder.Append(text);
            cursor = found + originalSlice.Length;
        }

        builder.Append(splice.MutatedText, cursor, splice.MutatedText.Length - cursor);
        return (builder.ToString(), placed);
    }

    private static PlacedSplice Shift(PlacedSplice placed, int by) => placed with { StartOffset = placed.StartOffset + by, EndOffset = placed.EndOffset + by };

    private void VisitDeclarations(IEnumerable<SynModuleDecl> declarations, FsharpFileLeaf file, List<Mutant> mutants)
    {
        foreach (var declaration in declarations)
        {
            switch (declaration)
            {
                case SynModuleDecl.Let let:
                    foreach (var binding in let.bindings)
                    {
                        VisitModuleBinding(binding, file, mutants);
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

    /// <summary>
    /// A module-level value (<c>let x = ...</c>) is built once, by the module's static initialiser, so its mutants are
    /// static: flagged, and the expression is wrapped to report what runs while it is built. Functions are left alone.
    /// </summary>
    private void VisitModuleBinding(SynBinding binding, FsharpFileLeaf file, List<Mutant> mutants)
    {
        var firstMutant = mutants.Count;
        var firstSplice = file.Splices.Count;
        VisitExpression(binding.expr, file, mutants);
        if (mutants.Count == firstMutant || IsFunction(binding))
        {
            return;
        }

        foreach (var mutant in mutants.Skip(firstMutant))
        {
            mutant.IsStaticValue = true;
        }

        var range = FsharpLocations.ToLocation(binding.expr.Range, file.SourceCode);
        // listed before the mutant splices, which may share its range: the wrapper must be the outer one
        file.Splices.Insert(firstSplice, TextSplice.TrackValue(range, file.SourceCode.Substring(range.StartOffset, range.EndOffset - range.StartOffset)));
    }

    private static bool IsFunction(SynBinding binding) =>
        binding.headPat is SynPat.LongIdent { argPats: SynArgPats.Pats { pats: { IsEmpty: false } } };

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
                AddMutants(expression, file, mutants);
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

        AddMutants(expression, file, mutants);
    }

    private void AddMutants(SynExpr expression, FsharpFileLeaf file, List<Mutant> mutants)
    {
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
