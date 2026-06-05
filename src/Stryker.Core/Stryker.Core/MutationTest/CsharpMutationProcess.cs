using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Configuration;
using Stryker.Core.Compiling;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.MutationTest;

public class CsharpMutationProcess : IMutationProcess
{
    private IStrykerOptions _options;
    private IMutantFilter _mutantFilter;
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;

    public CsharpMutationProcess(
        IFileSystem fileSystem,
        ILogger<CsharpMutationProcess> logger)
    {
        _fileSystem = fileSystem ?? new FileSystem();
        _logger = logger;
    }

    /// <inheritdoc/>
    public void Mutate(MutationTestInput input, IStrykerOptions options)
    {
        _options = options;
        var projectInfo = input.SourceProjectInfo.ProjectContents;
        var compilingProcess = new CsharpCompilingProcess(input, options: _options);
        var files = projectInfo.GetAllFiles().Cast<CsharpFileLeaf>().ToList();
        var semanticModels = compilingProcess.GetSemanticModels(files.Select(x => x.SyntaxTree)).ToList();

        var semanticModelLookup = semanticModels.ToDictionary(sm => sm.SyntaxTree);

        _options.MutantIdProvider ??= new BasicIdProvider();

        var parallelOptions = new ParallelOptions
        {
            // Use configured concurrency when provided; otherwise, derive a safe default
            MaxDegreeOfParallelism = options.Concurrency > 0
                ? options.Concurrency
                : Math.Max(1, Environment.ProcessorCount - 1)
        };

        Parallel.ForEach(files, parallelOptions, file =>
        {
            _logger.LogDebug("Mutating {FilePath}", file.FullPath);

            var placer = new MutantPlacer(input.SourceProjectInfo.CodeInjector);
            var orchestrator = new CsharpMutantOrchestrator(placer, options: _options);

            var mutatedSyntaxTree = orchestrator.Mutate(file.SyntaxTree, semanticModelLookup[file.SyntaxTree]);
            file.MutatedSyntaxTree = mutatedSyntaxTree;

            if (_options.DiagMode)
            {
                _logger.LogTrace("Mutated {FullPath}:{NewLine}{MutatedSyntaxTree}",
                    file.FullPath, Environment.NewLine, mutatedSyntaxTree.GetText());
            }

            file.Mutants = orchestrator.GetLatestMutantBatch();
        });

        _logger.LogDebug("{MutantsCount} mutants created", projectInfo.Mutants.Count());
    }

    /// <inheritdoc/>
    public void Compile(MutationTestInput input)
    {
        var compilingProcess = new CsharpCompilingProcess(input, options: _options);
        CompileMutations(input, compilingProcess);
    }

    private void CompileMutations(MutationTestInput input, CsharpCompilingProcess compilingProcess)
    {
        var info = input.SourceProjectInfo;
        var projectInfo = (ProjectComponent<SyntaxTree>)info.ProjectContents;
        using var ms = new MemoryStream();
        using var msForSymbols = _options.DiagMode ? new MemoryStream() : null;
        // compile the mutated syntax trees
        var compileResult = compilingProcess.Compile(projectInfo.CompilationSyntaxTrees, ms, msForSymbols);

        foreach (var testProject in info.TestProjectsInfo.AnalyzerResults)
        {
            var injectionPath = TestProjectsInfo.GetInjectionFilePath(testProject, input.SourceProjectInfo.AnalyzerResult);
            if (!_fileSystem.Directory.Exists(testProject.GetAssemblyDirectoryPath()))
            {
                _fileSystem.Directory.CreateDirectory(testProject.GetAssemblyDirectoryPath());
            }

            // inject the mutated Assembly into the test project
            using var fs = _fileSystem.File.Create(injectionPath);
            ms.Position = 0;
            ms.CopyTo(fs);

            if (msForSymbols != null)
            {
                // inject the debug symbols into the test project
                using var symbolDestination = _fileSystem.File.Create(Path.Combine(testProject.GetAssemblyDirectoryPath(), input.SourceProjectInfo.AnalyzerResult.GetSymbolFileName()));
                msForSymbols.Position = 0;
                msForSymbols.CopyTo(symbolDestination);
            }

            _logger.LogDebug("Injected the mutated assembly file into {InjectionPath}", injectionPath);
        }

        // if a rollback took place, mark the rolled back mutants as status:BuildError
        if (compileResult.RollbackedIds.Any())
        {
            foreach (var mutant in projectInfo.Mutants
                .Where(x => compileResult.RollbackedIds.Contains(x.Id)))
            {
                // Ignore compilation errors if the mutation is skipped anyways.
                if (mutant.ResultStatus == MutantStatus.Ignored)
                {
                    continue;
                }

                mutant.ResultStatus = MutantStatus.CompileError;
                mutant.ResultStatusReason = "Mutant caused compile errors";
            }
        }
    }

    public void FilterMutants(MutationTestInput input)
    {
        _mutantFilter ??= MutantFilterFactory.Create(_options, input);
        foreach (var file in input.SourceProjectInfo.ProjectContents.GetAllFiles())
        {
            // CompileError is a final status and can not be changed during filtering.
            var mutantsToFilter = file.Mutants.Where(x => x.ResultStatus != MutantStatus.CompileError);
            _mutantFilter.FilterMutants(mutantsToFilter, file, _options);
        }
    }
}
