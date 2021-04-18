---
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/configuration.md
---

## Basics

You run stryker from the test project directory.

On some dotnet core projects stryker can run without specifying any custom configuration. Simply run `dotnet stryker` to start testing.  
On dotnet framework projects the solution path argument is always required. Run at least `dotnet stryker --solution <solution-path>` or specify the solution file path in the config file to start testing. See [solution](#solution-[path]).

## Use a config file
When using Stryker regularly we recommend using a config file. This way you won't have to document how to run Stryker, you can save the config file in version control. To use a config file create a file called `stryker-config.json` in the (unit test) project folder and add a configuration section called stryker-config. Alternatively use [init](#init-[bool])

Example `stryker-config.json` file:
``` javascript
{
    "stryker-config":
    {
        "solution": "../SolutionFile.sln",
        "project": "ExampleProject.csproj"
    }
}
```

### `init` [`flag`]

Default: `false`  
Command line: `--init`  
Config file: `N/A`  

Creates a stryker config file with default options. Any options passed in the cli will override the defaults.

### `config-file` [`path`]

Default: `stryker-config.json`  
Command line: `[-f|--config-file] "appsettings.dev.json"`  
Config file: `N/A`  

You can specify a custom path to the config file. For example if you want to add the stryker config section to your appsettings file. The section should still be called `stryker-config`.

## Project information

### `solution` [`path`]

Default: `null`  
Command line: `[-s|--solution] "../solution.sln"`  
Config file: `"solution": '../solution.sln'`

The solution file is required for dotnet framework projects. You may specify the solution file for dotnet core projects. In some cases this can help with dependency resolution.

### `project` [`file-name`]

Default: `null`  
Command line: `[-p|--project] "MyAwesomeProject.csproj"`  
Config file: `"project": 'MyAwesomeProject.csproj'`

The project file name is required when your test project has more than one project reference. Stryker can currently mutate one project under test for 1..N test projects but not 1..N projects under test for one test project.

*\* Do not pass a path to this option. Pass the project file **name** as it appears in your test project's references.*

### `test-projects` [`path[]`]

Default: `null`  
Command line: `N/A`  
Config file: `"test-projects": ['../MyProject.UnitTests/MyProject.UnitTests.csproj', '../MyProject.SpecFlow/MyProject.SpecFlow.csproj']`

When you have multiple test projects covering one project under test you may specify all relevant test projects in the config file. You must run stryker from the project under test instead of the test project directory when using multiple test projects.

### `mutate` [`glob[]`]

Default: `*`  
Command line: `[-m|--mutate] "**/*Services.cs"`  
Config file: `"mutate": ['**/*Services.cs', '!**/*.Generated.cs']`

*\* The mutate option can be used multiple times on the command line*

With `mutate` you configure the subset of files to use for mutation testing. Only source files part of your project will be taken into account. When this option is not specified the whole project will be mutated.  
You can add an `!` in front of the pattern to exclude instead of include matching files. This can be used to for example ignore generated files while mutating.

When only exclude patterns are provided, all files will be included that do not match any exclude pattern. If both include and exclude patterns are provided, only the files that match an include pattern but not also an exclude pattern will be included. The order of the patterns is irrelevant.

The patterns support [globbing syntax](https://en.wikipedia.org/wiki/Glob_(programming)) to allow wildcards.

**Example**:

| Patterns  | File                      | Will be mutated   |
| ----------| ------------------------- | ----------------- |
| null            | MyFolder/MyFactory.cs    | Yes               |
| '\*\*/\*.\*'   | MyFolder/MyFactory.cs    | Yes               |
| '!\*\*/MyFactory.cs'   | MyFolder/MyFactory.cs    | No        |

To allow more fine grained filtering you can also specify the span of text that should be in- or excluded. A span is defined by the indices of the first character and the last character.

```bash
dotnet stryker -m "MyFolder/MyService.cs{10..100}"
```

### `language-version` [`string`]

Default: `latest`  
Command line: `N/A`  
Config file: `"language-version": 'CSharp7_3'`

Stryker compiles with the latest stable csharp version by default. This should generally be fine as csharp language features are forward compatible. You should not have to change the option from latest unless you're using preview versions of dotnet/csharp. If you do have compilation errors regarding language features you can explicitly set the language version.

Valid language versions:
- Default (Latest)
- Latest (Default)
- Csharp2
- Csharp3
- Csharp4
- Csharp5
- Csharp6
- Csharp7
- Csharp7_1
- Csharp7_2
- Csharp7_3
- Csharp8
- Csharp9
- Preview (next language version)

*\* Csharp version 1 is not allowed because stryker injects helper code that uses csharp 2 language features.*

## Control flow

### `mutation-level` [`level`]

Default: `Standard`  
Command line: `[-l|--mutation-level] "Advanced"`  
Config file: `"mutation-level": 'Advanced'`

Stryker supports multiple mutation levels. Each level comes with a specific set of mutations. Each level contains the mutations of the levels below it. By setting the level to `Complete` you will get all possible mutations and the thus the strictest mutation test. This comes at the price of longer runtime as more mutations will be generated and tested. 

The levels are:
- Basic
- Standard
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

### `reporter` [`string[]`]

Default: `html, progress`  
Command line: `[-r|--reporter] "html"`  
Config file: `"reporters": ['html', 'json', 'progress']`

*The reporter option can be used multiple times on the command line*

During a mutation testrun one or more reporters can be enabled. A reporter will produce some kind of output during or after the mutation testrun.

The available reporter options are
* all
* progress
* dots
* cleartext
* cleartexttree
* html
* json
* dashboard

You can find a description for every reporter in [reporter docs](./Reporters.md)

### `additional-timeout` [`number`]

Default: `5000`  
Command line: `N/A`  
Config file: `"additional-timeout": 3000`

Some mutations can create endless loops inside your code. To detect and stop these loops Stryker cancels a unit test run after a set time.
The formula to calculate the timeout is:

`timeout = initialTestTime + additionalTimeout`

If you have a lot of timeouts you might need to increase the additional timeout. If you have a lot of endless loops causing a long mutation testrun you might want to decrease the additional timeout. Only decrease the additional timeout if you are certain that the mutations are endless loops.

Timeout is in milliseconds.

### `concurrency` [`number`]

Default: `your number of logical processors / 2`  
Command line: `[-c|--concurrency] 10`  
Config file: `"concurrency": 10`

Change the amount of concurrent workers stryker uses for the mutation testrun. Defaults to using half your logical (virtual) processor count.

**Example**: an intel i7 quad-core with hyperthreading has 8 logical cores and 4 physical cores. Stryker will use 4 concurrent workers when using the default.

### `thresholds` [`object`]

Default: `{ high: 80, low: 60, break: 0 }`  
Command line: `[-b|--break] 40`  
Config file: `"thresholds": { "high": 80, "low": 60, "break": 0 }`

Configure the mutation score thresholds for your project.

- `mutation score >= threshold-high`: 
    - Awesome! Your reporters will color this green and happy.
- `mutation score < threshold-high && mutation score >= threshold-low`:
    - Warning! Your reporters will display yellow/orange colors, watch out!
- `mutation score < threshold-low`:
    - Danger! Your reporters will display red colors, you're in the danger zone now.
- `mutation score < threshold-break`:
    - Error! The application will exit with exit code 1.

Set threshold break to 0 (default) or leave it empty to not exit with an error code. This option can also be set using the command line.

### `ignore-mutations` [`string[]`]

Default: `null`  
Command line: `N/A`  
Config file: `"ignore-mutations": ['string', 'logical']`

Turn off mutations that are not currently relevant to your project. 

The mutants of the ignored types will not be tested. They will show up in your reports as 'Ignored'.

### `ignore-methods` [`string[]`]

Default: `null`  
Command line: `N/A`  
Config file: `"ignore-methods": ['ToString', 'ConfigureAwait', '*Exception.ctor']`

Skip specified method signatures from being mutated. 

```csharp
// This mutation will be skipped
ConfigureAwait(true); 

// This mutation won't because we cannot currently detect this
var t = true;
ConfigureAwait(t); 
```

You can also ignore constructors by specifying the type and adding the `.ctor` suffix.

Both, method names and constructor names support wildcards.

```json
"['*Log']" // Ignores all methods ending with Log
"['*Exception.ctor']" // Ignores all exception constructors
```

## Optimization

### `coverage-analysis` [`string`]

Default: `perTest`  
Command line: `N/A`  
Config file: `"coverage-analysis": 'off'`

Use coverage info to speed up execution. 

- **off**: coverage data is not captured.
- **perTest**: capture the list of mutants covered by each test. For every mutant that has tests, only the tests that cover the mutant are tested. Fastest option.
- **all**: capture the list of mutants covered by each test. Test only these mutants. Non covered mutants are assumed as survivors. Fast option.
- **perTestInIsolation**: like 'perTest', but running each test in an isolated run. This results in more accurate
coverage information for some mutants (see below), at the expense of a longer startup time.

#### Notes on coverage analysis
* Results should not be impacted by coverage analysis. If you identify a suspicious survivor, run
Stryker again without coverage analysis and report an issue if this mutant is killed by this run.
* when using `perTest` mode, mutants that are executed as part as some static constructor/initializer 
are run against all tests as Stryker cannot reliably capture coverage for those. This is a consequence of static
constructors/initialisers being called only once during tests. This heuristic is not needed when using
`perTestInIsolation` due to test being run one by one.


### `disable-bail` <`bool`>

Default: `false`  
Command line: `N/A`  
Config file: `"disable-bail": true`

Stryker aborts a unit testrun for a mutant as soon as one test fails because this is enough to confirm the mutant is killed. This can reduce the total runtime but also means you miss information about individual unit tests (eg if a unit test does not kill any mutants and is therefore useless). You can disable this behavior and run all unit tests for a mutant to completion. This can be especially useful when you want to find useless unit tests.

### `disable-mix-mutants` <`bool`>

Default: `false`  
Command line: `N/A`  
Config file: `"disable-mix-mutants": true`

Stryker combines multiple mutants in the same testrun when the mutants are not covered by the same unit tests. This reduces the total runtime. You can disable this behavior and run every mutation in an isolated testrun. This can be useful when mixed mutants have unintended side effects.

### `since` <`bool[<:comittish>]`>

Default: `false:master`  
Command line: `--since:feat-2`  
Config file: `"since": true, "since-target": 'feat-2'`

Use git information to test only code changes since the given target.

Set the diffing target on the command line by passing a comittish with the since flag.
Set the diffing target in the config file by setting the [since-target](#since-target-<comittish>) option.

*\* For changes on test project files all mutants covered by tests in that file will be seen as changed.*

### `since-target` <`comittish`>

Default: `master`  
Command line: `N/A`  
Config file: `"since-target": 'feat-2'`

Set the diffing target for the [since](#since-<bool[<:comittish>]>) feature.

### Diff ignore files
Allows to specify an array of C# files which should be ignored if present in the diff.
Any not ignored files will trigger all mutants to be tested because we cannot determine what mutants are affected by these files. 
This feature is only recommended when you are sure these files will not affect results, or when you are prepared to sacrifice accuracy for performance.
            
Use [globbing syntax](https://en.wikipedia.org/wiki/Glob_(programming)) for wildcards. Example: ['**/*Assets.json','**/favicon.ico']

```
dotnet stryker --diff-ignore-files ['**/*.ts']
dotnet stryker -diff-ignore-files ['**/*.ts']
```

Default: `[]`

## EXPERIMENTAL: Dashboard Compare
Enabling the dashboard compare feature saves reports and re-uses the result when a mutant or it's tests are unchanged.

```
dotnet stryker --dashboard-compare
dotnet stryker -compare
```

Default `"off"`

This feature automatically enables the --diff feature.

This feature is experimental. Results can contain slight false postives and false negatives.

### Fallback version
When enabling the --dashboard-compare feature you can provide a fallback version. This version will be used to pull a baseline when we cannot find a baseline for your current branch. When we are still unable to provide a baseline we will start a complete testrun to create a complete baseline.

```
dotnet stryker --dashboard-compare --dashboard-fallback-version master
dotnet stryker -compare -fallback-version master
```
Default: value provided to --git-source or null

### Baseline Storage location
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

### Configurating Dashboard location

See: [Dashboard Reporter Settings](./Reporters.md#dashboard-reporter)

#### Configuring Azure File Storage
When using Azure File Storage as baseline storage location you are required to provide the following values.

#### Azure File Storage URL
This is the url to your Azure File Storage. The URL should look something like this:

```
https://STORAGE_NAME.file.core.windows.net/FILE_SHARE/(optional)SUBFOLDER
```
Providing a subfolder is optional, we store the baseline in a `StrykerOutput` subfolder.

```
-storage url https://STORAGE_NAME.file.core.windows.net/FILE_SHARE/(optional)SUBFOLDER
--azure-storage-url https://STORAGE_NAME.file.core.windows.net/FILE_SHARE/(optional)SUBFOLDER
```

#### Shared Access Signature (SAS)
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

### Using dashboard compare in a pull request pipeline

See: [Using stryker in pipelines](./Stryker-in-pipeline.md)

## Troubleshooting

### `verbosity` [`log-level`]

Default: `info`  
Command line: `[-V|--verbosity] trace`  
Config file: `"verbosity": 'trace'`

Change the console `verbosity` of stryker when you want more or less details about the mutation testrun.

All available loglevels are
* error
* warning
* info
* debug
* trace

### `log-to-file` [`flag`]

Default: `false`  
Command line: `[-L|--log-to-file]`  
Config file: `N/A`

When creating an issue on github you can include a logfile so the issue can be diagnosed easier. 

*\* File logging always uses loglevel `trace`.*

### `dev-mode` [`flag`]

Default: `false`  
Command line: `[--dev-mode]`  
Config file: `N/A`

Stryker will not gracefully recover from compilation errors, instead crash immediately. Used during development to quickly diagnose errors.  
Also enables more debug logs not generally useful to normal users.
