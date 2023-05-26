using Buildalyzer;
using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.Text;
using Microsoft.Build.Logging.StructuredLogger;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Control;
using Stryker.Core.Exceptions;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.SourceProjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static FSharp.Compiler.Syntax.ParsedInput;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Stryker.Core.Initialisation
{
    internal class FsharpProjectComponentsBuilder : ProjectComponentsBuilder
    {
        private readonly SourceProjectInfo _projectInfo;
        private readonly string[] _foldersToExclude;
        private readonly ILogger _logger;

        public FsharpProjectComponentsBuilder(SourceProjectInfo projectInfo, string[] foldersToExclude, ILogger logger, IFileSystem fileSystem) : base(fileSystem)
        {
            _projectInfo = projectInfo;
            _foldersToExclude = foldersToExclude;
            _logger = logger;
        }
        
        public override IProjectComponent Build()
        {
            FsharpFolderComposite inputFiles;
            if (_projectInfo.AnalyzerResult.SourceFiles != null && _projectInfo.AnalyzerResult.SourceFiles.Any())
            {
                inputFiles = FindProjectFilesUsingBuildalyzer(_projectInfo.AnalyzerResult);
            }
            else
            {
                inputFiles = FindProjectFilesScanningProjectFolders(_projectInfo.AnalyzerResult);
            }
            return inputFiles;
        }

        private FsharpFolderComposite FindProjectFilesUsingBuildalyzer(IAnalyzerResult analyzerResult)
        {
            var inputFiles = new FsharpFolderComposite();
            var sourceProjectDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            var projectRoot = Path.GetDirectoryName(sourceProjectDir);
            var rootFolderComposite = new FsharpFolderComposite()
            {
                FullPath = projectRoot,
                RelativePath = string.Empty
            };
            var cache = new Dictionary<string, FsharpFolderComposite> { [string.Empty] = rootFolderComposite };

            // Save cache in a singleton so we can use it in other parts of the project
            FolderCompositeCache<FsharpFolderComposite>.Instance.Cache = cache;

            inputFiles.Add(rootFolderComposite);

            var fSharpChecker = FSharpChecker.Create(
                projectCacheSize: null,
                keepAssemblyContents: null,
                keepAllBackgroundResolutions: null,
                legacyReferenceResolver: null,
                tryGetMetadataSnapshot: null,
                suggestNamesForErrors: null,
                keepAllBackgroundSymbolUses: null,
                enableBackgroundItemKeyStoreAndSemanticClassification: null,
                enablePartialTypeChecking: null);

            foreach (var sourceFile in analyzerResult.SourceFiles)
            {
                // Skip xamarin UI generated files
                if (sourceFile.EndsWith(".xaml.cs"))
                {
                    continue;
                }

                if (!FileSystem.File.Exists(sourceFile))
                {
                    _logger.LogWarning($"F# project builder: skipping non existing file {sourceFile}.");
                    continue;
                }

                var relativePath = Path.GetRelativePath(sourceProjectDir, sourceFile);
                var folderComposite = GetOrBuildFolderComposite(cache, Path.GetDirectoryName(relativePath), sourceProjectDir, projectRoot, inputFiles);
                var fileName = Path.GetFileName(sourceFile);

                var file = new FsharpFileLeaf()
                {
                    SourceCode = FileSystem.File.ReadAllText(sourceFile),
                    RelativePath = FileSystem.Path.Combine(folderComposite.RelativePath, fileName),
                    FullPath = sourceFile
                };

                // Get the syntax tree for the source file
                var projectOptions = fSharpChecker.GetProjectOptionsFromScript(
                    fileName: file.FullPath,
                    source: SourceText.ofString(file.SourceCode),
                    previewEnabled: null,
                    loadedTimeStamp: null,
                    otherFlags: null,
                    useFsiAuxLib: null,
                    useSdkRefs: null,
                    assumeDotNetFramework: null,
                    optionsStamp: null,
                    userOpName: null,
                    sdkDirOverride: null);

                var fSharpOptions = FSharpAsync.RunSynchronously(projectOptions, timeout: null, cancellationToken: null);

                var parseFileResults = fSharpChecker.ParseFile(
                    fileName,
                    SourceText.ofString(file.SourceCode),
                    fSharpChecker.GetParsingOptionsFromProjectOptions(fSharpOptions.Item1).Item1,
                    userOpName: null,
                    cache: null);
                var result = FSharpAsync.RunSynchronously(parseFileResults, timeout: null, cancellationToken: null);

                if (result.ParseTree.IsImplFile)
                {
                    var syntaxTree = (ImplFile)result.ParseTree;

                    file.SyntaxTree = syntaxTree;
                    folderComposite.Add(file);
                }
                else
                {
                    var message = $"Cannot make Fsharp SyntaxTree from .fsi filetype (SyntaxTree.ParsedImplFileInput class wanted)";
                    throw new InputException(message);
                }
            }
            return inputFiles;
        }

        // get the FolderComposite object representing the the project's folder 'targetFolder'. Build the needed FolderComposite(s) for a complete path
        private FsharpFolderComposite GetOrBuildFolderComposite(IDictionary<string, FsharpFolderComposite> cache, string targetFolder, string sourceProjectDir,
            string projectRoot, FsharpFolderComposite inputFiles)
        {
            if (cache.ContainsKey(targetFolder))
            {
                return cache[targetFolder];
            }

            var folder = targetFolder;
            FsharpFolderComposite subDir = null;
            while (!string.IsNullOrEmpty(folder))
            {
                if (!cache.ContainsKey(folder))
                {
                    // we have not scanned this folder yet
                    var sub = Path.GetFileName(folder);
                    var fullPath = FileSystem.Path.Combine(sourceProjectDir, sub);
                    var newComposite = new FsharpFolderComposite
                    {
                        FullPath = fullPath,
                        RelativePath = Path.GetRelativePath(projectRoot, fullPath),
                    };
                    if (subDir != null)
                    {
                        newComposite.Add(subDir);
                    }

                    cache.Add(folder, newComposite);
                    subDir = newComposite;
                    folder = Path.GetDirectoryName(folder);
                    if (string.IsNullOrEmpty(folder))
                    {
                        // we are at root
                        inputFiles.Add(subDir);
                    }
                }
                else
                {
                    (cache[folder]).Add(subDir);
                    break;
                }
            }

            return cache[targetFolder];
        }

        private FsharpFolderComposite FindProjectFilesScanningProjectFolders(IAnalyzerResult analyzerResult)
        {
            var inputFiles = new FsharpFolderComposite();
            var sourceProjectDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            foreach (var dir in ExtractProjectFolders(analyzerResult))
            {
                var folder = FileSystem.Path.Combine(Path.GetDirectoryName(sourceProjectDir), dir);

                _logger.LogDebug($"Scanning {folder}");
                if (!FileSystem.Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException($"Can't find {folder}");
                }

                inputFiles.Add(FindInputFiles(sourceProjectDir, analyzerResult));
            }

            return inputFiles;
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// </summary>
        private FsharpFolderComposite FindInputFiles(string path, IAnalyzerResult analyzerResult)
        {
            var rootFolderComposite = new FsharpFolderComposite
            {
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.GetFileName(path),
            };

            rootFolderComposite.Add(
                FindInputFiles(path, Path.GetDirectoryName(analyzerResult.ProjectFilePath), rootFolderComposite.RelativePath)
            );
            return rootFolderComposite;
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// </summary>
        private FsharpFolderComposite FindInputFiles(string path, string sourceProjectDir, string parentFolder)
        {
            var lastPathComponent = Path.GetFileName(path);

            var folderComposite = new FsharpFolderComposite
            {
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.Combine(parentFolder, lastPathComponent),
            };

            foreach (var folder in FileSystem.Directory.EnumerateDirectories(folderComposite.FullPath).Where(x => !_foldersToExclude.Contains(Path.GetFileName(x))))
            {
                folderComposite.Add(FindInputFiles(folder, sourceProjectDir, folderComposite.RelativePath));
            }
            var fSharpChecker = FSharpChecker.Create(
                projectCacheSize: null,
                keepAssemblyContents: null,
                keepAllBackgroundResolutions: null,
                legacyReferenceResolver: null,
                tryGetMetadataSnapshot: null,
                suggestNamesForErrors: null,
                keepAllBackgroundSymbolUses: null,
                enableBackgroundItemKeyStoreAndSemanticClassification: null,
                enablePartialTypeChecking: null);

            foreach (var file in FileSystem.Directory.GetFiles(folderComposite.FullPath, "*.fs", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileName(file);

                var fileLeaf = new FsharpFileLeaf()
                {
                    SourceCode = FileSystem.File.ReadAllText(file),
                    RelativePath = Path.Combine(folderComposite.RelativePath, fileName),
                    FullPath = file,
                };

                // Get the syntax tree for the source file

                var projectOptions = fSharpChecker.GetProjectOptionsFromScript(
                    fileLeaf.FullPath,
                    SourceText.ofString(fileLeaf.SourceCode),
                    previewEnabled: null,
                    loadedTimeStamp: null,
                    otherFlags: null,
                    useFsiAuxLib: null,
                    useSdkRefs: null,
                    assumeDotNetFramework: null,
                    optionsStamp: null,
                    userOpName: null,
                    sdkDirOverride: null);

                var fsharpoptions = FSharpAsync.RunSynchronously(projectOptions, timeout: null, cancellationToken: null);

                var parseFileResults = fSharpChecker.ParseFile(
                    fileLeaf.FullPath,
                    SourceText.ofString(fileLeaf.SourceCode),
                    fSharpChecker.GetParsingOptionsFromProjectOptions(fsharpoptions.Item1).Item1,
                    userOpName: null,
                    cache: null);
                var result = FSharpAsync.RunSynchronously(parseFileResults, timeout: null, cancellationToken: null);

                if (result.ParseTree.IsImplFile)
                {
                    var syntaxTree = (ImplFile)result.ParseTree;

                    fileLeaf.SyntaxTree = syntaxTree;

                    folderComposite.Add(fileLeaf);
                }
                else
                {
                    var message = $"Cannot make Fsharp SyntaxTree from .fsi filetype (SyntaxTree.ParsedImplFileInput class wanted)";
                    throw new InputException(message);
                }
            }

            return folderComposite;
        }
    }
}
