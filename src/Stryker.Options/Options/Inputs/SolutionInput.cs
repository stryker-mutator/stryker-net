using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Abstractions.Options.Inputs;

public class SolutionInput : Input<string>
{
    public override string Default => null;

    protected override string Description => "Full path to your solution file. Required on dotnet framework.";

    private readonly string[] _validSuffixes = [".sln", ".slnx"];

    public string? Validate(string basePath, IFileSystem fileSystem)
    {
        if (SuppliedInput is not null)
        {
            var lowerInvariant = SuppliedInput.ToLowerInvariant();
            if (!_validSuffixes.Any(s => lowerInvariant.EndsWith(s)))
            {
                throw new InputException($"Given path is not a solution file: {SuppliedInput}");
            }
            var fullPath = fileSystem.Path.GetFullPath(SuppliedInput);
            if (!fileSystem.File.Exists(fullPath))
            {
                throw new InputException($"Given path does not exist: {SuppliedInput}");
            }

            return fullPath;
        }

        var solutionFiles = fileSystem.Directory.GetFiles(basePath, "*.*")
            .Where(file => _validSuffixes.Any(s => file.ToLowerInvariant().EndsWith(s))).ToArray();
        if (solutionFiles.Length <= 1)
        {
            return solutionFiles.FirstOrDefault();
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Expected exactly one .sln? file, found more than one:");
        foreach (var file in solutionFiles)
        {
            sb.AppendLine(file);
        }
        throw new InputException(sb.ToString());
    }
}
