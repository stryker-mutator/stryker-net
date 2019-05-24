For .NET Core projects Stryker.NET can be run without any configuration. On .NET Framework projects the solution path is required.

The full list of Stryker.NET configuration options are:

<!-- TOC -->
- [Solution path (required .NET Framework)](#solution-path)
- [Project file (required on some projects)](#project-file)
- [Test runner](#test-runner)
- [Timeout time](#timeout-time)
- [Reporters](#reporters)
- [Logging to console](#logging-to-console)
- [Excluding mutations](#excluding-mutations)
- [Excluding files](#excluding-files)
- [Custom tresholds](#unary-operators)
<!-- /TOC -->

## Solution path
On .NET Framework projects Stryker needs your `.sln` file path.

```
dotnet stryker --solution-path "..\\ExampleProject.sln"
dotnet stryker -s "..\\ExampleProject.sln"
```

Stryker.NET needs the path to execute:

```
nuget restore "*.sln"
```
and 
```
MSBuild.exe "*.sln"
```

## Project file
When Stryker finds two or more project references inside your test project, it needs to know what project should be mutated. Pass the name of this project using:

```
dotnet stryker --project-file SomeProjectName.csproj
dotnet stryker -p SomeProjectName.csproj
```

## Specify testrunner
Stryker supports `dotnet test` and `vstest` as testrunners. Dotnet test is currently the default because it has been more thoroughly tested, but vstest offers us more tight integration with the testrunner itself.
To use the experimental vstest testrunner pass the following option to stryker:

```
dotnet stryker --test-runner vstest
dotnet stryker -tr vstest
```

Default: `"dotnet test"`

## Timeout time
Some mutations can create endless loops inside your code. To detect and stop these loops, Stryker generates timeouts after some time. Using this parameter you can increase or decrease the time before a timeout will be thrown.

```
dotnet stryker --timeout-ms 5000
dotnet stryker -t 5000
```

Default: `"30000"`

## Reporters
During a mutation testrun one or more reporters can be enabled. A reporter will produce some kind of output during or after the mutation testrun.

```
dotnet stryker --reporters "['html', 'progress']"
dotnet stryker -r "['html', 'progress']"
```

You can find a list of all available reporters and what output they product in the [reporter docs](/docs/Reporters.md)

Default: `"['cleartext', 'progress']"`

## Logging to console
To gain more insight in what Stryker does during a mutation testrun you can lower your loglevel.
```
dotnet stryker --log-level "debug"
dotnet stryker -l "debug"
```

All available loglevels are:
* error
* warning
* info
* debug
* trace

Default: `"info"`

## Logging to a file
When creating an issue for Stryker.NET on github you can include a logfile. File logging always uses loglevel `trace`.

```
dotnet stryker --log-file
dotnet stryker -f

```

Default: `false`

## Maximum concurrent test runners  
By default Stryker.NET will use as much CPU power as you allow it to use during a mutation testrun. You can lower this setting to lower your CPU presure.

```
dotnet stryker --max-concurrent-test-runners 4
dotnet stryker -c 4
```

This setting can also be used to disable parallel testing. This can be useful if your test project cannot handle paralel testruns.
```
dotnet stryker -c 1
```

Default: `your number of logical processors / 2`*

\* This usually equals your physical processor count

## Custom thresholds
If you want to decide on your own mutation score thresholds, you can configure this with extra parameters.

```
dotnet stryker --threshold-high 90 --threshold-low 75 --threshold-break 50
dotnet stryker -th 90 -tl 75 -tb 50
```

- `mutation score > threshold-high`: 
    - Awesome! Your reporters will color this green and happy.
- `threshold-high > mutation score > threshold-low`:
    - Warning! Your reporters will display yellow/orange colors, watch out!
- `threshold-low < mutation score > threshold-break`:
    - Danger! Your reporters will display red colors, you're in the danger zone now.
- `threshold-break > mutation score`:
    - Error! The application will exit with exit code 1.

Default: `80`, `60`, `0`

## Excluding mutations
If you deem some mutations unwanted for your project you can disable mutations. 

```
dotnet stryker --excluded-mutations "['string', 'logical']"
dotnet stryker -em "['string', 'logical']"
```

The mutations of these kinds will be skipped and not be shown in your reports. This can also speed up your performance on large projects. But don't get too exited, skipping mutations doesn't improve your mutation score ;)

## Excluding files
If you decide to exclude files for unit testing, you can configure this with the following command:

```
dotnet stryker --files-to-exclude "['./ExampleClass.cs', './ExampleDirectory', './ExampleDirectory/ExampleClass2.cs']"
dotnet stryker -fte "['./ExampleClass.cs', './ExampleDirectory', './ExampleDirectory/ExampleClass2.cs']"
```

We recommend to use relative paths. Relative paths are automatically resolved. Absolute paths break easily on different devices. However it is also possible to use absolute paths.

When you want to exclude a large set of files, it is advised to use the stryker configuration file because it is easier to handle multiple files.

Default: `[]`

## Use a config file
There is also the option to use a config file. To use a config file all you have to do is add a file called `stryker-config.json` in the root of your test project and add a configuration section called stryker-config. Then you can add the options you want to configure to the file.

Example config file:
``` javascript
{
    "stryker-config":
    {
        "test-runner": "vstest",
        "reporters": [
            "progress",
            "html"
        ],
        "log-level": "info",
        "log-file":true,
        "timeout-ms": 10000,
        "project-file": "ExampleProject.csproj",
        "max-concurrent-test-runners": 4,
        "threshold-high": 80,
        "threshold-low": 70,
        "threshold-break": 60,
        "files-to-exclude": [
            "./ExampleClass.cs",
            "./ExampleDirectory/",
            "./ExampleDirectory/ExampleClass2.cs",
            "C:\\ExampleRepo\\ExampleDirectory\\ExampleClass.cs"
        ],
        "excluded-mutations": [
            "string",
            "Logical operators"
        ]
    }
}
```

## Config file location
If you want to integrate these settings in your existing settings json, make sure the section is called stryker-config and run stryker with the command
```
dotnet stryker --config-file-path <relativePathToFile>
```

Default: `"./stryker-config.json"`
