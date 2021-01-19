using Buildalyzer;
using FSharp.Compiler.SourceCodeServices;
using FSharp.Compiler.Text;
using Microsoft.Build.Logging.StructuredLogger;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Stryker.Core.Exceptions;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static FSharp.Compiler.SyntaxTree.ParsedInput;
using ParsedInput = FSharp.Compiler.SyntaxTree.ParsedInput;

namespace Stryker.Core.Initialisation
{
    internal class FsharpProjectComponentsBuilder : IProjectComponentsBuilder
    {
        private ProjectInfo _projectInfo;
        private IStrykerOptions _options;
        private string[] _foldersToExclude;
        private ILogger _logger;
        private IFileSystem _fileSystem;

        public FsharpProjectComponentsBuilder(ProjectInfo projectInfo, IStrykerOptions options, string[] foldersToExclude, ILogger logger, IFileSystem fileSystem)
        {
            _projectInfo = projectInfo;
            _options = options;
            _foldersToExclude = foldersToExclude;
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public IProjectComponent Build()
        {
            FolderCompositeFsharp inputFiles;
            if (_projectInfo.ProjectUnderTestAnalyzerResult.SourceFiles != null && _projectInfo.ProjectUnderTestAnalyzerResult.SourceFiles.Any())
            {
                inputFiles = FindProjectFilesUsingBuildalyzer(_projectInfo.ProjectUnderTestAnalyzerResult, _options);
            }
            else
            {
                inputFiles = FindProjectFilesScanningProjectFolders(_projectInfo.ProjectUnderTestAnalyzerResult, _options);
            }
            return inputFiles;
        }
        private FolderCompositeFsharp FindProjectFilesUsingBuildalyzer(IAnalyzerResult analyzerResult, IStrykerOptions options)
        {
            var inputFiles = new FolderCompositeFsharp();
            var projectUnderTestDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            var projectRoot = Path.GetDirectoryName(projectUnderTestDir);
            var rootFolderComposite = new FolderCompositeFsharp()
            {
                Name = string.Empty,
                FullPath = projectRoot,
                RelativePath = string.Empty,
                RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, projectUnderTestDir)
            };
            var cache = new Dictionary<string, FolderCompositeFsharp> { [string.Empty] = rootFolderComposite };

            // Save cache in a singleton so we can use it in other parts of the project
            FolderCompositeCache<FolderCompositeFsharp>.Instance.Cache = cache;

            inputFiles.Add(rootFolderComposite);

            //InjectMutantHelpers(rootFolderComposite);
            var fSharpChecker = FSharpChecker.Create(null, null, null, null, null, null, null, null);

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

                var file = new FileLeafFsharp()
                {
                    SourceCode = _fileSystem.File.ReadAllText(sourceFile),
                    Name = _fileSystem.Path.GetFileName(sourceFile),
                    RelativePath = _fileSystem.Path.Combine(folderComposite.RelativePath, fileName),
                    FullPath = sourceFile,
                    RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, sourceFile)
                };

                // Get the syntax tree for the source file
                Tuple<FSharpProjectOptions, FSharpList<FSharpErrorInfo>> fsharpoptions = FSharpAsync.RunSynchronously(fSharpChecker.GetProjectOptionsFromScript(file.FullPath, SourceText.ofString(file.SourceCode), null, null, null, null, null, null, null, null, null), null, null);
                FSharpParseFileResults result = FSharpAsync.RunSynchronously(fSharpChecker.ParseFile(file.Name, SourceText.ofString(file.SourceCode), fSharpChecker.GetParsingOptionsFromProjectOptions(fsharpoptions.Item1).Item1, null), null, null);

