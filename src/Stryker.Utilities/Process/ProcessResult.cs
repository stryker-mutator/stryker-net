using System.Diagnostics.CodeAnalysis;

namespace Stryker.Utilities.Process;

[ExcludeFromCodeCoverage]
public class ProcessResult
{
    public int ExitCode { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }
}
