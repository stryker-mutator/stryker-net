using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners.UnityTestRunner.RunUnity.UnityPath;

namespace Stryker.Core.TestRunners.UnityTestRunner.RunUnity;

public class RunUnity : IRunUnity
{
    private const int TestFailedExitCode = 2;

    private readonly IProcessExecutor _processExecutor;
    private readonly IUnityPath _unityPath;
    private readonly ILogger _logger;

    private static RunUnity instance;
    private bool _unityInProgress;
    private string _currentUnityRunArguments;

    private string _pathToUnityListenFile;
    private string _pathToActiveMutantsListenFile;
    private Task _unityProcessTask;


    public RunUnity(IProcessExecutor processExecutor, IUnityPath unityPath, ILogger logger)
    {
        _processExecutor = processExecutor;
        _unityPath = unityPath;
        _logger = logger;
    }

    public static RunUnity GetSingleInstance(Func<IProcessExecutor> processExecutor = null,
        Func<IUnityPath> unityPath = null,
        Func<ILogger> logger = null)
    {
        if (instance != null) return instance;

        instance = new RunUnity(processExecutor?.Invoke() ?? new ProcessExecutor(),
            unityPath?.Invoke() ?? new UnityPath.UnityPath(new FileSystem()),
            logger?.Invoke() ?? ApplicationLogging.LoggerFactory.CreateLogger<RunUnity>());

        return instance;
    }


    public void ReloadDomain(StrykerOptions strykerOptions, string projectPath, string additionalArgumentsForCli = null)
    {
        _logger.LogDebug("Request to reload domain");

        TryOpenUnity(strykerOptions, projectPath, additionalArgumentsForCli);
        SendCommandToUnity("reloadDomain");
        WaitUntilEndOfCommand();
        ThrowExceptionIfExists();
    }

    public XDocument RunTests(StrykerOptions strykerOptions, string projectPath,
        string additionalArgumentsForCli = null, string activeMutantId = null)
    {
        _logger.LogDebug("Request to close Unity");

        TryOpenUnity(strykerOptions, projectPath, additionalArgumentsForCli);

        var pathToTestResultXml =
            Path.Combine(strykerOptions.OutputPath, $"test_results_{DateTime.Now.ToFileTime()}.xml");

        File.WriteAllText(_pathToActiveMutantsListenFile, activeMutantId);

        SendCommandToUnity(pathToTestResultXml);
        WaitUntilEndOfCommand();
        ThrowExceptionIfExists();

        return XDocument.Load(pathToTestResultXml);
    }


    public void Dispose()
    {
        ThrowExceptionIfExists();

        CloseUnity();
    }


    private void TryOpenUnity(StrykerOptions strykerOptions, string projectPath,
        string additionalArgumentsForCli = null)
    {
        ThrowExceptionIfExists();

        if (_unityInProgress && _currentUnityRunArguments != GetArgumentsToRun(projectPath, additionalArgumentsForCli))
        {
            _logger.LogError(
                "Trying to run unity with other arguments when instance already opened. Waiting for closing the current one.");
            CloseUnity();
        }
        else if (_unityInProgress)
        {
            _logger.LogDebug("Trying to run unity when instance already opened. Nothing to do");
            return;
        }


        OpenUnity(strykerOptions, projectPath, additionalArgumentsForCli);
    }

    private void OpenUnity(StrykerOptions strykerOptions, string projectPath, string additionalArgumentsForCli = null)
    {
        _logger.LogDebug("OpenUnity started");

        _unityInProgress = true;

        _pathToUnityListenFile = Path.Combine(strykerOptions.OutputPath, "UnityListens.txt");
        Environment.SetEnvironmentVariable("Stryker.Unity.PathToListen", _pathToUnityListenFile);
        _pathToActiveMutantsListenFile = Path.Combine(strykerOptions.OutputPath, "ActiveMutantsListens.txt");
        Environment.SetEnvironmentVariable("ActiveMutationPath", _pathToActiveMutantsListenFile);

        CleanupCommandBuffer();

        var pathToUnityLogFile =
            Path.Combine(strykerOptions.OutputPath, "logs", "unity_" + DateTime.Now.ToFileTime() + ".log");

        _currentUnityRunArguments = GetArgumentsToRun(projectPath, additionalArgumentsForCli);

        _unityProcessTask = Task.Run(() =>
        {
            var processResult = _processExecutor.Start(".", _unityPath.GetPath(strykerOptions),
                $"-logFile {pathToUnityLogFile} " + _currentUnityRunArguments);
            _logger.LogDebug("OpenUnity finished");
            _unityInProgress = false;

            if (processResult.ExitCode != 0 && processResult.ExitCode != TestFailedExitCode)
            {
                throw new UnityExecuteException(processResult.ExitCode, pathToUnityLogFile);
            }
        });
    }

    private string GetArgumentsToRun(string projectPath, string additionalArgumentsForCli = null) =>
        $"-batchmode -projectPath={projectPath} " +
        additionalArgumentsForCli;

    private void SendCommandToUnity(string command)
    {
        File.WriteAllText(_pathToUnityListenFile, command);
    }

    private void CleanupCommandBuffer()
    {
        File.WriteAllText(_pathToUnityListenFile, string.Empty);
    }

    private void WaitUntilEndOfCommand()
    {
        while (string.IsNullOrWhiteSpace(File.ReadAllText(_pathToUnityListenFile)) == false)
        {
            ThrowExceptionIfExists();
        }
    }

    private void ThrowExceptionIfExists()
    {
        if (_unityProcessTask?.Exception != null)
        {
            throw _unityProcessTask.Exception;
        }
    }

    private void CloseUnity()
    {
        if (!_unityInProgress)
        {
            _logger.LogDebug("Request to close Unity. Unity is not running. Do nothing");
            return;
        }

        _logger.LogDebug("Request to close Unity");

        SendCommandToUnity("exit");
        _unityProcessTask.GetAwaiter().GetResult();
    }

    private void RemoveUnityCompileCache(string unityProjectPath)
    {
        var cachePath = Path.Combine(unityProjectPath, "Library", "ScriptAssemblies");
        if (Directory.Exists(cachePath))
        {
            Directory.Delete(cachePath, true);
        }
    }
}
