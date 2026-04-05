using System;
using System.Collections.Generic;
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
internal class MutableProjectTree(ProjectAnalyzerContext project, ILogger logger)
{
    private readonly ILogger _logger = logger;

    public ProjectAnalyzerContext Project { get; } = project;

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
            var newTarget = new MutableProjectTarget(target, _logger);
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
            _logger.LogWarning("Failed to find a valid {Framework} target for project {Project}. ", optionsTargetFramework, Project.ProjectFileName);
        }

        targetToKeep = Targets.Where(t => t.IsValidTarget).FirstOrDefault( t => OperatingSystem.IsWindows() || !t.ProjectTarget.TargetsFullFramework());
        Targets.Clear();
        if (targetToKeep == null)
        {
            _logger.LogWarning("Failed to find a valid target for project {Project}. ", Project.ProjectFileName);
            return;
        }
        _logger.LogInformation("Picking {Framework} for project {Project}. ", targetToKeep.ProjectTarget.TargetFramework, Project.ProjectFileName);
        Targets.Add(targetToKeep);
    }

    public void DumpForAnalysis()
    {
        _logger.LogInformation("Project {ProjectPath} overall analysis {Result}.",
            Project.ProjectFileName,
            IsValidTarget ? "succeeded" : "failed hence can't be mutated");
        foreach (var target in Targets)
        {
            target.DumpForAnalysis();
        }
    }
}
