﻿using System;
using System.Collections;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class OptionsBuilder
    {
        private readonly ILogger _logger;
        private IConfiguration _config;

        public OptionsBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public StrykerOptions Build(
            string basePath,
            CommandOption reporter,
            CommandOption dashboardApiKey,
            CommandOption reportersProjectName,
            CommandOption reportersModuleName,
            CommandOption reportersProjectVersion,
            CommandOption projectUnderTestNameFilter,
            CommandOption additionalTimeoutMS,
            CommandOption excludedMutations,
            CommandOption ignoreMethods,
            CommandOption logLevel,
            CommandOption logToFile,
            CommandOption devMode,
            CommandOption coverageAnalysis,
            CommandOption abortTestOnFail,
            CommandOption configFilePath,
            CommandOption maxConcurrentTestRunners,
            CommandOption thresholdHigh,
            CommandOption thresholdLow,
            CommandOption thresholdBreak,
            CommandOption filesToExclude,
            CommandOption filePatterns,
            CommandOption testRunner,
            CommandOption solutionPath,
            CommandOption languageVersion,
            CommandOption diff,
            CommandOption gitSource,
            CommandOption testProjects)
        {
            var fileLocation = Path.Combine(basePath, GetOption(configFilePath.Value(), CLIOptions.ConfigFilePath));
            if (File.Exists(fileLocation))
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile(fileLocation)
                    .Build()
                    .GetSection("stryker-config");
            }

            return new StrykerOptions(
                basePath: basePath,
                reporters: GetOption(reporter.Value(), CLIOptions.Reporters),
                dashboadApiKey: GetOption(dashboardApiKey.Value(), CLIOptions.DashboardApiKeyOption),
                projectName: GetOption(reportersProjectName.Value(), CLIOptions.DashboardProjectNameOption),
                moduleName: GetOption(reportersModuleName.Value(), CLIOptions.DashboardModuleNameOption),
                projectVersion: GetOption(reportersProjectVersion.Value(), CLIOptions.DashboardProjectVersionOption),
                projectUnderTestNameFilter: GetOption(projectUnderTestNameFilter.Value(), CLIOptions.ProjectFileName),
                additionalTimeoutMS: GetOption(additionalTimeoutMS.Value(), CLIOptions.AdditionalTimeoutMS),
                excludedMutations: GetOption(excludedMutations.Value(), CLIOptions.ExcludedMutations),
                ignoredMethods: GetOption(ignoreMethods.Value(), CLIOptions.IgnoreMethods),
                logLevel: GetOption(logLevel.Value(), CLIOptions.LogLevel),
                logToFile: GetOption(logToFile.HasValue(), CLIOptions.LogToFile),
                devMode: GetOption(devMode.HasValue(), CLIOptions.DevMode),
                maxConcurrentTestRunners: GetOption(maxConcurrentTestRunners.Value(), CLIOptions.MaxConcurrentTestRunners),
                coverageAnalysis: GetOption(coverageAnalysis.Value(), CLIOptions.CoverageAnalysis),
                abortTestOnFail: GetOption(abortTestOnFail.HasValue(), CLIOptions.AbortTestOnFail),
                thresholdHigh: GetOption(thresholdHigh.Value(), CLIOptions.ThresholdHigh),
                thresholdLow: GetOption(thresholdLow.Value(), CLIOptions.ThresholdLow),
                thresholdBreak: GetOption(thresholdBreak.Value(), CLIOptions.ThresholdBreak),
                filesToExclude: GetOption(filesToExclude.Value(), CLIOptions.FilesToExclude),
                mutate: GetOption(filePatterns.Value(), CLIOptions.Mutate),
                testRunner: GetOption(testRunner.Value(), CLIOptions.TestRunner),
                solutionPath: GetOption(solutionPath.Value(), CLIOptions.SolutionPath),
                languageVersion: GetOption(languageVersion.Value(), CLIOptions.LanguageVersionOption),
                diff: GetOption(diff.HasValue(), CLIOptions.Diff),
                gitSource: GetOption(gitSource.Value(), CLIOptions.GitSource),
                testProjects: GetOption(testProjects.Value(), CLIOptions.TestProjects));
        }

        private T GetOption<V, T>(V cliValue, CLIOption<T> option)
        {
            T value = default;
            var hasValue = false;

            if (cliValue != null &&
                (option.ValueType == CommandOptionType.NoValue && cliValue is bool boolValue && boolValue ||
                 option.ValueType != CommandOptionType.NoValue))
            {
                // Convert the cliValue string to the desired type
                value = ConvertTo(cliValue, option);
                hasValue = true;
            }
            else if (_config != null)
            {
                // Try to get the value from the config file
                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
                {
                    value = _config.GetSection(option.JsonKey).Get<T>();
                    hasValue = true;
                }
                else
                {
                    var configValue = _config.GetValue(option.JsonKey, string.Empty);
                    if (!string.IsNullOrEmpty(configValue))
                    {
                        value = ConvertTo(configValue, option);
                        hasValue = true;
                    }
                }
            }

            // Unable to get value from user, return default value
            if (!hasValue)
            {
                return option.DefaultValue;
            }

            // Notify user that they are using a deprecated argument.
            if (option.IsDeprecated && !Equals(value, option.DefaultValue))
            {
                _logger.LogWarning($"Argument {option.ArgumentName} is deprecated: {option.DeprecatedMessage}");
            }

            return value;
        }

        private T ConvertTo<V, T>(V value, CLIOption<T> option)
        {
            try
            {
                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
                {
                    // Convert json array to IEnumerable of desired type
                    var list = JsonConvert.DeserializeObject<T>(value as string);
                    return list;
                }

                // Convert value to desired type
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                throw new StrykerInputException(
                    "A value passed to an option was not valid.",
                    $@"The option {option.ArgumentName} with value {value} is not valid.
Hint:
{ex.Message}");
            }
        }
    }
}
