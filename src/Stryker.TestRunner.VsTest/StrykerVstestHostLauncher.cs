using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Stryker.Utilities.Logging;

namespace Stryker.TestRunner.VsTest;

public interface IStrykerTestHostLauncher : ITestHostLauncher
{
}

// can't be unit tested
[ExcludeFromCodeCoverage]
public class StrykerVsTestHostLauncher : IStrykerTestHostLauncher
{
    private readonly string _id;
    private readonly bool _devMode;

    private static ILogger Logger { get; }

    static StrykerVsTestHostLauncher() =>
        Logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerVsTestHostLauncher>();

    public StrykerVsTestHostLauncher(string id, bool devMode)
    {
        _id = id;
        _devMode = devMode;
    }

    public bool IsDebug => false;

    public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo) =>
        LaunchTestHost(defaultTestHostStartInfo, CancellationToken.None);

    public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo, CancellationToken cancellationToken)
    {
        var processInfo =
            new ProcessStartInfo(defaultTestHostStartInfo.FileName, defaultTestHostStartInfo.Arguments)
            {
                WorkingDirectory = defaultTestHostStartInfo.WorkingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

        ExternalEnvironmentVariables.Add(processInfo.Environment);

        var currentProcess = new Process { StartInfo = processInfo, EnableRaisingEvents = true };

        if (_devMode)
        {
            currentProcess.OutputDataReceived += Process_OutputDataReceived;
            currentProcess.ErrorDataReceived += Process_ErrorDataReceived;
        }

        if (!currentProcess.Start())
        {
            Logger.LogError("{Id}: Failed to start process {Arguments}.", _id, processInfo.Arguments);
        }

        currentProcess.BeginOutputReadLine();
        currentProcess.BeginErrorReadLine();

        return currentProcess.Id;
    }

    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            Logger.LogDebug("{Id}: {Data} (VsTest error)", _id, e.Data);
        }
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            Logger.LogTrace("{Id}: {Data} (VsTest output)", _id, e.Data);
        }
    }
}
