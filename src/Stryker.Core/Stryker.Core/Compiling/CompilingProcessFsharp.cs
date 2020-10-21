using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.Compiling
{
    class CompilingProcessFsharp
    {
        private readonly MutationTestInput _input;
        private readonly IRollbackProcess _rollbackProcess;
        private readonly ILogger _logger;

        public CompilingProcessFsharp(MutationTestInput input,
            IRollbackProcess rollbackProcess)
        {
            _input = input;
            _rollbackProcess = rollbackProcess;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<CompilingProcess>();
        }

        private string AssemblyName =>
            _input.ProjectInfo.ProjectUnderTestAnalyzerResult.AssemblyName;

        public CompilingProcessResult Compile(IEnumerable<ParsedInput> syntaxTrees, Stream ilStream, Stream memoryStream, bool devMode)
        {
            throw new NotImplementedException();
        }
    }
}
