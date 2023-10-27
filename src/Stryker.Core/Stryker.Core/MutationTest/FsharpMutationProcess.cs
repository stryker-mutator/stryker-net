using FSharp.Compiler.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Stryker.Core.Compiling;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Stryker.Core.MutationTest
{
    public class FsharpMutationProcess : IMutationProcess
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly StrykerOptions _options;
        private readonly BaseMutantOrchestrator<FSharpList<SynModuleOrNamespace>, object> _orchestrator;

        /// <summary>
        /// This constructor is for tests
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="options"></param>
        /// <param name="orchestrator"></param>
        public FsharpMutationProcess(
            IFileSystem fileSystem,
            StrykerOptions options,
            BaseMutantOrchestrator<FSharpList<SynModuleOrNamespace>, object> orchestrator)
        {
            _fileSystem = fileSystem;
            _options = options;
            _orchestrator = orchestrator ?? new FsharpMutantOrchestrator(options: _options);
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        /// <summary>
        /// This constructor is used by the <see cref="MutationTestProcess"/> initialization logic.
        /// </summary>
        /// <param name="options"></param>
        public FsharpMutationProcess(StrykerOptions options): this(null, options, null){}

        public void Mutate(MutationTestInput input)
        {
            var projectInfo = (ProjectComponent<ParsedInput>)input.SourceProjectInfo.ProjectContents;
            // Mutate source files
            foreach (var file in projectInfo.GetAllFiles().Cast<FsharpFileLeaf>())
            {
                _logger.LogDebug($"Mutating {file.RelativePath}");
                // Mutate the syntax tree
                var treeRoot = ((ParsedInput.ImplFile)file.SyntaxTree).Item.modules;
                var mutatedSyntaxTree = _orchestrator.Mutate(treeRoot, null);
                // Add the mutated syntax tree for compilation
                var tree = (ParsedInput.ImplFile)file.SyntaxTree;
                var item = tree.Item;
                //we hard code the lastCompiled flag to make the compile pass
                //this needs to be fixed in the FSharp.Compiler.SourceCodeServices package, or made dynamic as it now assumes the bottom of Program.fs is the entry point
                var lastCompile = item.fileName.Equals("Program.fs") ? new Tuple<bool, bool>(true, true) : item.isLastCompiland;

                var inputFile = ParsedImplFileInput.NewParsedImplFileInput(
                    item.fileName,
                    item.isScript,
                    item.qualifiedNameOfFile,
                    item.scopedPragmas,
                    item.hashDirectives,
                    mutatedSyntaxTree,
                    lastCompile,
                    item.trivia);
                file.MutatedSyntaxTree = ParsedInput.NewImplFile(inputFile);

                if (_options.DevMode)
                {
                    _logger.LogTrace($"Mutated {file.RelativePath}:{Environment.NewLine}{mutatedSyntaxTree}");
                }
                // Filter the mutants
                var allMutants = _orchestrator.GetLatestMutantBatch();
                file.Mutants = allMutants;
            }

            _logger.LogDebug("{0} mutants created", projectInfo.Mutants.Count());

            CompileMutations(input);
        }

        private void CompileMutations(MutationTestInput mutationTestInput)
        {
            var projectInfo = (ProjectComponent<ParsedInput>)mutationTestInput.SourceProjectInfo.ProjectContents;
            using var ms = new MemoryStream();
            using var msForSymbols = _options.DevMode ? new MemoryStream() : null;
            // compile the mutated syntax trees
            var compilingProcess = new FsharpCompilingProcess(mutationTestInput, _fileSystem ?? new FileSystem());
            var compileResult = compilingProcess.Compile(projectInfo.CompilationSyntaxTrees, _options.DevMode);
        }

        public void FilterMutants(MutationTestInput input)
        {
            // mutation filtering logic has not been implemented
        }
    }
}
