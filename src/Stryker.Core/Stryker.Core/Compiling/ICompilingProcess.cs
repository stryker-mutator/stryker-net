using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;

namespace Stryker.Core.Compiling
{
    public interface ICompilingProcess
    {
        /// <summary>
        /// Compiles the given input onto the memorysteam
        /// </summary>
        /// <param name="ms">The memorystream to function as output</param>
        CompilingProcessResult Compile(IEnumerable<SyntaxTree> syntaxTrees, MemoryStream ms);
    }
}