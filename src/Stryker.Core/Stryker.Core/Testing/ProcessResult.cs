using System.Diagnostics.CodeAnalysis;

namespace Stryker.Configuration.Testing
{
    [ExcludeFromCodeCoverage]
    public class ProcessResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }
    }
}
