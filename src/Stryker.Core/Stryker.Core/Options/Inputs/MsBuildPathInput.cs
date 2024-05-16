using System.IO.Abstractions;
using Stryker.Shared.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class MsBuildPathInput : Input<string>
{
    public override string Default => null;
    protected override string Description => "The path to the msbuild executable to use to build your .NET Framework application. Not used for .net (core).";

    public string Validate(IFileSystem fileSystem)
    {
        if (SuppliedInput is not null)
        {
            if(string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("MsBuild path cannot be empty. Either provide a valid msbuild path or let stryker locate msbuild automatically.");
            }
            if(!fileSystem.File.Exists(SuppliedInput))
            {
                throw new InputException($"Given MsBuild path '{SuppliedInput}' does not exist. Either provide a valid msbuild path or let stryker locate msbuild automatically.");
            }
            return SuppliedInput;
        }
        return Default;
    }
}
