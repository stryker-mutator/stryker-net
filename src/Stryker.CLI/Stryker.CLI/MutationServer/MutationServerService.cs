using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Stryker.CLI.CommandLineConfig;
using Stryker.CLI.Logging;
using Stryker.Configuration.Options;
using Stryker.Core;

namespace Stryker.CLI.MutationServer;

internal sealed class MutationServerService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigBuilder _configBuilder;
    private readonly ILoggingInitializer _loggingInitializer;
    private readonly IAnsiConsole _console;
    private readonly SemaphoreSlim _runLock = new(1, 1);
    private string _configFilePath;
    private string[] _serverArguments = [];

    public MutationServerService(
        IServiceProvider serviceProvider,
        IConfigBuilder configBuilder,
        ILoggingInitializer loggingInitializer,
        IAnsiConsole console)
    {
        _serviceProvider = serviceProvider;
        _configBuilder = configBuilder;
        _loggingInitializer = loggingInitializer;
        _console = console;
    }

    public void SetServerArguments(IReadOnlyCollection<string> serverArguments)
    {
        _serverArguments = serverArguments?.ToArray() ?? [];
        _configFilePath = null;
    }

    public async Task WaitForIdleAsync()
    {
        await _runLock.WaitAsync().ConfigureAwait(false);
        _runLock.Release();
    }

    public ConfigureResult Configure(ConfigureParams parameters)
    {
        _configFilePath = parameters?.ConfigFilePath;
        return new ConfigureResult { Version = MutationServerProtocol.Version };
    }

    public async Task<DiscoverResult> DiscoverAsync(
        DiscoverParams parameters,
        CancellationToken cancellationToken)
    {
        await _runLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var inputs = BuildInputs();
            var scope = MutationServerScope.ForDiscovery(inputs.BasePathInput.SuppliedInput, parameters);
            var reporter = new MutationServerReporter(
                inputs.BasePathInput.SuppliedInput,
                mutantSelection: scope.Includes,
                fileSelection: scope.IncludesFile);
            using var serviceScope = _serviceProvider.CreateScope();
            var runner = serviceScope.ServiceProvider.GetRequiredService<IStrykerRunner>();

            await runner.DiscoverMutantsAsync(
                inputs,
                reporter,
                scope.Includes,
                cancellationToken).ConfigureAwait(false);
            return reporter.DiscoverResult;
        }
        finally
        {
            _runLock.Release();
        }
    }

    public async Task<MutationTestResult> MutationTestAsync(
        MutationTestParams parameters,
        Func<MutationTestResult, Task> reportProgress,
        CancellationToken cancellationToken)
    {
        await _runLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var inputs = BuildInputs();
            var scope = MutationServerScope.ForMutationTest(inputs.BasePathInput.SuppliedInput, parameters);
            var reporter = new MutationServerReporter(
                inputs.BasePathInput.SuppliedInput,
                reportProgress: reportProgress,
                mutantSelection: scope.Includes,
                fileSelection: scope.IncludesFile,
                cancellationToken: cancellationToken);
            using var serviceScope = _serviceProvider.CreateScope();
            var runner = serviceScope.ServiceProvider.GetRequiredService<IStrykerRunner>();

            try
            {
                await runner.RunMutationTestAsync(
                    inputs,
                    reporter,
                    scope.Includes,
                    cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await reporter.CompleteAsync().ConfigureAwait(false);
                throw;
            }

            await reporter.CompleteAsync().ConfigureAwait(false);
            if (reporter.ProgressException is not null)
            {
                throw new InvalidOperationException(
                    "Failed to report mutation test progress.",
                    reporter.ProgressException);
            }

            return new MutationTestResult();
        }
        finally
        {
            _runLock.Release();
        }
    }

    private StrykerInputs BuildInputs()
    {
        var inputs = new StrykerInputs();
        var app = new CommandLineApplication();
        var commandLineConfigReader = new CommandLineConfigReader(_console);
        commandLineConfigReader.RegisterCommandLineOptions(app, inputs);

        var arguments = ApplyConfiguredFile(_serverArguments, _configFilePath);
        _configBuilder.Build(inputs, arguments, app, commandLineConfigReader);
        _loggingInitializer.InitializeOutputPath(
            inputs,
            writeWarning: warning => Console.Error.WriteLine(warning));
        return inputs;
    }

    private static string[] ApplyConfiguredFile(IEnumerable<string> arguments, string configFilePath)
    {
        if (string.IsNullOrWhiteSpace(configFilePath))
        {
            return arguments.ToArray();
        }

        var result = new List<string>();
        using var enumerator = arguments.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var argument = enumerator.Current;
            var separatorIndex = argument.IndexOfAny([':', '=']);
            var optionName = separatorIndex >= 0 ? argument[..separatorIndex] : argument;
            if (optionName is "-f" or "--config-file")
            {
                if (separatorIndex < 0)
                {
                    enumerator.MoveNext();
                }
                continue;
            }

            result.Add(argument);
        }

        result.Add("--config-file");
        result.Add(configFilePath);

        return result.ToArray();
    }
}
