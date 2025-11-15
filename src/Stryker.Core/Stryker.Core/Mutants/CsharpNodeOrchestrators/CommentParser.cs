using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Core.Mutants;
using Stryker.Utilities.Logging;

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
                        "{Label} not recognized as a mutator at {0}, {1}. Legal values are {2}.",
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

        switch (processedComments.Count)
        {
            case 0:
                return context;
            case > 1:
            {
                var errorMessage = new StringBuilder().Append(
                    $"Multiple Stryker comments at {node.GetLocation().GetMappedLineSpan().StartLinePosition}, {node.SyntaxTree.FilePath}. Only the first one will be used");
                errorMessage.Append(string.Join(Environment.NewLine, processedComments.Select(c => c.Item2.ToString())));
                Logger.LogWarning(errorMessage.ToString());
                break;
            }
        }

        return processedComments[0].Item1;
    }

    [ExcludeFromCodeCoverage(Justification = "Difficult to test timeouts")]
    private static MutationContext ProcessComment(SyntaxNode node, MutationContext context, string commentTrivia)
    {
        try
        {
            return InterpretStrykerComment(node, context, commentTrivia);
        }
        catch (TimeoutException exception)
        {
            Logger.LogWarning(
                "Parsing Stryker comments at {0}, {1} took too long to parse and was ignored ({2}). Comment: {3}",
                node.GetLocation().GetMappedLineSpan().StartLinePosition,
                node.SyntaxTree.FilePath, exception.Message, commentTrivia);
            return null;
        }
    }

    private static MutationContext InterpretStrykerComment(SyntaxNode node, MutationContext context, string commentTrivia)
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
            "Invalid Stryker comments at {0}, {1}.",
            node.GetLocation().GetMappedLineSpan().StartLinePosition,
            node.SyntaxTree.FilePath);
        return null;
    }
}
