using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.MutationTest
{
    class MutationTestProcessMethodFsharp : IMutationTestProcessMethod
    {
        private readonly ProjectComponent<ParsedInput> _projectInfo;
        private readonly ILogger _logger;
        private readonly StrykerOptions _options;
        private readonly CompilingProcessFsharp _compilingProcess;
        private readonly IFileSystem _fileSystem;
        private readonly MutationTestInput _input;
        private readonly IMutantOrchestrator _orchestrator;

        private readonly IMutantFilter _mutantFilter;
        private readonly IReporter _reporter;

        public MutationTestProcessMethodFsharp(MutationTestInput mutationTestInput,
            IMutantOrchestrator orchestrator = null,
            IFileSystem fileSystem = null,
            StrykerOptions options = null,

            IMutantFilter mutantFilter = null,
            IReporter reporter = null)
        {
            _input = mutationTestInput;
            _projectInfo = (ProjectComponent<ParsedInput>)mutationTestInput.ProjectInfo.ProjectContents;
            _options = options;
            _orchestrator = orchestrator ?? new MutantOrchestrator(options: _options);
            _compilingProcess = new CompilingProcessFsharp(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();

            _mutantFilter = mutantFilter ?? MutantFilterFactory.Create(options);
            _reporter = reporter;
        }

        public void Mutate()
        {
            throw new NotImplementedException();
        }
        public void FilterMutants()
        {
            throw new NotImplementedException();
        }
    }
}
