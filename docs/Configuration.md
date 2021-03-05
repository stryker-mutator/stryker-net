---
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/Configuration.md
---

For .NET Core projects Stryker.NET can be run without any configuration. On .NET Framework projects the solution path is required.

## Use a config file
When using Stryker in a team we recomend using a config file. This way you ensure all team members use the same settings to run Stryker. The settings will also be picked up in pipelines. To use a config file create a file called `stryker-config.json` in the folder you run Stryker and add a configuration section called stryker-config. Then you can add the options you want to configure to the file.

Example `stryker-config.json` file:
``` javascript
{
    "stryker-config":
    {
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
        "mutation-level": "Standard",
        "mutate": [
            "!ExampleClass.cs",
            "!Migrations/*.*",
            "!ExampleDirectory\\ExampleClass2.cs"
        ],
        "excluded-mutations": [
            "string",
            "Logical operators"
        ],
        "ignore-methods": [
            "*Log*",
            "ToString",
            "*HashCode*"
        ],
        "dashboard-compare": true,
        "baseline-storage-location": "AzureFileStorage",
        "azure-storage-url": "https://storageaccount.file.core.windows.net/sharename",
        "azure-storage-sas": "<SAS>"
    }
}
```

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
When Stryker finds two or more project references inside your test project, it needs to know which project should be mutated. Pass the name of this project using:

```
dotnet stryker --project-file SomeProjectName.csproj
dotnet stryker -p SomeProjectName.csproj
```

## Mutation level
Stryker supports multiple mutation levels. Each level comes with a specific set of mutations. Each level contains the mutations of the levels below it. By setting the level to `Complete` you will get all possible mutations and the best mutation testing experience. This comes at the price of longer runtime, as more mutations have to be tested at higher levels. 

The levels are as follows:
- Basic
- Standard (Default)
- Advanced
- Complete

| Mutations| Level| 
| ------------- | ------------- | 
| Arithmetic Operators | Basic|
| Block (not yet implemented) | Basic|
| Equality Operators | Standard |
| Boolean Literals | Standard|
| Assignment statements | Standard |
| Collection initializer | Standard |
| Unary Operators | Standard |
| Update Operators | Standard |
| String Literals and Constants | Standard |
| Bitwise Operators | Standard |
| Linq Methods | Standard |
| Checked Statements | Standard |
| Regex | Advanced |
| Advanced Linq Methods (not yet implemented) | Complete |
| Advanced Regex (not yet implemented) | Complete |

```
dotnet stryker --mutation-level Advanced
dotnet stryker -level Advanced
```

Default: `"Standard"`

## Specify testrunner
> ⚠ This parameter is deprecated. This option will be removed in version 1.0.  Please submit an issue if you have a use case that requires Dotnet test. 

Stryker supports `dotnet test`, the commandline testrunner and `VsTest`, the visual studio testrunner. 
VsTest is the default because it offers tight integration with all test frameworks (MsTest, xUnit, NUnit etc).
Dotnet test can be used if VsTest causes issues for some reason. Please also submit an issue with us if you experience difficulties with VsTest.


```
dotnet stryker --test-runner dotnettest
dotnet stryker -tr dotnettest
```

Default: `"vstest"`

## Timeout time
Some mutations can create endless loops inside your code. To detect and stop these loops, Stryker cancels a unit test run after a set time.
Using this parameter you can increase or decrease the time before a test will time out.

```
dotnet stryker --timeout-ms 1000
dotnet stryker -t 1000
```

Default: `"5000"`

## Reporters
During a mutation testrun one or more reporters can be enabled. A reporter will produce some kind of output during or after the mutation testrun.

```
dotnet stryker --reporters "['html', 'progress']"
dotnet stryker -r "['html', 'progress']"
```

You can find a list of all available reporters and what output they produce in the [reporter docs](./Reporters.md)

Default: `"['html', 'progress']"`

## Test projects
Stryker can also be run from the directory containing the project under test. If you pass a list of test projects that reference the project under test, the tests of all projects will be run while testing the mutants.

```
dotnet stryker --test-projects "['../MyProject.UnitTests/MyProject.UnitTests.csproj', '../MyProject.SpecFlow/MyProject.SpecFlow.csproj']"
dotnet stryker -tp "['../MyProject.UnitTests/MyProject.UnitTests.csproj', '../MyProject.SpecFlow/MyProject.SpecFlow.csproj']"
```

When running from a test project directory this option is not required.

Default: `null`

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

Default: `off`

## Maximum concurrent test runners  
By default Stryker.NET will use as much CPU power as you allow it to use during a mutation testrun. You can lower this setting to lower your CPU usage.

```
dotnet stryker --max-concurrent-test-runners 4
dotnet stryker -c 4
```

This setting can also be used to disable parallel testing. This can be useful if your test project cannot handle parallel testruns.
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
- `threshold-low > mutation score > threshold-break`:
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

