namespace Stryker.Core.Testing;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class ProcessResult
{
    public int ExitCode { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }
}
