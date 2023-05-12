namespace Stryker.Core.Compiling;

public class CompilingProcessResult
{
    public bool Success { get; set; }
    public RollbackProcessResult RollbackResult { get; set; }
}
