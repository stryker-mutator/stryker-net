namespace Stryker.Shared.Initialisation;
public interface IProjectAndTests
{
    bool IsFullFramework { get; }
    string HelperNamespace { get; }
    IReadOnlyList<string> GetTestAssemblies();
}
