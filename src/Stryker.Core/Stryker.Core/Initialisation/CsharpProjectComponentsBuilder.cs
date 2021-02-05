using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.MutantFilters.Extensions;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Initialisation
{
    public class CsharpProjectComponentsBuilder
    {
        private readonly ProjectInfo _projectInfo;
        private readonly IStrykerOptions _options;
        private readonly string[] _foldersToExclude;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public CsharpProjectComponentsBuilder(ProjectInfo projectInfo, IStrykerOptions options, string[] foldersToExclude, ILogger logger, IFileSystem fileSystem)
        {
            _projectInfo = projectInfo;
            _options = options;
            _foldersToExclude = foldersToExclude;
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public IProjectComponent Build()
        {
            CsharpFolderComposite inputFiles;
            if (_projectInfo.ProjectUnderTestAnalyzerResult.SourceFiles != null && _projectInfo.ProjectUnderTestAnalyzerResult.SourceFiles.Any())
            {
                inputFiles = FindProjectFilesUsingBuildalyzer(_projectInfo.ProjectUnderTestAnalyzerResult, _options);
            }
            else
            {
                _logger.LogWarning("Buildalyzer could not find sourcefiles. This should not happen. Will fallback to filesystem scan. Please report an issue at github.");
                inputFiles = FindProjectFilesScanningProjectFolders(_projectInfo.ProjectUnderTestAnalyzerResult, _options);
            }
            return inputFiles;
        }

        // Deprecated method, should not be maintained
        private CsharpFolderComposite FindProjectFilesScanningProjectFolders(IAnalyzerResult analyzerResult, IStrykerOptions options)
        {
            var inputFiles = new CsharpFolderComposite();
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

        private CsharpFolderComposite FindProjectFilesUsingBuildalyzer(IAnalyzerResult analyzerResult, IStrykerOptions options)
        {
            var projectUnderTestDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            var generatedAssemblyInfo = analyzerResult.AssemblyAttributeFileName();
            var projectUnderTestFolderComposite = new CsharpFolderComposite()
            {
                FullPath = projectUnderTestDir,
                RelativePath = string.Empty,
            };
            var cache = new Dictionary<string, CsharpFolderComposite> { [string.Empty] = projectUnderTestFolderComposite };

            // Save cache in a singleton so we can use it in other parts of the project
            FolderCompositeCache<CsharpFolderComposite>.Instance.Cache = cache;

            CSharpParseOptions cSharpParseOptions = BuildCsharpParseOptions(analyzerResult, options);
            InjectMutantHelpers(projectUnderTestFolderComposite, cSharpParseOptions);

            foreach (var sourceFile in analyzerResult.SourceFiles)
            {
                // Skip xamarin UI generated files
                if (sourceFile.EndsWith(".xaml.cs"))
                {
                    continue;
                }

                var relativePath = Path.GetRelativePath(projectUnderTestDir, sourceFile);
                var folderComposite = GetOrBuildFolderComposite(cache, Path.GetDirectoryName(relativePath), projectUnderTestDir, projectUnderTestFolderComposite);

                var file = new CsharpFileLeaf()
                {
                    SourceCode = _fileSystem.File.ReadAllText(sourceFile),
                    FullPath = sourceFile,
                    RelativePath = Path.GetRelativePath(projectUnderTestDir, sourceFile)
                };

                // Get the syntax tree for the source file
                var syntaxTree = CSharpSyntaxTree.ParseText(file.SourceCode,
                    path: file.FullPath,
                    encoding: Encoding.UTF32,
                    options: cSharpParseOptions);

                // don't mutate auto generated code
                if (syntaxTree.IsGenerated())
                {
                    // we found the generated assemblyinfo file
                    if (_fileSystem.Path.GetFileName(sourceFile).ToLowerInvariant() == generatedAssemblyInfo)
                    {
                        // add the mutated text
                        syntaxTree = InjectMutationLabel(syntaxTree);
                    }
                    _logger.LogDebug("Skipping auto-generated code file: {fileName}", file.FullPath);
                    folderComposite.AddCompilationSyntaxTree(syntaxTree); // Add the syntaxTree to the list of compilationSyntaxTrees
                    continue; // Don't add the file to the folderComposite as we're not reporting on the file
                }

                file.SyntaxTree = syntaxTree;
                folderComposite.Add(file);
            }

            return projectUnderTestFolderComposite;
        }

        private SyntaxTree InjectMutationLabel(SyntaxTree syntaxTree)
        {
            var root = syntaxTree.GetRoot();

            var myAttribute = ((CompilationUnitSyntax)root).AttributeLists
                .SelectMany(al => al.Attributes).FirstOrDefault(n => n.Name.Kind() == SyntaxKind.QualifiedName
                                                                     && ((QualifiedNameSyntax)n.Name).Right
                                                                     .Kind() == SyntaxKind.IdentifierName
                                                                     && (string)((IdentifierNameSyntax)((QualifiedNameSyntax)n.Name).Right)
                                                                     .Identifier.Value == "AssemblyTitleAttribute");
            var labelNode = myAttribute?.ArgumentList.Arguments.First()?.Expression;
            var newLabel = string.Empty;
            if (labelNode != null && labelNode.Kind() == SyntaxKind.StringLiteralExpression)
            {
                var literal = (LiteralExpressionSyntax)labelNode;
                newLabel = $"Mutated {literal.Token.Value}";
            }

            if (myAttribute != null)
            {
                var newAttribute = myAttribute.ReplaceNode(labelNode,
                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(newLabel)));
                root = root.ReplaceNode(myAttribute, newAttribute);
                return root.SyntaxTree;
            }

            return syntaxTree;
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// Deprecated method, should not be maintained
        /// </summary>
        private CsharpFolderComposite FindInputFiles(string path, string projectUnderTestDir, IAnalyzerResult analyzerResult, IStrykerOptions options)
        {
            var rootFolderComposite = new CsharpFolderComposite
            {
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.GetRelativePath(projectUnderTestDir, Path.GetFullPath(path))
            };

            CSharpParseOptions cSharpParseOptions = BuildCsharpParseOptions(analyzerResult, options);
            InjectMutantHelpers(rootFolderComposite, cSharpParseOptions);

            rootFolderComposite.Add(
                FindInputFiles(path, Path.GetDirectoryName(analyzerResult.ProjectFilePath), cSharpParseOptions)
            );
            return rootFolderComposite;
        }


        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// Deprecated method, should not be maintained
        /// </summary>
        private CsharpFolderComposite FindInputFiles(string path, string projectUnderTestDir, CSharpParseOptions cSharpParseOptions)
        {

            var folderComposite = new CsharpFolderComposite
            {
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.GetRelativePath(projectUnderTestDir, Path.GetFullPath(path))
            };

            foreach (var folder in _fileSystem.Directory.EnumerateDirectories(folderComposite.FullPath).Where(x => !_foldersToExclude.Contains(Path.GetFileName(x))))
            {
                folderComposite.Add(FindInputFiles(folder, projectUnderTestDir, cSharpParseOptions));
            }
            foreach (var file in _fileSystem.Directory.GetFiles(folderComposite.FullPath, "*.cs", SearchOption.TopDirectoryOnly).Where(f => !f.EndsWith(".xaml.cs")))
            {
                // Roslyn cannot compile xaml.cs files generated by xamarin. 
                // Since the files are generated they should not be mutated anyway, so skip these files.

                var fileLeaf = new CsharpFileLeaf()
                {
                    SourceCode = _fileSystem.File.ReadAllText(file),
                    FullPath = file,
                    RelativePath = Path.GetRelativePath(projectUnderTestDir, file)
                };

                // Get the syntax tree for the source file
                var syntaxTree = CSharpSyntaxTree.ParseText(fileLeaf.SourceCode, path: fileLeaf.FullPath, options: cSharpParseOptions);

                // don't mutate auto generated code
                if (syntaxTree.IsGenerated())
                {
                    _logger.LogDebug("Skipping auto-generated code file: {fileName}", fileLeaf.FullPath);
                    folderComposite.AddCompilationSyntaxTree(syntaxTree); // Add the syntaxTree to the list of compilationSyntaxTrees
                    continue; // Don't add the file to the folderComposite as we're not reporting on the file
                }

                fileLeaf.SyntaxTree = syntaxTree;

                folderComposite.Add(fileLeaf);
            }

            return folderComposite;
        }

        private static void InjectMutantHelpers(CsharpFolderComposite rootFolderComposite, CSharpParseOptions cSharpParseOptions)
        {
            foreach (var (name, code) in CodeInjection.MutantHelpers)
            {
                rootFolderComposite.AddCompilationSyntaxTree(CSharpSyntaxTree.ParseText(code, path: name, encoding: Encoding.UTF32, options: cSharpParseOptions));
            }
        }

        private static CSharpParseOptions BuildCsharpParseOptions(IAnalyzerResult analyzerResult, IStrykerOptions options)
        {
            return new CSharpParseOptions(options.LanguageVersion, DocumentationMode.None, preprocessorSymbols: analyzerResult.GetDefineConstants());
        }

        // get the FolderComposite object representing the the project's folder 'targetFolder'. Build the needed FolderComposite(s) for a complete path
        private CsharpFolderComposite GetOrBuildFolderComposite(IDictionary<string, CsharpFolderComposite> cache, string targetFolder, string projectUnderTestDir, ProjectComponent<SyntaxTree> inputFiles)
        {
            if (cache.ContainsKey(targetFolder))
            {
                return cache[targetFolder];
            }

            var folder = targetFolder;
            CsharpFolderComposite subDir = null;
            while (!string.IsNullOrEmpty(folder))
            {
                if (!cache.ContainsKey(folder))
                {
                    // we have not scanned this folder yet
                    var fullPath = _fileSystem.Path.Combine(projectUnderTestDir, folder);
                    var newComposite = new CsharpFolderComposite
                    {
                        FullPath = fullPath,
                        RelativePath = Path.GetRelativePath(projectUnderTestDir, fullPath),
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
                        var root = inputFiles as IFolderComposite;
                        root.Add(subDir);
                    }
                }
                else
                {
                    cache[folder].Add(subDir);
                    break;
                }
            }

            return cache[targetFolder];
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
