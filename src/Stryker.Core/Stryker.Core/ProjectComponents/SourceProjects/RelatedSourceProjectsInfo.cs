using System.Collections.Generic;
using Stryker.Core.Initialisation;

namespace Stryker.Core.ProjectComponents.SourceProjects;

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