                if (result.ParseTree.Value.IsImplFile)
                {
                    var syntaxTree = (ImplFile)result.ParseTree.Value;

                    //// don't mutate auto generated code
                    //if (syntaxTree.IsGenerated())
                    //{
                    //    // we found the generated assemblyinfo file
                    //    if (_fileSystem.Path.GetFileName(sourceFile).ToLowerInvariant() == generatedAssemblyInfo)
                    //    {
                    //        // add the mutated text
                    //        syntaxTree = InjectMutationLabel(syntaxTree);
                    //    }
                    //    _logger.LogDebug("Skipping auto-generated code file: {fileName}", file.Name);
                    //    folderComposite.AddCompilationSyntaxTree(syntaxTree); // Add the syntaxTree to the list of compilationSyntaxTrees
                    //    continue; // Don't add the file to the folderComposite as we're not reporting on the file
                    //}

                    file.SyntaxTree = syntaxTree;
                    folderComposite.Add(file);
                }
                else
                {
                    var message = $"Cannot make Fsharp SyntaxTree from .fsi filetype (SyntaxTree.ParsedImplFileInput class wanted)";
                    throw new StrykerInputException(message);
                }
            }
            return inputFiles;
        }

        // get the FolderComposite object representing the the project's folder 'targetFolder'. Build the needed FolderComposite(s) for a complete path
        private FolderCompositeFsharp GetOrBuildFolderComposite(IDictionary<string, FolderCompositeFsharp> cache, string targetFolder, string projectUnderTestDir,
            string projectRoot, ProjectComponent<ParsedInput> inputFiles)
        {
            if (cache.ContainsKey(targetFolder))
            {
                return cache[targetFolder];
            }

            var folder = targetFolder;
            FolderCompositeFsharp subDir = null;
            while (!string.IsNullOrEmpty(folder))
            {
                if (!cache.ContainsKey(folder))
                {
                    // we have not scanned this folder yet
                    var sub = Path.GetFileName(folder);
                    var fullPath = _fileSystem.Path.Combine(projectUnderTestDir, sub);
                    var newComposite = new FolderCompositeFsharp
                    {
                        Name = sub,
                        FullPath = fullPath,
                        RelativePath = Path.GetRelativePath(projectRoot, fullPath),
                        RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, fullPath)
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
                        ((IFolderComposite)inputFiles).Add(subDir);
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

        //private FSharpOption<ParsedInput> InjectMutationLabel(FSharpOption<ParsedInput> syntaxTree)
        //{
        //    var root = syntaxTree.GetRoot();

        //    var myAttribute = ((CompilationUnitSyntax)root).AttributeLists
        //        .SelectMany(al => al.Attributes).FirstOrDefault(n => n.Name.Kind() == SyntaxKind.QualifiedName
        //                                                             && ((QualifiedNameSyntax)n.Name).Right
        //                                                             .Kind() == SyntaxKind.IdentifierName
        //                                                             && (string)((IdentifierNameSyntax)((QualifiedNameSyntax)n.Name).Right)
        //                                                             .Identifier.Value == "AssemblyTitleAttribute");
        //    var labelNode = myAttribute?.ArgumentList.Arguments.First()?.Expression;
        //    var newLabel = string.Empty;
        //    if (labelNode != null && labelNode.Kind() == SyntaxKind.StringLiteralExpression)
        //    {
        //        var literal = (LiteralExpressionSyntax)labelNode;
        //        newLabel = $"Mutated {literal.Token.Value}";
        //    }

        //    if (myAttribute != null)
        //    {
        //        var newAttribute = myAttribute.ReplaceNode(labelNode,
        //            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(newLabel)));
        //        root = root.ReplaceNode(myAttribute, newAttribute);
        //        return root.SyntaxTree;
        //    }

        //    return syntaxTree;
        //}

        private FolderCompositeFsharp FindProjectFilesScanningProjectFolders(IAnalyzerResult analyzerResult, IStrykerOptions options)
        {
            var inputFiles = new FolderCompositeFsharp();
            var projectUnderTestDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            foreach (var dir in ExtractProjectFolders(analyzerResult))
            {
                var folder = _fileSystem.Path.Combine(Path.GetDirectoryName(projectUnderTestDir), dir);

                _logger.LogDebug($"Scanning {folder}");
                if (!_fileSystem.Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException($"Can't find {folder}");
                }

                inputFiles.Add(FindInputFiles(folder, projectUnderTestDir, analyzerResult, options));
            }

            return inputFiles;
        }

        private IEnumerable<string> ExtractProjectFolders(IAnalyzerResult projectAnalyzerResult)
        {
            var projectFilePath = projectAnalyzerResult.ProjectFilePath;
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            var xDocument = XDocument.Load(projectFile);
            var folders = new List<string>();
            var projectDirectory = _fileSystem.Path.GetDirectoryName(projectFilePath);
            folders.Add(projectDirectory);

            foreach (var sharedProject in FindSharedProjects(xDocument))
            {
                var sharedProjectName = ReplaceMsbuildProperties(sharedProject, projectAnalyzerResult);

                if (!_fileSystem.File.Exists(_fileSystem.Path.Combine(projectDirectory, sharedProjectName)))
                {
                    throw new FileNotFoundException($"Missing shared project {sharedProjectName}");
                }

                var directoryName = _fileSystem.Path.GetDirectoryName(sharedProjectName);
                folders.Add(_fileSystem.Path.Combine(projectDirectory, directoryName));
            }

            return folders;
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// </summary>
        private FolderCompositeFsharp FindInputFiles(string path, string projectUnderTestDir, IAnalyzerResult analyzerResult, IStrykerOptions options)
        {
            var rootFolderComposite = new FolderCompositeFsharp
            {
                Name = Path.GetFileName(path),
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.GetFileName(path),
                RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, Path.GetFullPath(path))
            };

            //InjectMutantHelpers(rootFolderComposite);

            rootFolderComposite.Add(
                FindInputFiles(path, Path.GetDirectoryName(analyzerResult.ProjectFilePath), rootFolderComposite.RelativePath)
            );
            return rootFolderComposite;
        }

        private static void InjectMutantHelpers(FolderCompositeFsharp rootFolderComposite)
        {
            var fSharpChecker = FSharpChecker.Create(null, null, null, null, null, null, null, null);
            /* [OptionalArgument] FSharpOption<int> projectCacheSize, 
             * [OptionalArgument] FSharpOption<bool> keepAssemblyContents, 
             * [OptionalArgument] FSharpOption<bool> keepAllBackgroundResolutions, 
             * [OptionalArgument] FSharpOption<ReferenceResolver.Resolver> legacyReferenceResolver, 
             * [OptionalArgument] FSharpOption<FSharpFunc<Tuple<string, DateTime>, FSharpOption<Tuple<object, IntPtr, int>>>> tryGetMetadataSnapshot, 
             * [OptionalArgument] FSharpOption<bool> suggestNamesForErrors, 
             * [OptionalArgument] FSharpOption<bool> keepAllBackgroundSymbolUses, 
             * [OptionalArgument] FSharpOption<bool> enableBackgroundItemKeyStoreAndSemanticClassification*/
            foreach (var (name, code) in CodeInjection.MutantHelpers)
            {
                Tuple<FSharpProjectOptions, FSharpList<FSharpErrorInfo>> fsharpoptions = FSharpAsync.RunSynchronously(fSharpChecker.GetProjectOptionsFromScript(name, SourceText.ofString(code), null, null, null, null, null, null, null, null, null), null, null);
                /* [OptionalArgument] FSharpOption<bool> previewEnabled, 
                 * [OptionalArgument] FSharpOption<DateTime> loadedTimeStamp, 
                 * [OptionalArgument] FSharpOption<string[]> otherFlags, 
                 * [OptionalArgument] FSharpOption<bool> useFsiAuxLib, 
                 * [OptionalArgument] FSharpOption<bool> useSdkRefs,
                 * [OptionalArgument] FSharpOption<bool> assumeDotNetFramework,
                 * [OptionalArgument] FSharpOption<object> extraProjectInfo,
                 * [OptionalArgument] FSharpOption<long> optionsStamp,
                 * [OptionalArgument] FSharpOption<string> userOpName*/
                FSharpParseFileResults result = FSharpAsync.RunSynchronously(fSharpChecker.ParseFile(name, SourceText.ofString(code), fSharpChecker.GetParsingOptionsFromProjectOptions(fsharpoptions.Item1).Item1, null), null, null);
                /*[OptionalArgument] FSharpOption<string> userOpName */
                // Get the syntax tree for the source file

                if (!result.ParseHadErrors && result.ParseTree.Value.IsImplFile)
                {
                    rootFolderComposite.AddCompilationSyntaxTree(result.ParseTree.Value);
                }
                else
                {
                    var message = $"Cannot make Fsharp SyntaxTree from .fsi filetype (SyntaxTree.ParsedImplFileInput class wanted)";
                    throw new StrykerInputException(message);
                }

            }
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// </summary>
        private FolderCompositeFsharp FindInputFiles(string path, string projectUnderTestDir, string parentFolder)
        {
            var lastPathComponent = Path.GetFileName(path);

            var folderComposite = new FolderCompositeFsharp
            {
                Name = lastPathComponent,
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.Combine(parentFolder, lastPathComponent),
                RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, Path.GetFullPath(path))
            };

            foreach (var folder in _fileSystem.Directory.EnumerateDirectories(folderComposite.FullPath).Where(x => !_foldersToExclude.Contains(Path.GetFileName(x))))
            {
                folderComposite.Add(FindInputFiles(folder, projectUnderTestDir, folderComposite.RelativePath));
            }
            var fSharpChecker = FSharpChecker.Create(null, null, null, null, null, null, null, null);
            foreach (var file in _fileSystem.Directory.GetFiles(folderComposite.FullPath, "*.fs", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileName(file);

                var fileLeaf = new FileLeafFsharp()
                {
                    SourceCode = _fileSystem.File.ReadAllText(file),
                    Name = _fileSystem.Path.GetFileName(file),
                    RelativePath = Path.Combine(folderComposite.RelativePath, fileName),
                    FullPath = file,
                    RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, file)
                };

                // Get the syntax tree for the source file
                Tuple<FSharpProjectOptions, FSharpList<FSharpErrorInfo>> fsharpoptions = FSharpAsync.RunSynchronously(fSharpChecker.GetProjectOptionsFromScript(fileLeaf.FullPath, SourceText.ofString(fileLeaf.SourceCode), null, null, null, null, null, null, null, null, null), null, null);
                FSharpParseFileResults result = FSharpAsync.RunSynchronously(fSharpChecker.ParseFile(fileLeaf.FullPath, SourceText.ofString(fileLeaf.SourceCode), fSharpChecker.GetParsingOptionsFromProjectOptions(fsharpoptions.Item1).Item1, null), null, null);

                if (result.ParseTree.Value.IsImplFile)
                {
                    var syntaxTree = (ImplFile)result.ParseTree.Value;

                    //// don't mutate auto generated code
                    //if (syntaxTree.IsGenerated())
                    //{
                    //    _logger.LogDebug("Skipping auto-generated code file: {fileName}", fileLeaf.Name);
                    //    folderComposite.AddCompilationSyntaxTree(syntaxTree); // Add the syntaxTree to the list of compilationSyntaxTrees
                    //    continue; // Don't add the file to the folderComposite as we're not reporting on the file
                    //}
                    fileLeaf.SyntaxTree = syntaxTree;

                    folderComposite.Add(fileLeaf);
                }
                else
                {
                    var message = $"Cannot make Fsharp SyntaxTree from .fsi filetype (SyntaxTree.ParsedImplFileInput class wanted)";
                    throw new StrykerInputException(message);
                }
            }

            return folderComposite;
        }

        private IEnumerable<string> FindSharedProjects(XDocument document)
        {
            var importStatements = document.Elements().Descendants()
                .Where(projectElement => string.Equals(projectElement.Name.LocalName, "Import", StringComparison.OrdinalIgnoreCase));

            var sharedProjects = importStatements
                .SelectMany(importStatement => importStatement.Attributes(
                    XName.Get("Project")))
                .Select(importFileLocation => FilePathUtils.NormalizePathSeparators(importFileLocation.Value))
                .Where(importFileLocation => importFileLocation.EndsWith(".projitems"));
            return sharedProjects;
        }

        private static string ReplaceMsbuildProperties(string projectReference, IAnalyzerResult projectAnalyzerResult)
        {
            var propertyRegex = new Regex(@"\$\(([a-zA-Z_][a-zA-Z0-9_\-.]*)\)");
            var properties = projectAnalyzerResult.Properties;

            return propertyRegex.Replace(projectReference,
                m =>
                {
                    var property = m.Groups[1].Value;
                    if (properties.TryGetValue(property, out var propertyValue))
                    {
                        return propertyValue;
                    }

                    var message = $"Missing MSBuild property ({property}) in project reference ({projectReference}). Please check your project file ({projectAnalyzerResult.ProjectFilePath}) and try again.";
                    throw new StrykerInputException(message);
                });
        }
    }
}
