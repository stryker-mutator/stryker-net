using System.IO.Abstractions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class OutputBasePathInput : Input<string>
{
    public override string Default => null;
    protected override string Description => @"Changes the default output path for logs and reports. This can be a relative or absolute path.
By default, the output is stored in StrykerOutput/ in de working directory.";
}
