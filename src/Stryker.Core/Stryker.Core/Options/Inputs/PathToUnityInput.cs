using System.IO;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class PathToUnityInput : Input<string>
{
    public override string Default => null;

    protected override string Description => "Override path to Unity instance for running tests";

    public string Validate()
    {
        if (SuppliedInput is not null)
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("Path to unity cannot be empty");
            }

            if (!File.Exists(SuppliedInput))
            {
                throw new InputException($"File on this path doesn't exist '{SuppliedInput}'");
            }
            return SuppliedInput;
        }

        return Default;
    }
}
