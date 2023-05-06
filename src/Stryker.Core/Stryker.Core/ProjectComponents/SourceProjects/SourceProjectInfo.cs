using Buildalyzer;

namespace Stryker.Core.ProjectComponents.SourceProjects;

public class SourceProjectInfo
{
    public IAnalyzerResult AnalyzerResult { get; set; }

    /// <summary>
    /// The Folder/File structure found in the project under test.
    /// </summary>
    public IProjectComponent ProjectContents { get; set; }
}
