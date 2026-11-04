using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Core.MutationTest;
using Stryker.Utilities.Buildalyzer;
using Stryker.Fsharp.Mutants;
using Stryker.Fsharp.ProjectComponents;

namespace Stryker.Fsharp;

/// <summary>F# mutation pipeline: orchestrate, splice, compile with the helper assembly, inject into the test outputs.</summary>
public class FsharpMutationProcess(IFileSystem fileSystem, ILogger<FsharpMutationProcess> logger) : IMutationProcess
{
    public void Mutate(MutationTestInput input, IStrykerOptions options)
    {
        var info = input.SourceProjectInfo;
        var leaves = info.ProjectContents.GetAllFiles().Cast<FsharpFileLeaf>().ToList();
        var orchestrator = new FsharpMutantOrchestrator(info.HelperNamespace, null, options.MutationLevel);
        foreach (var leaf in leaves)
        {
            logger.LogDebug("Mutating {FilePath}", leaf.FullPath);
            orchestrator.Mutate(leaf);
        }

        logger.LogDebug("{MutantsCount} mutants created", leaves.Sum(l => l.Mutants.Count()));

        var helperBytes = FsharpHelperAssembly.Compile(info.CodeInjector, info.AnalyzerResult);
        var helperPath = WriteHelperToTemp(info.CodeInjector, helperBytes);
        try
        {
            var compiler = new FsharpCompilingProcess(info.AnalyzerResult, fileSystem, logger);
            var rollback = FsharpRollback.Run(leaves.Where(l => l.Splices.Count > 0).ToList(), info.HelperNamespace,
                overrides => compiler.Compile(overrides, [helperPath]));
            MarkBuildErrors(leaves, rollback.RolledBackIds);
            logger.LogDebug("{RolledBack} mutants rolled back after {Attempts} compilation attempts", rollback.RolledBackIds.Count, rollback.Attempts);
            var result = rollback.Compiled;
            compiler.Inject(info.TestProjectsInfo, result.Assembly);
            var helperFileName = FsharpHelperAssembly.FileName(info.CodeInjector);
            compiler.InjectFile(info.TestProjectsInfo, helperFileName, helperBytes);
            foreach (var testProject in info.TestProjectsInfo.AnalyzerResults)
            {
                FsharpHelperAssembly.RegisterInDependencyManifests(fileSystem, testProject.GetAssemblyDirectoryPath(), helperFileName);
            }
        }
        finally
        {
            fileSystem.File.Delete(helperPath);
        }
    }

    /// <summary>Rolled-back mutants become CompileError, as in the C# process, unless already ignored.</summary>
    private static void MarkBuildErrors(IEnumerable<FsharpFileLeaf> leaves, IReadOnlyCollection<int> rolledBackIds)
    {
        foreach (var mutant in leaves.SelectMany(l => l.Mutants).Where(m => rolledBackIds.Contains(m.Id)))
        {
            if (mutant.ResultStatus == MutantStatus.Ignored)
            {
                continue;
            }

            mutant.ResultStatus = MutantStatus.CompileError;
            mutant.ResultStatusReason = "Mutant caused compile errors";
        }
    }

    // TODO(agt): apply MutantFilterFactory filters; the C# filters still assume Roslyn nodes, so mutants stay unfiltered for now.
    public void FilterMutants(MutationTestInput input)
    {
    }

    private string WriteHelperToTemp(Core.InjectedHelpers.CodeInjection injector, byte[] bytes)
    {
        var path = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString("N") + "-" + FsharpHelperAssembly.FileName(injector));
        fileSystem.File.WriteAllBytes(path, bytes);
        return path;
    }
}
