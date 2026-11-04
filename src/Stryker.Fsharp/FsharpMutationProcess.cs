using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
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
        var orchestrator = new FsharpMutantOrchestrator(info.HelperNamespace);
        foreach (var leaf in leaves)
        {
            logger.LogDebug("Mutating {FilePath}", leaf.FullPath);
            orchestrator.Mutate(leaf);
        }

        logger.LogDebug("{MutantsCount} mutants created", leaves.Sum(l => l.Mutants.Count()));

        var overrides = leaves.Where(l => l.Splices.Count > 0).ToDictionary(
            l => l.FullPath,
            l => FsharpMutantOrchestrator.ApplySplices(l.SourceCode, l.Splices, info.HelperNamespace));

        var helperBytes = FsharpHelperAssembly.Compile(info.CodeInjector, info.AnalyzerResult);
        var helperPath = WriteHelperToTemp(info.CodeInjector, helperBytes);
        try
        {
            var compiler = new FsharpCompilingProcess(info.AnalyzerResult, fileSystem, logger);
            var result = compiler.Compile(overrides, [helperPath]);
            if (!result.Success)
            {
                // TODO(agt): roll back mutants that break compilation (task 9: compile-error rollback)
                var errors = string.Join(Environment.NewLine, result.Diagnostics.Where(d => d.Severity.IsError).Select(d => d.ToString()));
                throw new GeneralStrykerException($"The mutated F# project failed to compile:{Environment.NewLine}{errors}");
            }

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
