﻿namespace Stryker.CLI
{
    public static class CLIOptions
    {
        public static readonly CLIOption<string> ConfigFilePath = new CLIOption<string>
        {
            ArgumentName = "--config-file-path",
            ArgumentShortName = "-cp <path>",
            ArgumentDescription = "Sets the config-file-path relative to current workingDirectory | stryker-config.json (default)",
            DefaultValue = "stryker-config.json"
        };

        public static readonly CLIOption<string> Reporter = new CLIOption<string>
        {
            ArgumentName = "--reporter",
            ArgumentShortName = "-r <reporter>",
            ArgumentDescription = "Sets the reporter | Options [Console (default), ReportOnly]",
            DefaultValue = "Console",
            JsonKey = "reporter"
        };

        public static readonly CLIOption<string> LogLevel = new CLIOption<string>
        {
            ArgumentName = "--log-console",
            ArgumentShortName = "-l <logLevel>",
            ArgumentDescription = "Sets the logging level | Options [error, warning (default), info, debug, trace]",
            DefaultValue = "warning",
            JsonKey = "log-level"
        };

        public static readonly CLIOption<bool> UseLogFile = new CLIOption<bool>
        {
            ArgumentName = "--log-file",
            ArgumentShortName = "-f <useLogFile>",
            ArgumentDescription = "Use log-file | Options [false (Default), true]",
            DefaultValue = false,
            JsonKey = "log-file"
        };

        public static readonly CLIOption<int> AdditionalTimeoutMS = new CLIOption<int>
        {
            ArgumentName = "--timeout-ms",
            ArgumentShortName = "-t <ms>",
            ArgumentDescription = "When passed, a logfile will be created for this mutationtest run on trace level",
            DefaultValue = 30000,
            JsonKey = "timeout-ms"
        };

        public static readonly CLIOption<string> ProjectFileName = new CLIOption<string>
        {
            ArgumentName = "--project-file",
            ArgumentShortName = "-p <projectFileName>",
            ArgumentDescription = @"Used for matching the project references when finding the project to mutate. Example: ""ExampleProject.csproj""",
            JsonKey = "project-file"
        };

        public static readonly CLIOption<int> MaxConcurrentTestRunners = new CLIOption<int>
        {
            ArgumentName = "--max-concurrent-test-runners",
            ArgumentShortName = "-m <maxConcurrentTestRunners>",
            ArgumentDescription = @"Mutation testing is time consuming. By default Stryker tries to make the most of your CPU, by spawning as many test runners as you have CPU cores.
                                                                 This setting allows you to override this default behavior.

                                                                 Reasons you might want to lower this setting:
                                                                 
                                                                 -Your test runner starts a browser (another CPU-intensive process)
                                                                 -You're running on a shared server and/or
                                                                 -Your hard disk cannot handle the I/O of all test runners",
            DefaultValue = int.MaxValue,
            JsonKey = "max-concurrent-test-runners"
        };
    }
}
