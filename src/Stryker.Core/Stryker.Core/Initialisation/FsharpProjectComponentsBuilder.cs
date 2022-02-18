using FSharp.Compiler.SourceCodeServices;
using FSharp.Compiler.Text;
using Microsoft.Build.Logging.StructuredLogger;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.ProjectAnalyzer;
using Stryker.Core.ProjectComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using static FSharp.Compiler.SyntaxTree.ParsedInput;
using ParsedInput = FSharp.Compiler.SyntaxTree.ParsedInput;

namespace Stryker.Core.Initialisation
{
    internal class FsharpProjectComponentsBuilder : ProjectComponentsBuilder
    {
        private readonly ProjectInfo _projectInfo;
        private readonly string[] _foldersToExclude;
        private readonly ILogger _logger;

        public FsharpProjectComponentsBuilder(ProjectInfo projectInfo, string[] foldersToExclude, ILogger logger, IFileSystem fileSystem) : base(fileSystem)
        {
            _projectInfo = projectInfo;
            _foldersToExclude = foldersToExclude;
            _logger = logger;
        }
        
        public override IProjectComponent Build()
        {
            FsharpFolderComposite inputFiles;
            if (_projectInfo.ProjectUnderTestAnalyzerResult.SourceFiles != null && _projectInfo.ProjectUnderTestAnalyzerResult.SourceFiles.Any())
            {
                inputFiles = FindProjectFilesUsingBuildalyzer(_projectInfo.ProjectUnderTestAnalyzerResult);
            }
            else
            {
                inputFiles = FindProjectFilesScanningProjectFolders(_projectInfo.ProjectUnderTestAnalyzerResult);
            }
            return inputFiles;
        }

        private FsharpFolderComposite FindProjectFilesUsingBuildalyzer(IAnalysisResult analyzerResult)
        {
            var inputFiles = new FsharpFolderComposite();
            var projectUnderTestDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            var projectRoot = Path.GetDirectoryName(projectUnderTestDir);
            var rootFolderComposite = new FsharpFolderComposite()
            {
                FullPath = projectRoot,
                RelativePath = string.Empty
            };
            var cache = new Dictionary<string, FsharpFolderComposite> { [string.Empty] = rootFolderComposite };

            // Save cache in a singleton so we can use it in other parts of the project
            FolderCompositeCache<FsharpFolderComposite>.Instance.Cache = cache;

            inputFiles.Add(rootFolderComposite);

            var fSharpChecker = FSharpChecker.Create(projectCacheSize: null, keepAssemblyContents: null, keepAllBackgroundResolutions: null, legacyReferenceResolver: null, tryGetMetadataSnapshot: null, suggestNamesForErrors: null, keepAllBackgroundSymbolUses: null, enableBackgroundItemKeyStoreAndSemanticClassification: null);

            foreach (var sourceFile in analyzerResult.SourceFiles)
            {
                // Skip xamarin UI generated files
                if (sourceFile.EndsWith(".xaml.cs"))
                {
                    continue;
                }

                var relativePath = Path.GetRelativePath(projectUnderTestDir, sourceFile);
                var folderComposite = GetOrBuildFolderComposite(cache, Path.GetDirectoryName(relativePath), projectUnderTestDir, projectRoot, inputFiles);
                var fileName = Path.GetFileName(sourceFile);

                var file = new FsharpFileLeaf()
                {
                    SourceCode = FileSystem.File.ReadAllText(sourceFile),
                    RelativePath = FileSystem.Path.Combine(folderComposite.RelativePath, fileName),
                    FullPath = sourceFile
                };

                // Get the syntax tree for the source file
                Tuple<FSharpProjectOptions, FSharpList<FSharpErrorInfo>> fSharpOptions = FSharpAsync.RunSynchronously(fSharpChecker.GetProjectOptionsFromScript(filename: file.FullPath, sourceText: SourceText.ofString(file.SourceCode), previewEnabled: null, loadedTimeStamp: null, otherFlags: null, useFsiAuxLib: null, useSdkRefs: null, assumeDotNetFramework: null, extraProjectInfo: null, optionsStamp: null, userOpName: null), timeout: null, cancellationToken: null);
                FSharpParseFileResults result = FSharpAsync.RunSynchronously(fSharpChecker.ParseFile(fileName, SourceText.ofString(file.SourceCode), fSharpChecker.GetParsingOptionsFromProjectOptions(fSharpOptions.Item1).Item1, userOpName: null), timeout: null, cancellationToken: null);

                if (result.ParseTree.Value.IsImplFile)
                {
                    var syntaxTree = (ImplFile)result.ParseTree.Value;

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
        private FsharpFolderComposite GetOrBuildFolderComposite(IDictionary<string, FsharpFolderComposite> cache, string targetFolder, string projectUnderTestDir,
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
                    var fullPath = FileSystem.Path.Combine(projectUnderTestDir, sub);
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

        private FsharpFolderComposite FindProjectFilesScanningProjectFolders(IAnalysisResult analyzerResult)
        {
            var inputFiles = new FsharpFolderComposite();
            var projectUnderTestDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            foreach (var dir in ExtractProjectFolders(analyzerResult))
            {
                var folder = FileSystem.Path.Combine(Path.GetDirectoryName(projectUnderTestDir), dir);

                _logger.LogDebug($"Scanning {folder}");
                if (!FileSystem.Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException($"Can't find {folder}");
                }

                inputFiles.Add(FindInputFiles(projectUnderTestDir, analyzerResult));
            }

            return inputFiles;
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// </summary>
        private FsharpFolderComposite FindInputFiles(string path, IAnalysisResult analyzerResult)
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
        private FsharpFolderComposite FindInputFiles(string path, string projectUnderTestDir, string parentFolder)
        {
            var lastPathComponent = Path.GetFileName(path);

            var folderComposite = new FsharpFolderComposite
            {
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.Combine(parentFolder, lastPathComponent),
            };

            foreach (var folder in FileSystem.Directory.EnumerateDirectories(folderComposite.FullPath).Where(x => !_foldersToExclude.Contains(Path.GetFileName(x))))
            {
                folderComposite.Add(FindInputFiles(folder, projectUnderTestDir, folderComposite.RelativePath));
            }
            var fSharpChecker = FSharpChecker.Create(projectCacheSize: null, keepAssemblyContents: null, keepAllBackgroundResolutions: null, legacyReferenceResolver: null, tryGetMetadataSnapshot: null, suggestNamesForErrors: null, keepAllBackgroundSymbolUses: null, enableBackgroundItemKeyStoreAndSemanticClassification: null);
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
                Tuple<FSharpProjectOptions, FSharpList<FSharpErrorInfo>> fsharpoptions = FSharpAsync.RunSynchronously(fSharpChecker.GetProjectOptionsFromScript(fileLeaf.FullPath, SourceText.ofString(fileLeaf.SourceCode), previewEnabled: null, loadedTimeStamp: null, otherFlags: null, useFsiAuxLib: null, useSdkRefs: null, assumeDotNetFramework: null, extraProjectInfo: null, optionsStamp: null, userOpName: null), timeout: null, cancellationToken: null);
                FSharpParseFileResults result = FSharpAsync.RunSynchronously(fSharpChecker.ParseFile(fileLeaf.FullPath, SourceText.ofString(fileLeaf.SourceCode), fSharpChecker.GetParsingOptionsFromProjectOptions(fsharpoptions.Item1).Item1, userOpName: null), timeout: null, cancellationToken: null);

                if (result.ParseTree.Value.IsImplFile)
                {
                    var syntaxTree = (ImplFile)result.ParseTree.Value;

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
