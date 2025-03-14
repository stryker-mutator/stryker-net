using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Helpers.ProcessUtil;

[ExcludeFromCodeCoverage]
public class ProcessResult
{
    public int ExitCode { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }
}
