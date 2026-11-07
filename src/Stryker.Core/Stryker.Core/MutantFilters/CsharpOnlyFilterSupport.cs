using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Utilities.Logging;

namespace Stryker.Core.MutantFilters;

/// <summary>
/// Shared behaviour of filters that walk Roslyn trees: mutants that are not C# mutations
/// are never filtered, and the first one met is logged once per filter.
/// </summary>
internal sealed class CsharpOnlyFilterSupport(string filterName)
{
    private readonly ILogger _logger = ApplicationLogging.LoggerFactory.CreateLogger<CsharpOnlyFilterSupport>();
    private bool _hasLoggedSkip;

    /// <summary>
    /// Keeps every mutant that is not a <see cref="CsharpMutation"/>, and the C# mutants for which <paramref name="isExcluded"/> is false.
    /// </summary>
    public IEnumerable<IMutant> Filter(IEnumerable<IMutant> mutants, System.Func<IMutant, CsharpMutation, bool> isExcluded) =>
        mutants.Where(mutant => mutant.Mutation is not CsharpMutation csharp
            ? KeepNonCsharp()
            : !isExcluded(mutant, csharp));

    private bool KeepNonCsharp()
    {
        if (!_hasLoggedSkip)
        {
            _hasLoggedSkip = true;
            _logger.LogDebug("The {Filter} only applies to C# mutations; other mutants are not filtered.", filterName);
        }

        return true;
    }
}
