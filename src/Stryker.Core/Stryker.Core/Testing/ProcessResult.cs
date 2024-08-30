using System.Diagnostics.CodeAnalysis;

namespace Stryker.Abstractions.Testing
{
    [ExcludeFromCodeCoverage]
    public class ProcessResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }
    }
}
