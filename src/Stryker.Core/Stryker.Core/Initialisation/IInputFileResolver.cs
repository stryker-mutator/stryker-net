using Stryker.Core.MutationTest;

namespace Stryker.Core.Initialisation
{
    public interface IInputFileResolver
    {
        ProjectInfo ResolveInput(string currentDirectory, string projectUnderTestNameFilter);
    }
}