The mutations of these kinds will be skipped and not be shown in your reports. This can also speed up your performance on large projects. But don't get too excited, skipping mutations doesn't improve your mutation score ;)

## Excluding files
> ⚠ This parameter is deprecated. Use [Mutate](#mutate) instead.

If you decide to exclude files for unit testing, you can configure this with the following command:

```
dotnet stryker --files-to-exclude "['./ExampleClass.cs', './ExampleDirectory', './ExampleDirectory/ExampleClass2.cs']"
dotnet stryker -fte "['./ExampleClass.cs', './ExampleDirectory', './ExampleDirectory/ExampleClass2.cs']"
```

We recommend to use relative paths. Relative paths are automatically resolved. Absolute paths break easily on different devices. However it is also possible to use absolute paths.

When you want to exclude a large set of files, it is advised to use the stryker configuration file because it is easier to handle multiple files.

Default: `[]`

## Mutate
To specify which files should be mutated you can use file pattern to in- or excluded files or even only parts of a files. By default all files are included.

```
dotnet stryker --mutate "['C:/Repos/MyProject/MyFile.cs']"
dotnet stryker -m "['C:/Repos/MyProject/MyFile.cs']"
```

The patterns support [globbing syntax](https://en.wikipedia.org/wiki/Glob_(programming)) to allow wildcards.

```
dotnet stryker --mutate "['**/*Services.cs']"
dotnet stryker -m "['**/*Services.cs']"
```

You can add an `!` in front of the pattern to exclude all files that match the pattern.

```
dotnet stryker --mutate "['!**/*Factory.cs']"
dotnet stryker -m "['!**/*Factory.cs']"
```
When only exclude patterns are provided, all files will be included that do not match any exclude pattern. If both include and exclude patterns are provided, only the files that match an include pattern but not also an exclude pattern will be included. The order of the patterns is irrelevant.

### Example:

| Patterns  | File                      | Will be mutated   |
| ----------| ------------------------- | ----------------- |
| []            | MyFolder/MyFactory.cs    | Yes               |
| []            | MyFolder/MyService.cs | Yes               |
| ['\*\*/\*.\*']   | MyFolder/MyFactory.cs    | Yes               |
| ['\*\*/\*.\*']   | MyFolder/MyService.cs | Yes               |
| ['!\*\*/MyFactory.cs']   | MyFolder/MyFactory.cs    | No        |
| ['!\*\*/MyFactory.cs']   | MyFolder/MyService.cs | Yes       |
| ['!\*\*/MyFactory.cs', '\*\*/My\*.cs']   | MyFolder/MyFactory.cs    | No        |
| ['!\*\*/MyFactory.cs', '\*\*/My\*.cs']   | MyFolder/MyService.cs | Yes       |

To allow more fine grained filtering you can also specify the span of text that should be in- or excluded. A span is defined by the indices of the first character and the last character.

```
dotnet stryker --mutate "['MyFolder/MyService.cs{10..100}']"
dotnet stryker -m "['MyFolder/MyService.cs{10..100}']"
```

## Ignore methods
If you would like to ignore some mutations that are passed as method parameters, you can do so by specifying which methods to ignore:

```
dotnet stryker --ignore-methods "['ToString', 'ConfigureAwait', '*Exception.ctor']"
dotnet stryker -im "['ToString', 'ConfigureAwait', '*Exception.ctor']"
```

Ignore methods will only affect mutations in directly passed parameters.

``` csharp
// This mutation will be skipped;
ConfigureAwait(true); 

// This mutation won't
var t = true;
ConfigureAwait(t); 
```

You can also ignore constructors by specifying the type and adding the `.ctor` suffix.

`dotnet stryker -im "['NotImplementedException.ctor']"`

Both, method names and constructor names, support wildcards.

```
dotnet stryker -im "['*Log']" // Ignores all methods ending with Log
dotnet stryker -im "['*Exception.ctor']" // Ignores all exception constructors
```

Default: `[]`

## Config file location
If you want to integrate these settings in your existing settings json, make sure the section is called stryker-config and run stryker with the command
```
dotnet stryker --config-file-path <relativePathToFile>
dotnet stryker -cp <relativePathToFile>
```

Default: `"./stryker-config.json"`

## Coverage analysis
Use coverage info to speed up execution. Possible values are: off, perTest, all, perIsolatedTest.

- **off**: coverage data is not captured.
- **perTest**: capture the list of mutants covered by each test. For every mutant that has tests, only the tests that cover the mutant are tested. Fastest option.
- **all**: capture the list of mutants covered by each test. Test only these mutants. Non covered mutants are assumed as survivors. Fast option.
- **perTestInIsolation**: like 'perTest', but running each test in an isolated run. This results in more accurate
coverage information for some mutants (see below), at the expense of a longer startup time.

```
dotnet stryker --coverage-analysis perTest
dotnet stryker -ca perTest
```

Default: `"perTest"`
### Notes on coverage analysis
* The 'dotnet test' runner only supports `all` mode. This is due to dotnet test limitation
* Results are not impacted by coverage analysis. If you identify a suspicious survivor, run
Stryker again without coverage analysis and report an issue if this mutant is killed by this run.
* when using `perTest` mode, mutants that are executed as part as some static constructor/initializer 
are run against all tests, as Stryker cannot reliably capture coverage for those. This is a consequence of static
constructors/initialisers being called only once during tests. This heuristic is not needed when using
`perTestInIsolation` due to test being run one by one.


## Abort test on fail
Abort unit testrun as soon as any one unit test fails. This can reduce the overall running time.

```
dotnet stryker --abort-test-on-fail
dotnet stryker -atof
```

Default: `"on"`

## Diff
Enables the diff feature. It makes sure to only mutate changed files. Gets the diff from git by default.

```
dotnet stryker --diff
dotnet stryker -diff
```

Default: `false`

## Diff ignore files
Allows to specify an array of C# files which should be ignored if present in the diff.
Any not ignored files will trigger all mutants to be tested because we cannot determine what mutants are affected by these files. 
This feature is only recommended when you are sure these files will not affect results, or when you are prepared to sacrifice accuracy for performance.
            
Use [globbing syntax](https://en.wikipedia.org/wiki/Glob_(programming)) for wildcards. Example: ['**/*Assets.json','**/favicon.ico']

```
dotnet stryker --diff-ignore-files ['**/*.ts']
dotnet stryker -diff-ignore-files ['**/*.ts']
```

Default: `[]`

## Git diff target
Sets the source commit-ish (branch or commit) to compare with the current codebase, used for calculating the difference when --diff is enabled.

```
dotnet stryker --git-diff-target "development"
dotnet stryker -gdt "development"
```

Default: `master`

This feature works based on file diffs, which means that only changed files will be mutated.

Also note that for changes on test files all mutants covered by tests in that file will be mutated.

## EXPERIMENTAL: Dashboard Compare
Enabling the dashboard compare feature saves reports and re-uses the result when a mutant or it's tests are unchanged.

```
dotnet stryker --dashboard-compare
dotnet stryker -compare
```

Default `"off"`

This feature automatically enables the --diff feature.

This feature is experimental. Results can contain slight false postives and false negatives.

## Fallback version
When enabling the --dashboard-compare feature you can provide a fallback version. This version will be used to pull a baseline when we cannot find a baseline for your current branch. When we are still unable to provide a baseline we will start a complete testrun to create a complete baseline.

```
dotnet stryker --dashboard-compare --dashboard-fallback-version master
dotnet stryker -compare -fallback-version master
```
Default: value provided to --git-source or null

## Baseline Storage location
Sets the storage location for the baseline used by --dashboard-compare. By default this is set to disk, when the dashboard reporter is enabled this is automatically set to Stryker Dashboard.

Supported storage locations are:

| Storage location | Option | Description |
|------------------|--------|-------------|
| Disk             | Disk   | Saves the baseline to the `StrykerOutput` folder|
| Stryker Dashboard| Dashboard | Saves the baseline to Stryker Dashboard |
| Azure File Storage | AzureFileStorage | Saves the baseline to Azure File Storage |

```
dotnet stryker --dashboard-compare --baseline-storage-location disk
dotnet stryker -compare -bsl disk
```
Defaut `"disk"`

## Configurating Dashboard location

See: [Dashboard Reporter Settings](./Reporters.md#dashboard-reporter)

## Configuring Azure File Storage
When using Azure File Storage as baseline storage location you are required to provide the following values.

### Azure File Storage URL
This is the url to your Azure File Storage. The URL should look something like this:

```
https://STORAGE_NAME.file.core.windows.net/FILE_SHARE/(optional)SUBFOLDER
```
Providing a subfolder is optional, we store the baseline in a `StrykerOutput` subfolder.

```
-storage url https://STORAGE_NAME.file.core.windows.net/FILE_SHARE/(optional)SUBFOLDER
--azure-storage-url https://STORAGE_NAME.file.core.windows.net/FILE_SHARE/(optional)SUBFOLDER
```

### Shared Access Signature (SAS)
For authentication we support Shared Access Signatures. For more information on how to configure a SAS check the [Azure documentation](https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview).

```
-storage-sas <STORAGE_SAS>
--azure-storage-sas <STORAGE_SAS>
```

The complete configuration would look like this:
```
dotnet stryker --dashboard-compare --baseline-storage-location AzureFileStorage --azure-storage-url https://STORAGE_NAME.file.core.windows.net/FILE_SHARE/(optional)SUBFOLDER --azure-storage-sas STORAGE_SAS

or

dotnet stryker -compare -bsl AzureFileStorage -storage-url https://STORAGE_NAME.file.core.windows.net/FILE_SHARE/(optional)SUBFOLDER -sas STORAGE_SAS
```

## Using dashboard compare in a pull request pipeline

See: [Using stryker in pipelines](./Stryker-in-pipeline.md)
