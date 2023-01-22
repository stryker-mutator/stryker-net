using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners.UnityTestRunner.RunUnity.UnityPath;

namespace Stryker.Core.TestRunners.UnityTestRunner.RunUnity;

public class RunUnity : IRunUnity
{
    private readonly IProcessExecutor _processExecutor;
    private readonly IUnityPath _unityPath;
    private readonly ILogger _logger;

    private static RunUnity instance;
    private bool _unityInProgress;

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
        if (instance == null)
        {
            instance = new RunUnity(processExecutor?.Invoke() ?? new ProcessExecutor(),
                unityPath?.Invoke() ?? new UnityPath.UnityPath(new FileSystem()),
                logger?.Invoke() ?? ApplicationLogging.LoggerFactory.CreateLogger<RunUnity>());
        }

        return instance;
    }

    public ProcessResult RunUnityUntilFinish(StrykerOptions strykerOptions, string projectPath,
        string runArgumentsForCli)
    {
        if (_unityInProgress)
        {
            _logger.LogError("Trying to run unity when instance already opened. Waiting for closing the current one.");
            //todo quit the process and start a new one
        }

        _unityInProgress = true;

        var pathToUnityLogFile =
            Path.Combine(strykerOptions.OutputPath, "logs", "unity_" + DateTime.Now.ToFileTime() + ".log");
        _logger.LogDebug("RunUnity started");

        var processResult = _processExecutor.Start(".", _unityPath.GetPath(strykerOptions),
            @$"-batchmode -projectPath={projectPath} -logFile {pathToUnityLogFile} " + runArgumentsForCli);
        _logger.LogDebug("RunUnity finished");

        _unityInProgress = false;

        return processResult;
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
