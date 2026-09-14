using System.Collections.Generic;
using Microsoft.Build.Logging.StructuredLogger;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Configuration.Options.Inputs;

public class BuildPropertiesInput: Input<IEnumerable<string>>
{
    public override IEnumerable<string> Default => [];

    protected override string Description => "Allows to specify build properties that will be passed to MSBuild when building the project. " +
        "Use 'key=value' format for each property";

    public Dictionary<string, string> Validate()
    {
        var result = new Dictionary<string, string>();
        foreach (var input in SuppliedInput ?? Default)
        {
            var parts = input.Split('=', 2);
            if (parts.Length != 2)
            {
                throw new InputException($"Invalid build property format: '{input}'. Expected format is 'key=value'.");
            }
            result[parts[0]] = parts[1].TrimQuotes();
        }
        return result;
    }
}
