using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Core.Initialisation;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.MutationTest;

public interface IAutoIgnoreTimeoutsProcess
{
    /// <summary>
    /// Adds Stryker comments to the source code of the projects to ignore mutants that resulted in a timeout,
    /// and verifies the result by building each changed project. Changes to a project are reverted if its build fails.
    /// </summary>
    void IgnoreTimeouts(IEnumerable<SourceProjectInfo> projects, IStrykerOptions options);
}

public class AutoIgnoreTimeoutsProcess(
    IFileSystem fileSystem,
    IInitialBuildProcess buildProcess,
    ILogger<AutoIgnoreTimeoutsProcess> logger) : IAutoIgnoreTimeoutsProcess
{
    private readonly IFileSystem _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly IInitialBuildProcess _buildProcess = buildProcess ?? throw new ArgumentNullException(nameof(buildProcess));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public void IgnoreTimeouts(IEnumerable<SourceProjectInfo> projects, IStrykerOptions options)
    {
        foreach (var project in projects)
        {
            IgnoreTimeouts(project, options);
        }
    }

    private void IgnoreTimeouts(SourceProjectInfo project, IStrykerOptions options)
    {
        var originalFiles = new Dictionary<string, byte[]>();
        var ignoredMutants = 0;

        foreach (var file in project.ProjectContents.GetAllFiles())
        {
            var timeouts = file.Mutants.Where(m => m.ResultStatus == MutantStatus.Timeout).ToList();
            if (timeouts.Count == 0)
            {
                continue;
            }

            var originalBytes = TryIgnoreTimeouts(file.FullPath, file.SyntaxTree.GetRoot(), timeouts, out var placed);
            if (originalBytes is not null)
            {
                originalFiles[file.FullPath] = originalBytes;
                ignoredMutants += placed;
            }
        }

        var projectPath = project.AnalyzerResult.ProjectFilePath;
        if (originalFiles.Count == 0)
        {
            _logger.LogInformation("No timeouts to ignore in {Project}.", projectPath);
            return;
        }

        if (!BuildSucceeds(project, options))
        {
            foreach (var (path, bytes) in originalFiles)
            {
                _fileSystem.File.WriteAllBytes(path, bytes);
            }
            _logger.LogError(
                "The build failed after adding Stryker comments to ignore timeouts, all changes to {Project} were reverted.",
                projectPath);
            return;
        }

        _logger.LogInformation(
            "Added Stryker comments to ignore {MutantCount} timeout mutants in {FileCount} files of {Project}. The next run will skip them.",
            ignoredMutants, originalFiles.Count, projectPath);
    }

    /// <returns>The original file content when the file was modified, null otherwise.</returns>
    private byte[] TryIgnoreTimeouts(string path, SyntaxNode root, List<IMutant> timeouts, out int placed)
    {
        placed = 0;
        var originalBytes = _fileSystem.File.ReadAllBytes(path);
        var (currentText, encoding) = Decode(originalBytes);
        if (currentText != root.SyntaxTree.GetText().ToString())
        {
            _logger.LogWarning("{Path} changed since it was analyzed, its timeouts will not be ignored.", path);
            return null;
        }

        var newText = TimeoutCommentInserter.Insert(root, timeouts, out var unplaced);
        foreach (var mutant in unplaced)
        {
            _logger.LogWarning("Could not find a valid location to ignore timeout mutant {MutantId} in {Path}.", mutant.Id, path);
        }

        if (newText is null)
        {
            return null;
        }

        _fileSystem.File.WriteAllText(path, newText, encoding);
        placed = timeouts.Count - unplaced.Count;
        return originalBytes;
    }

    private bool BuildSucceeds(SourceProjectInfo project, IStrykerOptions options)
    {
        var analyzerResult = project.AnalyzerResult;
        var fullFramework = analyzerResult.TargetsDesktop();
        _logger.LogInformation("Building {Project} to validate the added Stryker comments.", analyzerResult.ProjectFilePath);
        try
        {
            _buildProcess.InitialBuild(
                fullFramework,
                analyzerResult.ProjectFilePath,
                fullFramework ? options.SolutionPath : null,
                analyzerResult.GetProperty("Configuration"),
                analyzerResult.GetProperty("Platform"),
                msbuildPath: analyzerResult.MsBuildPath());
            return true;
        }
        catch (InputException)
        {
            // details are already logged by the build process
            return false;
        }
    }

    private static (string Text, Encoding Encoding) Decode(byte[] bytes)
    {
        // Detect a BOM, otherwise assume UTF-8 without BOM so the encoding is written back as it was found.
        using var reader = new StreamReader(new MemoryStream(bytes), new UTF8Encoding(false), detectEncodingFromByteOrderMarks: true);
        var text = reader.ReadToEnd();
        return (text, reader.CurrentEncoding);
    }
}
