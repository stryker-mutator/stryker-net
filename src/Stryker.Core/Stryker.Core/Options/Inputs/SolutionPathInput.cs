using Stryker.Core.Exceptions;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Inputs
{
    public class SolutionPathInput : InputDefinition<string>
    {
        protected override string Description => "Full path to your solution file. Required on dotnet framework.";
        protected override string HelpOptions => "";

        public SolutionPathInput() { }
        public SolutionPathInput(IFileSystem fileSystem, string solutionPath)
        {
            if (solutionPath is { })
            {
                if (!fileSystem.File.Exists(solutionPath))  //validate file existance and maintain moq
                {
                    throw new StrykerInputException("Given solution path does not exist: {0}", solutionPath);
                }

                Value = solutionPath;
            }
        }
    }
}
