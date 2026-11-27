using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Configuration.Options.Inputs;

public class IgnoreMethodsInput : Input<IEnumerable<string>>
{
    private const string ArgumentsOnlySuffix = "(*)";

    public override IEnumerable<string> Default => Enumerable.Empty<string>();

    protected override string Description => @"Ignore mutations in calls to the given methods. Add (*) to a method name to ignore only the mutations in the arguments of its calls.";

    /// <summary>
    /// The patterns without the (*) suffix: the whole call is ignored.
    /// </summary>
    public IEnumerable<Regex> Validate() =>
        ParseRegex(ValidatedPatterns().Where(pattern => !IsArgumentsOnly(pattern)));

    /// <summary>
    /// The patterns with the (*) suffix, stripped of it: only the arguments of the call are ignored.
    /// </summary>
    public IEnumerable<Regex> ValidateArgumentsOnly() =>
        ParseRegex(ValidatedPatterns().Where(IsArgumentsOnly).Select(pattern => pattern[..^ArgumentsOnlySuffix.Length]));

    private IEnumerable<string> ValidatedPatterns()
    {
        var patterns = (SuppliedInput ?? Default).Where(pattern => !string.IsNullOrEmpty(pattern)).ToList();

        // Any other parenthesised form is rejected, so that it stays available for a later syntax.
        var invalidPattern = patterns.Find(pattern => !HasValidName(pattern));
        if (invalidPattern is not null)
        {
            throw new InputException($"Invalid ignore-methods pattern ({invalidPattern}). Use a method name, optionally followed by {ArgumentsOnlySuffix} to ignore only the mutations in the arguments of its calls, for example ConfigureAwait{ArgumentsOnlySuffix}.");
        }

        return patterns;
    }

    private static bool IsArgumentsOnly(string pattern) => pattern.EndsWith(ArgumentsOnlySuffix);

    private static bool HasValidName(string pattern)
    {
        var methodName = IsArgumentsOnly(pattern) ? pattern[..^ArgumentsOnlySuffix.Length] : pattern;
        return methodName.Length > 0 && methodName.IndexOfAny(['(', ')']) < 0;
    }

    private static IEnumerable<Regex> ParseRegex(IEnumerable<string> methodPatterns) =>
        methodPatterns
            .Select(methodPattern => new Regex("^(?:[^.]*\\.)*" + Regex.Escape(methodPattern).Replace("\\*", "[^.]*") + "(<[^>]*>)?$", RegexOptions.IgnoreCase))
            .ToList();
}
