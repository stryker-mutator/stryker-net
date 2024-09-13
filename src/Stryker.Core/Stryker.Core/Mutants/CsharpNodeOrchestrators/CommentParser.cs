using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal static class CommentParser
{
    private static readonly Regex Pattern = new("^\\s*(\\/\\/\\s*Stryker(.*))|(\\/\\*\\s*Stryker(.*[^\\*][^\\\\])\\*\\/\\s*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200));
    private static readonly Regex Parser = new("^\\s*(disable|restore)\\s*(once|)\\s*([^:]*)\\s*:?(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200));
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
                        "{Label} not recognized as a mutator at {Location}, {FilePath}. Legal values are {LegalValues}.",
                        labels[i],
                        node.GetLocation().GetMappedLineSpan().StartLinePosition,
                        node.SyntaxTree.FilePath,
                        string.Join(',', Enum.GetValues<Mutator>()));
                }
            }
        }

        return context.FilterMutators(disable, filteredMutators, match.Groups[OnceGroup].Value.ToLower() == "once", comment);
    }

    public static MutationContext ParseNodeLeadingComments(SyntaxNode node, MutationContext context)
    {
        var processedComments = node.GetFirstToken(true).GetPreviousToken(true).TrailingTrivia
            .Union(node.GetLeadingTrivia())
            .Where(t => t.IsKind(SyntaxKind.MultiLineCommentTrivia) || t.IsKind(SyntaxKind.SingleLineCommentTrivia))
            .Select(t => (ProcessComment(node, context, t.ToString()), t)).Where(t => t.Item1 != null).ToList();

        if (processedComments.Count == 0)
        {
            return context;
        }
        if (processedComments.Count > 1)
        {
            var errorMessage = new StringBuilder().Append(  
                $"Multiple Stryker comments at {node.GetLocation().GetMappedLineSpan().StartLinePosition}, {node.SyntaxTree.FilePath}. Only the first one will be used");
            errorMessage.Append(string.Join(Environment.NewLine, processedComments.Select(c => c.Item2.ToString())));
            Logger.LogWarning(errorMessage.ToString());

        }
        return processedComments[0].Item1;
    }

    private static MutationContext ProcessComment(SyntaxNode node, MutationContext context, string commentTrivia)
    {
        // perform a quick pattern check to see if it is a 'Stryker comment'
        var strykerCommentMatch = Pattern.Match(commentTrivia);
        if (!strykerCommentMatch.Success)
        {
            return null;
        }

        // now we can extract actual command
        var isSingleLine = strykerCommentMatch.Groups[1].Success;
        var command = isSingleLine ? strykerCommentMatch.Groups[2].Value : strykerCommentMatch.Groups[4].Value;

        var match = Parser.Match(command);
        if (match.Success)
        {
            // this is a Stryker comments, now we parse it
            return ParseStrykerComment(context, match, node);
        }

        Logger.LogWarning(
            "Invalid Stryker comments at {Position}, {FilePath}.",
            node.GetLocation().GetMappedLineSpan().StartLinePosition,
            node.SyntaxTree.FilePath);
        return null;
    }
}
