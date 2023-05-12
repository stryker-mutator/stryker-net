namespace Stryker.Core.Options.Inputs;
using System.IO;
using System.IO.Abstractions;
using Stryker.Core.Exceptions;

public class BasePathInput : Input<string>
{
    protected override string Description => "The path from which stryker is started.";

    public override string Default => null;

    public string Validate(IFileSystem fileSystem)
    {
        if (string.IsNullOrWhiteSpace(SuppliedInput))
        {
            throw new InputException("Base path can't be null or empty.");
        }

        if (!fileSystem.Directory.Exists(SuppliedInput)) // validate base path is valid path
        {
            throw new InputException("Base path must exist.");
        }

        return SuppliedInput;
    }
}
