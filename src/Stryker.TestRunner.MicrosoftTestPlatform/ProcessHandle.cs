using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CliWrap;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

[ExcludeFromCodeCoverage]
public class ProcessHandle(CommandTask<CommandResult> commandTask, Stream output) : IProcessHandle, IDisposable
{
    private bool _disposed;

    public int Id { get; } = commandTask.ProcessId;
    public string ProcessName { get; } = "dotnet";
    public int ExitCode { get; private set; }
    public TextWriter StandardInput => new StringWriter();
    public TextReader StandardOutput
    {
        get
        {
            // Output is rarely consumed, and when it is, it's from a file stream
            if (output.CanSeek && output.Position != 0)
            {
                output.Position = 0;
            }
            return new StreamReader(output);
        }
    }

    public void Kill()
    {
        try
        {
            var process = Process.GetProcessById(Id);
            process.Kill(entireProcessTree: true);
        }
        catch (Exception)
        {
            // Process may have already exited
        }
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
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // CliWrap's CommandTask.Dispose() throws if the task hasn't completed.
            // Kill the process first, then wait briefly for the task to complete before disposing.
            if (!commandTask.Task.IsCompleted)
            {
                Kill();
                try
                {
                    commandTask.Task.Wait(TimeSpan.FromSeconds(5));
                }
                catch (Exception)
                {
                    // Process may not finish in time; proceed with disposal anyway
                }
            }

            try
            {
                commandTask.Dispose();
            }
            catch (InvalidOperationException)
            {
                // Task may still not be in a completion state after kill
            }
        }
        
        _disposed = true;
    }
}
