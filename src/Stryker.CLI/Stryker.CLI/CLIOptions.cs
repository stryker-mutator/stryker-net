namespace Stryker.CLI
{
    public static class CLIOptions
    {
        public static readonly CLIOption<string> ConfigFilePath = new CLIOption<string>
        {
            ArgumentName = "--configFilePath",
            ArgumentShortName = "-cp <path>",
            ArgumentDescription = "Sets the configFilePath relative to current workingDirectory | stryker-config.json (default)",
            DefaultValue = "stryker-config.json"
        };

        public static readonly CLIOption<string> Reporter = new CLIOption<string>
        {
            ArgumentName = "--reporter",
            ArgumentShortName = "-r <reporter>",
            ArgumentDescription = "Sets the reporter | Options [Console (default), RapportOnly]",
            DefaultValue = "Console",
            JsonKey = "reporter"
        };

        public static readonly CLIOption<string> LogLevel = new CLIOption<string>
        {
            ArgumentName = "--logConsole",
            ArgumentShortName = "-l <logLevel>",
            ArgumentDescription = "Sets the logging level | Options [info (default), warning, debug, trace]",
            DefaultValue = "info",
            JsonKey = "logLevel"
        };

        public static readonly CLIOption<bool> UseLogFile = new CLIOption<bool>
        {
            ArgumentName = "--logFile",
            ArgumentShortName = "-f <useLogFile>",
            ArgumentDescription = "Use logFile | Options [false (Default), true]",
            DefaultValue = false,
            JsonKey = "logFile"
        };

        public static readonly CLIOption<int> AdditionalTimeoutMS = new CLIOption<int>
        {
            ArgumentName = "--timeoutMS",
            ArgumentShortName = "-t <ms>",
            ArgumentDescription = "When passed, a logfile will be created for this mutationtest run on trace level",
            DefaultValue = 30000,
            JsonKey = "timeoutMS"
        };

        public static readonly CLIOption<string> ProjectName = new CLIOption<string>
        {
            ArgumentName = "--project",
            ArgumentShortName = "-p <projectName>",
            ArgumentDescription = @"Used for matching the project references when finding the project to mutate. Example: ""ExampleProject.csproj""",
            JsonKey = "projectName"
        };        
    }
}
