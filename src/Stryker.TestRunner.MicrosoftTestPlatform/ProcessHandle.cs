using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CliWrap;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

[ExcludeFromCodeCoverage]
public class ProcessHandle(CommandTask<CommandResult> commandTask, Stream output) : IProcessHandle, IDisposable
{
    private bool _disposed;
    private volatile bool _exitVerified;
    private readonly Process? _process = CaptureProcess(commandTask);

    public int Id { get; } = commandTask.ProcessId;
    public string ProcessName { get; } = "dotnet";
    public int ExitCode { get; private set; }
    public bool HasExited
    {
        get
        {
            if (_exitVerified || _process is null)
            {
                return true;
            }
            if (_process.HasExited)
            {
                _exitVerified = true;
                return true;
            }
            return false;
        }
    }

    private static Process? CaptureProcess(CommandTask<CommandResult> command)
    {
        if (command.Task.IsCompleted)
        {
            return null;
        }
        try
        {
            var process = Process.GetProcessById(command.ProcessId);
            // Retain the OS handle rather than reopening a possibly recycled PID during teardown.
            _ = process.Handle;
            return process;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
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
            if (_process is not null && !HasExited)
            {
                _process.Kill(entireProcessTree: true);
            }
        }
        catch (ArgumentException)
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

            if (!HasExited)
            {
                throw new InvalidOperationException("Cannot dispose an MTP process handle before its exit has been verified.");
            }
            try
            {
                commandTask.Dispose();
            }
            catch (InvalidOperationException)
            {
                // Task may still not be in a completion state after kill
            }
            _process?.Dispose();
        }

        _disposed = true;
    }
}
