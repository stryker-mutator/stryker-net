using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Buildalyzer;
using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.Text;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.Initialisation;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Fsharp.ProjectComponents;

namespace Stryker.Fsharp;

/// <summary>Builds the F# component tree from the analyzer's compile items, in fsproj order.</summary>
public class FsharpProjectComponentsBuilder(SourceProjectInfo projectInfo, ILogger logger, IFileSystem fileSystem) : ProjectComponentsBuilder(fileSystem)
{
    private static readonly string[] ExcludedExtensions = [".fsi", ".fsx"];

    public override IReadOnlyProjectComponent Build()
    {
        var analyzerResult = projectInfo.AnalyzerResult;
        var projectDir = FileSystem.Path.GetDirectoryName(analyzerResult.ProjectFilePath) ?? string.Empty;
        var root = new FsharpFolderComposite
        {
            FullPath = projectDir,
            RelativePath = FileSystem.Path.GetDirectoryName(projectDir) ?? string.Empty,
        };
        var folders = new Dictionary<string, FsharpFolderComposite> { [string.Empty] = root };
        // Buildalyzer reports project-relative paths
        var sourceFiles = analyzerResult.SourceFiles.Select(f => FileSystem.Path.GetFullPath(f, projectDir)).ToList();
        var parsingOptions = CreateParsingOptions(analyzerResult, sourceFiles);

        // SourceFiles holds the compile items in fsproj order; F# compilation order is semantic.
        foreach (var sourceFile in sourceFiles)
        {
            if (IsGenerated(sourceFile, projectDir))
            {
                // still compiled (the compile step uses the fsc arguments), but never mutated
                logger.LogDebug("Excluding generated F# compile item {File}", sourceFile);
                continue;
            }

            if (ExcludedExtensions.Contains(FileSystem.Path.GetExtension(sourceFile), StringComparer.OrdinalIgnoreCase))
            {
                logger.LogDebug("Excluding unsupported F# compile item {File}", sourceFile);
                continue;
            }

            var sourceCode = FileSystem.File.ReadAllText(sourceFile);
            var parseResults = FSharpAsync.RunSynchronously(
                FsharpCheckerHolder.Instance.ParseFile(sourceFile, SourceText.ofString(sourceCode), parsingOptions, null, null), null, null);
            if (parseResults.ParseHadErrors)
            {
                logger.LogWarning("Skipping F# file {File}: it does not parse ({Errors}).", sourceFile, string.Join("; ",
                    parseResults.Diagnostics.Where(d => d.Severity.IsError).Select(d => d.Message)));
                continue;
            }

            var relativePath = FileSystem.Path.GetRelativePath(projectDir, sourceFile);
            GetFolder(folders, FileSystem.Path.GetDirectoryName(relativePath) ?? string.Empty, projectDir).Add(new FsharpFileLeaf
            {
                SourceCode = sourceCode,
                ParseTree = parseResults.ParseTree,
                FullPath = sourceFile,
                RelativePath = relativePath,
            });
        }

        return root;
    }

    private bool IsGenerated(string sourceFile, string projectDir)
    {
        var relative = FileSystem.Path.GetRelativePath(projectDir, sourceFile).Replace('\\', '/');
        return relative.StartsWith("obj/", StringComparison.OrdinalIgnoreCase);
    }

    private static FSharpParsingOptions CreateParsingOptions(IAnalyzerResult analyzerResult, IEnumerable<string> sourceFiles)
    {
        var args = analyzerResult.PreprocessorSymbols.Select(symbol => $"--define:{symbol}");
        var (options, _) = FsharpCheckerHolder.Instance.GetParsingOptionsFromCommandLineArgs(
            ListModule.OfSeq(sourceFiles), ListModule.OfSeq(args), null, null);
        return options;
    }

    private FsharpFolderComposite GetFolder(Dictionary<string, FsharpFolderComposite> folders, string relativeFolder, string projectDir)
    {
        if (folders.TryGetValue(relativeFolder, out var existing))
        {
            return existing;
        }

        var parent = GetFolder(folders, FileSystem.Path.GetDirectoryName(relativeFolder) ?? string.Empty, projectDir);
        var folder = new FsharpFolderComposite
        {
            FullPath = FileSystem.Path.Combine(projectDir, relativeFolder),
            RelativePath = relativeFolder,
        };
        parent.Add(folder);
        folders[relativeFolder] = folder;
        return folder;
    }

    // F# helpers live in a separate compiled assembly (see FsharpHelperAssembly), not in the source tree.
    public override void InjectHelpers(IReadOnlyProjectComponent inputFiles)
    {
    }

    public override Action PostBuildAction() => () => { };
}
