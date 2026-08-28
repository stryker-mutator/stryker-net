using System;
using System.Diagnostics;
using System.IO;

namespace Stryker.CLI;

/// <summary>
/// Works around a .NET runtime crash on macOS (dotnet/runtime#132581): when the process that
/// launched Stryker had an SA_SIGINFO handler installed for SIGUSR1 (debuggers, Node/Electron
/// hosts such as VS Code, Go-based CI runners), macOS leaks SA_SIGINFO in sa_flags across execve
/// while resetting the handler to SIG_DFL. CoreCLR saves that poisoned disposition as the
/// "previous" SIGUSR1 action, and when the kernel misattributes one of the runtime's own
/// thread-suspension signals under a SIGCHLD storm (which Stryker generates by killing testhost
/// process trees), the runtime chains to the NULL handler and dies with SIGSEGV at pc=0.
///
/// Re-executing ourselves fixes this: a child spawned through .NET's fork/exec starts with fully
/// reset signal dispositions, so the poisoned state cannot reach the new runtime.
/// </summary>
public static class MacOsSignalWorkaround
{
    private const string ReExecMarker = "STRYKER_MACOS_SIGNAL_REEXEC";
    private const string OptOut = "STRYKER_DISABLE_MACOS_SIGNAL_WORKAROUND";

    /// <summary>
    /// Re-executes the current process on macOS and relays its exit code. Returns false when no
    /// re-exec is needed (not macOS, already re-executed, opted out, or a debugger is attached —
    /// re-executing would detach the debugger from the process doing the real work).
    /// </summary>
    public static bool TryReExec(string[] args, out int exitCode)
    {
        exitCode = 0;
        if (!OperatingSystem.IsMacOS()
            || Environment.GetEnvironmentVariable(ReExecMarker) is not null
            || Environment.GetEnvironmentVariable(OptOut) is not null
            || Debugger.IsAttached
            || Environment.ProcessPath is not { } processPath)
        {
            return false;
        }

        var startInfo = new ProcessStartInfo(processPath) { UseShellExecute = false };
        if (Path.GetFileNameWithoutExtension(processPath) is "dotnet")
        {
            // Launched as `dotnet <assembly.dll> ...`: the assembly path is argv[0], not part of args
            startInfo.ArgumentList.Add(Environment.GetCommandLineArgs()[0]);
        }
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }
        startInfo.Environment[ReExecMarker] = "1";

        // Ctrl+C is delivered to the whole foreground process group; the child handles it and
        // exits, the parent must survive long enough to relay the child's exit code.
        Console.CancelKeyPress += (_, e) => e.Cancel = true;

        using var child = Process.Start(startInfo);
        if (child is null)
        {
            return false;
        }

        child.WaitForExit();
        exitCode = child.ExitCode;
        return true;
    }
}
