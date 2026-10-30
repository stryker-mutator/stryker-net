using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Utilities;
using Stryker.Utilities.Logging;

namespace Stryker.Configuration.Options.Inputs;

public class MutateInput : Input<IEnumerable<string>>
{
    private readonly string _defaultInput = "**/*";
    public override IEnumerable<string> Default => new List<string> { _defaultInput };

    protected override string Description => @"Allows to specify file that should in- or excluded for the mutations.
    Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
    Use '!' at the start of a pattern to exclude all matched files.
    Use '{<start>..<end>}' at the end of a pattern to specify spans of text in files to in- or exclude.
    Example: ['**/*Service.cs','!**/MySpecialService.cs', '**/MyOtherService.cs{1..10}{32..45}']";

    public IEnumerable<IFilePattern> Validate(ILogger<MutateInput> logger = null)
    {
        logger ??= ApplicationLogging.LoggerFactory.CreateLogger<MutateInput>();

        var effectivePatternStrings = GetEffectivePatternStrings().ToList();

        logger.LogInformation("File mutation patterns: {MutatePatterns}.", string.Join(", ", effectivePatternStrings));

        return BuildFilePatterns();
    }

    private IEnumerable<string> GetEffectivePatternStrings()
    {
        if (SuppliedInput is { } && SuppliedInput.Any())
        {
            if (SuppliedInput.All(p => p.StartsWith('!')))
            {
                return SuppliedInput.Concat(new[] { _defaultInput });
            }

            return SuppliedInput;
        }

        return Default;
    }

    private IEnumerable<IFilePattern> BuildFilePatterns()
    {
        if (SuppliedInput is { } && SuppliedInput.Any())
        {
            var filesToInclude = new List<IFilePattern>();

            foreach (var pattern in SuppliedInput)
            {
                filesToInclude.Add(FilePattern.Parse(FilePathUtils.NormalizePathSeparators(pattern)));
            }

            if (filesToInclude.TrueForAll(f => f.IsExclude))
            {
                // If there are only exclude patterns, we add a pattern that matches every file.
                filesToInclude.Add(FilePattern.Parse(_defaultInput));
            }

            return filesToInclude;
        }
        return Default.Select(FilePattern.Parse).ToList();
    }
}
