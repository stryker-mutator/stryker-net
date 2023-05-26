using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class SolutionInput : Input<string>
    {
        public override string Default => null;

        protected override string Description => "Full path to your solution file. Required on dotnet framework.";

        public string Validate(string basePath, IFileSystem fileSystem)
        {
            if (SuppliedInput is not null)
            {
                if (!SuppliedInput.EndsWith(".sln"))
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
            else
            {
                var solutionFiles = fileSystem.Directory.GetFiles(basePath, "*.*").Where(file => file.EndsWith("sln")).ToArray();
                if (solutionFiles.Count() > 1)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"Expected exactly one .sln file, found more than one:");
                    foreach (var file in solutionFiles)
                    {
                        sb.AppendLine(file);
                    }
                    throw new InputException(sb.ToString());
                }
                return solutionFiles.FirstOrDefault();
            }
        }
    }
}
