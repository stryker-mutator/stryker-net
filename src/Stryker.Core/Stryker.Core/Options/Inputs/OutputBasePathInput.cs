using System.IO.Abstractions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class OutputBasePathInput : Input<string>
{
    public override string Default => null;
    protected override string Description => @"Changes the default output path for logs and reports. This can be a relative or absolute path.
By default, the output is stored in StrykerOutput/ in de working directory.";

    public string Validate(IFileSystem fileSystem)
    {
        if (string.IsNullOrWhiteSpace(SuppliedInput))
        {
            throw new InputException("Outputpath can't be null or whitespace");
        }
        if (!fileSystem.Directory.Exists(SuppliedInput))
        {
            throw new InputException("Outputpath should exist");
        }
        return SuppliedInput;
    }
}
