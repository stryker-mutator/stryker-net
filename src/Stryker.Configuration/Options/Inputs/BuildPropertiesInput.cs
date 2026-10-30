
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
        var result = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
        if (SuppliedInput == null)
        {
            return result;
        }
        foreach (var block in SuppliedInput)
        {
            var inName = true;
            var name = string.Empty;
            var value = string.Empty;
            var inQuotes = false;
            foreach (var car in block)
            {
                if (car=='"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }
                if (inQuotes)
                {
                    if (inName)
                    {
                        name += car;
                    }
                    else
                    {
                        value += car;
                    }
                }
                else
                {
                    if (car=='=')
                    {
                        if (!inName)
                        {
                            throw new InputException($"Invalid build property format: '{block}'. Expected format is 'key=value'.");
                        }
                        inName = false;
                        continue;
                    }
                    if (car==';')
                    {
                        if (inName)
                        {
                            throw new InputException($"Invalid build property format: '{block}'. Expected format is 'key=value'.");
                        }
                        result[name.TrimQuotes()] = value.TrimQuotes();
                        name = string.Empty;
                        value = string.Empty;
                        inName = true;
                    }
                    else
                    {
                        if (inName)
                        {
                            name += car;
                        }
                        else
                        {
                            value += car;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                result[name.TrimQuotes()] = value.TrimQuotes();
            }
        }
        return result;
    }
}
