using System.Collections.Generic;
using Stryker.Core.Initialisation;

namespace Stryker.Core.ProjectComponents.SourceProjects;

/// <summary>
/// Simple class aggregating mutable projects and the associated overarching context: platform/framework... settings
/// and solution file when relevant
/// </summary>
public class RelatedSourceProjectsInfo
{
    public IReadOnlyCollection<SourceProjectInfo> SourceProjectInfos { get; }
    public ProjectsTracker Tracker { get; }

    public RelatedSourceProjectsInfo(ProjectsTracker projectsTracker, IReadOnlyCollection<SourceProjectInfo> sourceProjectInfos)
    {
        Tracker = projectsTracker;
        SourceProjectInfos = sourceProjectInfos;
    }
}
