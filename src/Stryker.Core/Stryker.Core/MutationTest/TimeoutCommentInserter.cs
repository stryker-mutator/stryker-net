using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Stryker.Abstractions;

namespace Stryker.Core.MutationTest;

/// <summary>
/// Adds <c>// Stryker disable once</c> comments to a syntax tree for the supplied mutants.
/// Comments are attached to the leading trivia of the closest statement or member declaration enclosing the mutation,
/// which guarantees they end up in a position the compiler and the Stryker comment parser both understand.
/// </summary>
internal static class TimeoutCommentInserter
{
    internal const string Reason = "Auto-ignored by Stryker (--auto-ignore-timeouts), the mutation caused a timeout";

    /// <summary>
    /// Returns the updated source text or null when no comment could be placed.
    /// </summary>
    /// <param name="root">The root of the (original) syntax tree the mutants originate from.</param>
    /// <param name="mutants">The mutants to ignore.</param>
    /// <param name="unplacedMutants">Mutants for which no valid comment location was found.</param>
    public static string Insert(SyntaxNode root, IEnumerable<IReadOnlyMutant> mutants, out IReadOnlyList<IReadOnlyMutant> unplacedMutants)
    {
        var text = root.SyntaxTree.GetText();
        var unplaced = new List<IReadOnlyMutant>();
        // key: first token of the node receiving the comment
        var mutatorsPerToken = new Dictionary<SyntaxToken, SortedSet<string>>();

        foreach (var mutant in mutants)
        {
            var target = FindCommentTarget(root, mutant);
            if (target is null)
            {
                unplaced.Add(mutant);
                continue;
            }

            var firstToken = target.GetFirstToken();
            if (!mutatorsPerToken.TryGetValue(firstToken, out var mutators))
            {
                mutators = new SortedSet<string>(StringComparer.Ordinal);
                mutatorsPerToken[firstToken] = mutators;
            }
            mutators.Add(mutant.Mutation.Type.ToString());
        }

        unplacedMutants = unplaced;
        if (mutatorsPerToken.Count == 0)
        {
            return null;
        }

        var newLine = text.ToString().Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        var newRoot = root.ReplaceTokens(mutatorsPerToken.Keys, (original, _) =>
            original.WithLeadingTrivia(original.LeadingTrivia.AddRange(
                CreateCommentTrivia(text, original, mutatorsPerToken[original], newLine))));

        return newRoot.ToFullString();
    }

    private static SyntaxNode FindCommentTarget(SyntaxNode root, IReadOnlyMutant mutant)
    {
        var span = mutant.Mutation.OriginalNode?.Span;
        if (span is null || !root.FullSpan.Contains(span.Value))
        {
            return null;
        }

        return root.FindNode(span.Value)
            .AncestorsAndSelf()
            .FirstOrDefault(n => n is StatementSyntax and not BlockSyntax || n is MemberDeclarationSyntax);
    }

    private static IEnumerable<SyntaxTrivia> CreateCommentTrivia(SourceText text, SyntaxToken token, IEnumerable<string> mutators, string newLine)
    {
        var line = text.Lines.GetLineFromPosition(token.SpanStart);
        var lineText = text.ToString(line.Span);
        var indentation = new string(lineText.TakeWhile(c => c is ' ' or '\t').ToArray());

        yield return SyntaxFactory.Comment($"// Stryker disable once {string.Join(',', mutators)}: {Reason}");
        yield return SyntaxFactory.EndOfLine(newLine);
        if (indentation.Length > 0)
        {
            yield return SyntaxFactory.Whitespace(indentation);
        }
    }
}
