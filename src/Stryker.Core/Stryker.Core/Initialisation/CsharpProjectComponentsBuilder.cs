using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.MutantFilters.Extensions;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.SourceProjects;

namespace Stryker.Core.Initialisation
{
    public class CsharpProjectComponentsBuilder : ProjectComponentsBuilder
    {
        private readonly SourceProjectInfo _projectInfo;
        private readonly StrykerOptions _options;
        private readonly string[] _foldersToExclude;
        private readonly ILogger _logger;

        public CsharpProjectComponentsBuilder(SourceProjectInfo projectInfo, StrykerOptions options, string[] foldersToExclude, ILogger logger, IFileSystem fileSystem) : base(fileSystem)
        {
            _projectInfo = projectInfo;
            _options = options;
            _foldersToExclude = foldersToExclude;
            _logger = logger;
        }

        public override IProjectComponent Build()
        {
            CsharpFolderComposite inputFiles;
            if (_projectInfo.AnalyzerResult.SourceFiles != null && _projectInfo.AnalyzerResult.SourceFiles.Any())
            {
                inputFiles = FindProjectFilesUsingBuildalyzer(_projectInfo.AnalyzerResult, _options);
            }
            else
            {
                _logger.LogWarning("Buildalyzer could not find sourcefiles. This should not happen. Will fallback to filesystem scan. Please report an issue at github.");
                inputFiles = FindProjectFilesScanningProjectFolders(_projectInfo.AnalyzerResult, _options);
            }
            return inputFiles;
        }

        // Deprecated method, should not be maintained
        private CsharpFolderComposite FindProjectFilesScanningProjectFolders(IAnalyzerResult analyzerResult, StrykerOptions options)
        {
            var inputFiles = new CsharpFolderComposite();
            var sourceProjectDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            foreach (var dir in ExtractProjectFolders(analyzerResult))
            {
                var folder = FileSystem.Path.Combine(Path.GetDirectoryName(sourceProjectDir), dir);

                _logger.LogDebug($"Scanning {folder}");
                if (!FileSystem.Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException($"Can't find {folder}");
                }

                inputFiles.Add(FindInputFiles(folder, sourceProjectDir, analyzerResult, options));
            }

            return inputFiles;
        }

        private CsharpFolderComposite FindProjectFilesUsingBuildalyzer(IAnalyzerResult analyzerResult, StrykerOptions options)
        {
            var sourceProjectDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            var generatedAssemblyInfo = analyzerResult.AssemblyAttributeFileName();
            var projectUnderTestFolderComposite = new CsharpFolderComposite()
            {
                FullPath = sourceProjectDir,
                RelativePath = Path.GetDirectoryName(sourceProjectDir),
            };
            var cache = new Dictionary<string, CsharpFolderComposite> { [string.Empty] = projectUnderTestFolderComposite };

            // Save cache in a singleton so we can use it in other parts of the project
            FolderCompositeCache<CsharpFolderComposite>.Instance.Cache = cache;

            var cSharpParseOptions = analyzerResult.GetParseOptions(options);
            InjectMutantHelpers(projectUnderTestFolderComposite, cSharpParseOptions);

            foreach (var sourceFile in analyzerResult.SourceFiles)
            {
                var relativePath = Path.GetRelativePath(sourceProjectDir, sourceFile);
                var folderComposite = GetOrBuildFolderComposite(cache, Path.GetDirectoryName(relativePath), sourceProjectDir, projectUnderTestFolderComposite);

                var file = new CsharpFileLeaf()
                {
                    SourceCode = FileSystem.File.ReadAllText(sourceFile),
                    FullPath = sourceFile,
                    RelativePath = relativePath
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
                    if (FileSystem.Path.GetFileName(sourceFile).ToLowerInvariant() == generatedAssemblyInfo)
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
        private CsharpFolderComposite FindInputFiles(string path, string sourceProjectDir, IAnalyzerResult analyzerResult, StrykerOptions options)
        {
            var rootFolderComposite = new CsharpFolderComposite
            {
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.GetRelativePath(sourceProjectDir, Path.GetFullPath(path))
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
        private CsharpFolderComposite FindInputFiles(string path, string sourceProjectDir, CSharpParseOptions cSharpParseOptions)
        {

            var folderComposite = new CsharpFolderComposite
            {
                FullPath = Path.GetFullPath(path),
                RelativePath = Path.GetRelativePath(sourceProjectDir, Path.GetFullPath(path))
            };

            foreach (var folder in FileSystem.Directory.EnumerateDirectories(folderComposite.FullPath).Where(x => !_foldersToExclude.Contains(Path.GetFileName(x))))
            {
                folderComposite.Add(FindInputFiles(folder, sourceProjectDir, cSharpParseOptions));
            }

            foreach (var file in FileSystem.Directory.GetFiles(folderComposite.FullPath, "*.cs", SearchOption.TopDirectoryOnly).Where(f => !f.EndsWith(".xaml.cs")))
            {
                // Roslyn cannot compile xaml.cs files generated by xamarin. 
                // Since the files are generated they should not be mutated anyway, so skip these files.

                var fileLeaf = new CsharpFileLeaf()
                {
                    SourceCode = FileSystem.File.ReadAllText(file),
                    FullPath = file,
                    RelativePath = Path.GetRelativePath(sourceProjectDir, file)
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

        private void InjectMutantHelpers(CsharpFolderComposite rootFolderComposite, CSharpParseOptions cSharpParseOptions)
        {
            foreach (var (name, code) in _projectInfo.CodeInjector.MutantHelpers)
            {
                rootFolderComposite.AddCompilationSyntaxTree(CSharpSyntaxTree.ParseText(code, path: name, encoding: Encoding.UTF32, options: cSharpParseOptions));
            }
        }

        private static CSharpParseOptions BuildCsharpParseOptions(IAnalyzerResult analyzerResult, StrykerOptions options) => new(options.LanguageVersion, DocumentationMode.None, preprocessorSymbols: analyzerResult.PreprocessorSymbols);

        // get the FolderComposite object representing the the project's folder 'targetFolder'. Build the needed FolderComposite(s) for a complete path
        private CsharpFolderComposite GetOrBuildFolderComposite(IDictionary<string, CsharpFolderComposite> cache, string targetFolder, string sourceProjectDir, ProjectComponent<SyntaxTree> inputFiles)
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
                    var fullPath = FileSystem.Path.Combine(sourceProjectDir, folder);
                    var newComposite = new CsharpFolderComposite
                    {
                        FullPath = fullPath,
                        RelativePath = Path.GetRelativePath(sourceProjectDir, fullPath),
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
                        var root = inputFiles as IReadOnlyFolderComposite;
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
    }
}
