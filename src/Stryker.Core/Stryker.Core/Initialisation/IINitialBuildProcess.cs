namespace Stryker.Core.Initialisation
{
    public interface IInitialBuildProcess
    {
        void InitialBuild(string path, string projectName);
    }
}