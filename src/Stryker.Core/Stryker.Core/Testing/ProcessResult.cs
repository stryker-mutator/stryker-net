using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Testing
{
    [ExcludeFromCodeCoverage]
    public class ProcessResult
    {
        public int ExitCode;
        public string Output;
        public string Error;
    }
}
