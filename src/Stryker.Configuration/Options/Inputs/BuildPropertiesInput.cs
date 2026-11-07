
using System.Collections.Generic;
using System.Text;
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
            var index = 0;
            var parts = new StringBuilder[2];
            parts[0] = new StringBuilder();
            parts[1] = new StringBuilder();
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
                    parts[index].Append(car);
                    continue;
                }

                switch (car)
                {
                    case '=' when index != 0:
                        throw new InputException($"Invalid build property format: '{block}'. Expected format is 'key=value'.");
                    case '=':
                        index = 1;
                        continue;
                    case ';':
                        result[parts[0].ToString()] = parts[1].ToString();
                        index = 0;
                        parts[0].Clear();
                        parts[1].Clear();
                        break;
                    default:
                        parts[index].Append(car);
                        break;
                }
            }
            if (parts[0].Length > 0)
            {
                result[parts[0].ToString()] = parts[1].ToString();
            }
        }
        return result;
    }
}
