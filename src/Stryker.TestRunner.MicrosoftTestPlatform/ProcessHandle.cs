using System.Diagnostics.CodeAnalysis;
using CliWrap;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

[ExcludeFromCodeCoverage]
public class ProcessHandle(CommandTask<CommandResult> commandTask, Stream output) : IProcessHandle
{
    public int Id { get; } = commandTask.ProcessId;
    public string ProcessName { get; } = "dotnet";
    public int ExitCode { get; private set; }
    public TextWriter StandardInput => new StringWriter();
    public TextReader StandardOutput => new StreamReader(output);
    public void Dispose()
    {
        commandTask.Dispose();
    }

    public void Kill()
    {
        Dispose();
    }

    public Task<int> StopAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<int> WaitForExitAsync()
    {
        var commandResult = await commandTask;
        return ExitCode = commandResult.ExitCode;
    }

    public Task WriteInputAsync(string input)
    {
        return Task.CompletedTask;
    }
}
