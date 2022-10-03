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
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.MutationTest
{
    public class FsharpMutationProcess : IMutationProcess
    {
        private readonly ProjectComponent<ParsedInput> _projectInfo;
        private readonly ILogger _logger;
        private readonly StrykerOptions _options;
        private readonly FsharpCompilingProcess _compilingProcess;
        private readonly BaseMutantOrchestrator<FSharpList<SynModuleOrNamespace>> _orchestrator;

        /// <summary>
        /// This constructor is for tests
        /// </summary>
        /// <param name="mutationTestInput"></param>
        /// <param name="fileSystem"></param>
        /// <param name="options"></param>
        /// <param name="orchestrator"></param>
        public FsharpMutationProcess(MutationTestInput mutationTestInput,
            IFileSystem fileSystem,
            StrykerOptions options,
            BaseMutantOrchestrator<FSharpList<SynModuleOrNamespace>> orchestrator)
        {
            var input = mutationTestInput;
            _projectInfo = (ProjectComponent<ParsedInput>)input.SourceProjectInfo.ProjectContents;

            _options = options;
            _orchestrator = orchestrator ?? new FsharpMutantOrchestrator(options: _options);
            _compilingProcess = new FsharpCompilingProcess(mutationTestInput, new RollbackProcess(), fileSystem ?? new FileSystem());
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        /// <summary>
        /// This constructor is used by the <see cref="MutationTestProcess"/> initialization logic.
        /// </summary>
        /// <param name="mutationTestInput"></param>
        /// <param name="options"></param>
        public FsharpMutationProcess(MutationTestInput mutationTestInput,
            StrykerOptions options): this(mutationTestInput, null, options, null){}

        public void Mutate()
        {
            // Mutate source files
            foreach (var file in _projectInfo.GetAllFiles().Cast<FsharpFileLeaf>())
            {
                _logger.LogDebug($"Mutating {file.RelativePath}");
                // Mutate the syntax tree
                var treeRoot = ((ParsedInput.ImplFile)file.SyntaxTree).Item.modules;
                var mutatedSyntaxTree = _orchestrator.Mutate(treeRoot);
                // Add the mutated syntax tree for compilation
                var tree = (ParsedInput.ImplFile)file.SyntaxTree;
                var item = tree.Item;
                //we hardcode the lastcompiled flag to make the compile pass
                //this needs to be fixed in the FSharp.Compiler.SourceCodeServices package, or made dynamic as it now assumes the bottom of Program.fs is the entry point
                var lastCompile = item.fileName.Equals("Program.fs") ? new Tuple<bool, bool>(true, true) : item.isLastCompiland;
                file.MutatedSyntaxTree = ParsedInput.NewImplFile(ParsedImplFileInput.NewParsedImplFileInput(item.fileName, item.isScript, item.qualifiedNameOfFile, item.scopedPragmas, item.hashDirectives, mutatedSyntaxTree, lastCompile));
                if (_options.DevMode)
                {
                    _logger.LogTrace($"Mutated {file.RelativePath}:{Environment.NewLine}{mutatedSyntaxTree}");
                }
                // Filter the mutants
                var allMutants = _orchestrator.GetLatestMutantBatch();
                file.Mutants = allMutants;
            }

            _logger.LogDebug("{0} mutants created", _projectInfo.Mutants.Count());

            CompileMutations();
        }

        private void CompileMutations()
        {
            using var ms = new MemoryStream();
            using var msForSymbols = _options.DevMode ? new MemoryStream() : null;
            // compile the mutated syntax trees
            var compileResult = _compilingProcess.Compile(_projectInfo.CompilationSyntaxTrees, _options.DevMode);

            // if a rollback took place, mark the rolled back mutants as status:BuildError
            if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
            {
                foreach (var mutant in _projectInfo.Mutants
                    .Where(x => compileResult.RollbackResult.RollbackedIds.Contains(x.Id)))
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

        public void FilterMutants()
        {
        }
    }
}
