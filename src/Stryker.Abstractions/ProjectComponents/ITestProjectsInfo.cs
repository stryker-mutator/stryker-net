using Buildalyzer;

namespace Stryker.Abstractions.ProjectComponents;

public interface ITestProjectsInfo
{
    IEnumerable<IAnalyzerResult> AnalyzerResults { get; }
    IEnumerable<ITestFile> TestFiles { get; }
    IEnumerable<ITestProject> TestProjects { get; set; }

    void BackupOriginalAssembly(IAnalyzerResult sourceProject);
    IReadOnlyList<string> GetTestAssemblies();
    void RestoreOriginalAssembly(IAnalyzerResult sourceProject);
}
