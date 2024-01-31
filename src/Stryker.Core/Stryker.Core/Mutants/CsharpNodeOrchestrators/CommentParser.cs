using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal static class CommentParser
{
    private static readonly Regex Pattern = new("^\\s*\\/\\/\\s*Stryker", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(20));
    private static readonly Regex Parser = new("^\\s*\\/\\/\\s*Stryker\\s*(disable|restore)\\s*(once|)\\s*([^:]*)\\s*:?(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(20));
    private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger("CommentParser");

    private static MutationContext ParseStrykerComment(MutationContext context, Match match, SyntaxNode node)
    {
        const int ModeGroup = 1;
        const int OnceGroup = 2;
        const int MutatorsGroup = 3;
        const int CommentGroup = 4;

        // get the ignore comment
        var comment = match.Groups[CommentGroup].Value.Trim();
        if (string.IsNullOrEmpty(comment))
        {
            comment = "Ignored via code comment.";
        }

        var disable = match.Groups[ModeGroup].Value.ToLower() switch
        {
            "disable" => true,
            _ => false,
        };
            
        Mutator[] filteredMutators;
        if (match.Groups[MutatorsGroup].Value.ToLower().Trim() == "all")
        {
            filteredMutators = Enum.GetValues<Mutator>();
        }
        else
        {
            var labels = match.Groups[MutatorsGroup].Value.ToLower().Split(',');
            filteredMutators = new Mutator[labels.Length];
            for (var i = 0; i < labels.Length; i++)
            {
                if (Enum.TryParse<Mutator>(labels[i], true, out var value))
                {
                    filteredMutators[i] = value;
                }
                else
                {
                    Logger.LogError(
                        $"{labels[i]} not recognized as a mutator at {node.GetLocation().GetMappedLineSpan().StartLinePosition}, {node.SyntaxTree.FilePath}. Legal values are {string.Join(',', Enum.GetValues<Mutator>())}.");
                }
            }
        }

        return context.FilterMutators(disable, filteredMutators, match.Groups[OnceGroup].Value.ToLower() == "once", comment);
    }

    public static MutationContext ParseNodeComments(SyntaxNode node, MutationContext context)
    {
        foreach (var commentTrivia in node.GetLeadingTrivia()
                     .Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                                 t.IsKind(SyntaxKind.MultiLineCommentTrivia)).Select(t => t.ToString()))
        {
            // perform a quick pattern check to see if it is a 'Stryker comment'
            if (!Pattern.Match(commentTrivia).Success)
            {
                continue;
            }

            var match = Parser.Match(commentTrivia);
            if (match.Success)
            {
                // this is a Stryker comments, now we parse it
                context = ParseStrykerComment(context, match, node);
                break;
            }

            Logger.LogWarning(
                $"Invalid Stryker comments at {node.GetLocation().GetMappedLineSpan().StartLinePosition}, {node.SyntaxTree.FilePath}.");
        }

        return context;
    }
}
