using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

/// <summary>
/// This class is used to keep track of the project and its targets during the initialization process.
/// It allows us to keep track of the targets that are valid for mutation and to keep track of the project analyzer context.
/// </summary>
/// <param name="project"></param>
internal class MutableProjectTree(ProjectSimulatedBuildHandler project, ILogger logger)
{
    public List<MutableProjectTarget> Targets { get; } = [];

    public bool IsValidTarget => Targets.Any(t => t.IsValidTarget);

    public MutableProjectTarget this[IAnalyzerResult target]
    {
        get
        {
            var existingTarget = Targets.FirstOrDefault(t => t.ProjectTarget == target);
            if (existingTarget != null)
            {
                return existingTarget;
            }
            var newTarget = new MutableProjectTarget(target, logger);
            Targets.Add(newTarget);
            return newTarget;
        }
    }

    /// <summary>
    /// Keep a single target for mutation when analysis gave several results
    /// </summary>
    /// <param name="optionsTargetFramework"></param>
    public void KeepOnlyOneTarget(string optionsTargetFramework)
    {
        MutableProjectTarget targetToKeep;
        // look for a specified target framework if any
        if (!string.IsNullOrEmpty(optionsTargetFramework))
        {
            // try to find a valid analysis for requested framework
            targetToKeep = Targets.FirstOrDefault(t => t.ProjectTarget.TargetFramework == optionsTargetFramework && t.IsValidTarget);
            // then try to find a valid test project for the requested framework
            targetToKeep ??= Targets.FirstOrDefault(t => t.TestProjects.Any( tp => tp.TargetFramework == optionsTargetFramework && tp.IsValid()));
            if (targetToKeep != null)
            {
                Targets.Clear();
                Targets.Add(targetToKeep);
                return;
            }
            logger.LogWarning("Failed to find a valid {Framework} target for project {Project}. ", optionsTargetFramework, project.ProjectFileName);
        }

        var mutableProjectTargets = Targets.Where(t => t.IsValidTarget);
        // on non windows platform, keep first non netframework target
        // on Windows, keep first netframework target if any, otherwise keep first valid target
        targetToKeep = mutableProjectTargets.FirstOrDefault( t => OperatingSystem.IsWindows() == t.ProjectTarget.TargetsFullFramework()) ??
                       mutableProjectTargets.FirstOrDefault();
        Targets.Clear();
        if (targetToKeep == null)
        {
            logger.LogWarning("Failed to find a valid target for project {Project}. ", project.ProjectFileName);
            return;
        }
        logger.LogInformation("Picking {Framework} for project {Project}. ", targetToKeep.ProjectTarget.TargetFramework, project.ProjectFileName);
        Targets.Add(targetToKeep);
    }

    public void DumpForAnalysis()
    {
        logger.LogInformation("Project {ProjectPath} overall analysis {Result}.",
            Path.GetFileName(project.ProjectFileName),
            IsValidTarget ? "succeeded" : "failed hence can't be mutated");
        foreach (var target in Targets)
        {
            target.DumpForAnalysis();
        }
    }
}
