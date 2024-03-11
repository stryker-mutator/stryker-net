---
title: Configuration
sidebar_position: 30
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/configuration.md
---

## Basics

You run stryker from the test project directory.

On some dotnet core projects stryker can run without specifying any custom configuration. Simply run `dotnet stryker` to start testing.  
On dotnet framework projects the solution path argument is always required. Run at least `dotnet stryker --solution <solution-path>` or specify the solution file path in the config file to start testing. See [solution](#solution-path).

## Use a config file
When using Stryker regularly we recommend using a config file. This way you won't have to document how to run Stryker, you can save the config file in version control. 
To use a config file create a file called `stryker-config.json` in the (unit test) project folder and add a configuration section called stryker-config.

We support json and yaml as the config file formats

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

Example `stryker-config.yaml` file:
``` yaml
  stryker-config:
    solution: '../SolutionFile.sln'
    project: 'ExampleProject.csproj'
```

### Init

To get started quickly with configuring stryker, use the command:

```
dotnet stryker init
```

This will output a new json configuration file where the command is being run.

To create a config file with a custom name, provide a config file name:

```
dotnet stryker init --config-file "custom.json"
```

To override the default values, provide a value using the regular cli options:

```
dotnet stryker init --mutation-level "advanced"
```

### `config-file` &lt;`path`&gt;

Default: `stryker-config.json`  
Command line: `[-f|--config-file] "appsettings.dev.json"`  
Config file: `N/A`  

You can specify a custom path to the config file. For example if you want to add the stryker config section to your appsettings file. The section should still be called `stryker-config`.

## Project information

### `solution` &lt;`path`&gt;

Default: `null`  
Command line: `[-s|--solution] "../solution.sln"`  
Config file: `"solution": '../solution.sln'`

The solution path can be supplied to help with dependency resolution. If stryker is ran from the solution file location the solution file will be analyzed and all projects in the solution will be tested by stryker.

Note: The solution file is required for dotnet framework projects. For dotnet (core) projects the solution file is optional.

### `project` &lt;`file-name`&gt;

Default: `null`  
Command line: `[-p|--project] "MyAwesomeProject.csproj"`  
Config file: `"project": 'MyAwesomeProject.csproj'`

The project file name is required when your test project has more than one project reference. Stryker can currently mutate one project under test for 1..N test projects but not 1..N projects under test for one test project.

*\* Do not pass a path to this option. Pass the project file **name** as it appears in your test project's references.*

### `test-projects` &lt;`string[]`&gt;

Default: `null`  
Command line: `[-tp|--test-project] "../Tests/Tests.csproj" -tp "../MoreTests/MoreTests.csproj"`  
Config file: `"test-projects": ['../MyProject.UnitTests/MyProject.UnitTests.csproj', '../MyProject.SpecFlow/MyProject.SpecFlow.csproj']`

When you have multiple test projects covering one project under test you may specify all relevant test projects in the config file. You must run stryker from the project under test instead of the test project directory when using multiple test projects.

### `test-case-filter` &lt;`string`&gt;

Default: `""`  
Command line: `N/A`  
Config file: `"test-case-filter": "(FullyQualifiedName~UnitTest1&TestCategory=CategoryA)|Priority=1"`

Filter expression to run selective tests. Uses `dotnet test --filter` option syntax, [detailed here](https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests). Use this option if you wish to run stryker only on a selective subset of tests from your test suite.

### `mutate` &lt;`glob[]`&gt;

Default: `*`  
Command line: `[-m|--mutate] "**/*Services.cs" -m "!**/*.Generated.cs"`  
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

To allow more fine-grained filtering you can also specify the span of text that should be in- or excluded. A span is defined by the indices of the first character and the last character.

```bash
dotnet stryker -m "MyFolder/MyService.cs{10..100}"
```

### `language-version` &lt;`string`&gt;

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
- Csharp10
- Csharp11
- Preview (next language version)

*\* Csharp version 1 is not allowed because stryker injects helper code that uses csharp 2 language features.*

### `target-framework` &lt;`string`&gt;

Default: randomly selected
Command line: `target-framework`  
Config file: `"target-framework": "netcoreapp3.1"`

If the project targets multiple frameworks, this way you can specify the particular framework to build against. If you specify a non-existent target, Stryker will build the project against a random one (or the only one if so).

### `project-info.name` &lt;`string`&gt;

Default: `null`  
Command line: `N/A`  
Config file: `"project-info": { "name": 'github.com/stryker-mutator/stryker-net' }`

The name registered with the [Stryker dashboard](./reporters.md#dashboard-reporter). It is in the form of `gitProvider/organization/repository`. At the moment the dashboard backend only supports github.com as a git provider. It can have an indefinite number of levels. Slashes (/) in this name are not escaped. For example `github.com/stryker-mutator/stryker-net`.

### `project-info.module` &lt;`string`&gt;

Default: `null`  
Command line: `N/A`  
Config file: `"project-info": { "module": 'stryker-core' }`

If you want to store multiple reports for a given version you can use this option to separate them logically. For example in a mono-repo setup where each package (or project or module) delivers a separate report. The Stryker dashboard will combine all module reports for a given version into one complete project report.

See [Stryker dashboard](./reporters.md#dashboard-reporter)

### `project-info.version` &lt;`committish`&gt;

Default: `null`  
Command line: `[-v|--version] "feat/logging"`  
Config file: `"project-info": { "version": 'feat/logging' }`

The version of the report. This should be filled with the branch name, git tag or git sha (although no validation is done). You can override a report of a specific version, like docker tags. Slashes in the version should not be encoded. For example, it's valid to use "feat/logging".

See [Stryker dashboard](./reporters.md#dashboard-reporter)

## Control flow

### `mutation-level` &lt;`level`&gt;

Default: `Standard`  
Command line: `[-l|--mutation-level] "Advanced"`  
Config file: `"mutation-level": 'Advanced'`

Stryker supports multiple mutation levels. Each level comes with a specific set of mutations. Each level contains the mutations of the levels below it. By setting the level to `Complete` you will get all possible mutations and thus the strictest mutation test. This comes at the price of longer runtime as more mutations will be generated and tested. 

The levels are:
- Basic
- Standard
- Advanced
- Complete

| Mutations| Level| 
| ------------- | ------------- | 
| Arithmetic Operators | Basic|
| Block statements | Basic|
| Logical Operators | Basic |
| Bitwise Operators | Basic |
| Equality Operators | Standard |
| Boolean Literals | Standard|
| Assignment statements | Standard |
| Collection initializer | Standard |
| Unary Operators | Standard |
| Update Operators | Standard |
| String Literals | Standard |
| Linq Methods | Standard |
| Checked Statements | Standard |
| Regex | Advanced |
| Math Methods | Advanced |

### `reporter` &lt;`string[]`&gt;

Default: `html, progress`  
Command line: `[-r|--reporter] "html" -r "json" -r "progress"`  
Config file: `"reporters": ['html', 'json', 'progress']`

*The reporter option can be used multiple times on the command line*

During a mutation testrun one or more reporters can be enabled. A reporter will produce some kind of output during or after the mutation testrun.

The available reporter options are
* all (Enable all reporters)
* [html](./reporters.md#html-reporter)
* [progress](./reporters.md#progress-reporter)
* [dashboard](./reporters.md#dashboard-reporter)
* [cleartext](./reporters.md#cleartext-reporter)
* [cleartexttree](./reporters.md#cleartext-tree-reporter)
* [dots](./reporters.md#dots-reporter)
* [json](./reporters.md#json-reporter)

You can find a description for every reporter in the [reporter docs](./reporters.md)

### `open-report` &lt;`string`&gt;

Default: `html`
Command line: `[-o:html|--open-report:dashboard]`
Config file: `N/A`

When this option is passed, generated reports should open in the browser automatically once Stryker starts testing mutants, and will update the report till Stryker is done. Both html and dashboard reports can be opened automatically.

Valid values:
- html
- dashboard

### `report-file-name` &lt;`string`&gt;

Default: `mutation-report`
Command line: `N/A` 
Config file: `report-file-name`

If HTML and/or JSON reporting is being used you can use this option to change the report file name.

### `additional-timeout` &lt;`number`&gt;

Default: `5000`  
Command line: `N/A`  
Config file: `"additional-timeout": 3000`

Some mutations can create endless loops inside your code. To detect and stop these loops Stryker cancels a unit test run after a set time.
The formula to calculate the timeout is:

`timeout = initialTestTime + additionalTimeout`

If you have a lot of timeouts you might need to increase the additional timeout. If you have a lot of endless loops causing a long mutation testrun you might want to decrease the additional timeout. Only decrease the additional timeout if you are certain that the mutations are endless loops.

*\* Timeout is in milliseconds.*

### `concurrency` &lt;`number`&gt;

Default: `your number of logical processors / 2`  
Command line: `[-c|--concurrency] 10`  
Config file: `"concurrency": 10`

Change the amount of concurrent workers Stryker uses for the mutation testrun. Defaults to using half your logical (virtual) processor count.

**Example**: an intel i7 quad-core with hyperthreading has 8 logical cores and 4 physical cores. Stryker will use 4 concurrent workers when using the default.

### `thresholds` &lt;`object`&gt;

Default: `{ high: 80, low: 60, break: 0 }`  
Command line: `N/A`  
Config file: `"thresholds": { "high": 80, "low": 60, "break": 0 }`

Configure the mutation score thresholds for your project. Thresholds should be a number between 0 and 100. Thresholds can have the same value.

Threshold calculations in order:
- `mutation score >= threshold-high`: 
    - Awesome! Your reporters will color this green and happy.
- `mutation score < threshold-high && mutation score >= threshold-low`:
    - Warning! Your reporters will display yellow/orange colors, watch out!
- `mutation score < threshold-low`:
    - Danger! Your reporters will display red colors, you're in the danger zone now.
- `mutation score < threshold-break`:
    - Error! Stryker will exit with exitcode 1.

### `break-at` &lt;`number`&gt;

Default: `0`  
Command line: `[-b|--break-at] 40`  
Config file: See [thresholds](#thresholds-object)

Must be less than or equal to threshold low.  
When threshold break is set to anything other than 0 and the mutation score is lower than the threshold Stryker will exit with a non-zero code. This can be used in a CI pipeline to fail the pipeline when your mutation score is not sufficient.  

### `threshold-high` &lt;`number`&gt;

Default: `80`  
Command line: `--threshold-high 90`  
Config file: See [thresholds](#thresholds-object)

Minimum good mutation score. Must be higher than or equal to threshold low. Must be higher than 0.

### `threshold-low` &lt;`number`&gt;

Default: `60`  
Command line: `--threshold-low 40`  
Config file: See [thresholds](#thresholds-object)

Minimum acceptable mutation score. Must be less than or equal to threshold high and more than or equal to threshold break.

### `ignore-mutations` &lt;`string[]`&gt;

Default: `null`  
Command line: `N/A`  
Config file: `"ignore-mutations": ['string', 'logical']`

Ignores mutations that are not currently relevant to your project. Find the mutation names [here](https://stryker-mutator.io/docs/stryker-net/mutations).

#### `Linq expressions`

It's also possible to disable specific linq expressions using:
```json
"stryker-config": {
    "ignore-mutations": [
        "linq.First",
        "linq.Sum"
    ]
}
```

The mutants of the ignored types will not be tested. They will show up in your reports as `Ignored`.

### `ignore-methods` &lt;`string[]`&gt;

Default: `null`  
Command line: `N/A`  
Config file: `"ignore-methods": ['ToString', 'ConfigureAwait', '*Exception.ctor', 'Console.Write*']`

Skip specified method signatures from being mutated. 

```csharp
// This mutation will be skipped
ConfigureAwait(true); 

// This mutation won't because we cannot currently detect this
var t = true;
ConfigureAwait(t); 
```

You can also ignore constructors by specifying the type and adding the `.ctor` suffix.

You can also qualify method names by (partial) class name.

Both, method names and constructor names support wildcards.

```json
"stryker-config": {
    "ignore-methods": [
        "*Log", // Ignores all methods ending with Log
        "Console.Write*", // Ignores all methods starting with Write in the class Console
        "*Exception.ctor" // Ignores all exception constructors
    ]
}
```

### `output` &lt;`string`&gt;

Default: `null`  
Command line: `[--output|-O] /path/to/output`  
Config file: `N/A`

Changes the output path for Stryker logs and reports. This can be an absolute or relative path.

## Optimization

### `coverage-analysis` &lt;`string`&gt;

Default: `perTest`  
Command line: `N/A`  
Config file: `"coverage-analysis": 'off'`

Use coverage info to speed up execution. 

- **perTest**: capture the list of mutants covered by each test. For every mutant that has tests, only the tests that cover the mutant are used to test a mutant. Mutants without tests are reported as `NoCoverage`. Fastest option.
- **perTestInIsolation**: like 'perTest', but running each test in an isolated run. This results in more accurate
coverage information for some mutants (see below), at the expense of a longer startup time.
- **all**: capture the list of mutants covered by a test. Test only the mutants covered by unit tests. Non covered mutants are assumed as survivors. Fast option.
- **off**: coverage data is not captured. All unit tests are run against all mutants.

#### Notes on coverage analysis
* Results should not be impacted by coverage analysis. If you identify a suspicious survivor, run
Stryker again without coverage analysis and report an issue if this mutant is killed by this run.
* When using `perTest` mode, mutants that are executed as part of some static constructor/initializer 
are run against all tests as Stryker cannot reliably capture coverage for those. This is a consequence of static
constructors/initializers being called only once during tests. This heuristic is not needed when using
`perTestInIsolation` due to tests being run one by one.

### `disable-bail` &lt;`flag`&gt;

Default: `false`  
Command line: `--disable-bail`  
Config file: `"disable-bail": true`

Stryker aborts a unit testrun for a mutant as soon as one test fails because this is enough to confirm the mutant is killed. This can reduce the total runtime but also means you miss information about individual unit tests (e.g. if a unit test does not kill any mutants and is therefore useless). You can disable this behavior and run all unit tests for a mutant to completion. This can be especially useful when you want to find useless unit tests.

### `disable-mix-mutants` &lt;`flag`&gt;

Default: `false`  
Command line: `N/A`  
Config file: `"disable-mix-mutants": true`

Stryker combines multiple mutants in the same testrun when the mutants are not covered by the same unit tests. This reduces the total runtime. You can disable this behavior and run every mutation in an isolated testrun. This can be useful when mixed mutants have unintended side effects.

### `since` &lt;`flag`&gt; [`:committish`]

Default: `false`  
Command line: `--since:feat-2`  
Config file: `"since": { }`

Use git information to test only code changes since the given target. Stryker will only report on mutants within the changed code. All other mutants will not have a result.

If you wish to test only changed sources and tests but would like to have a complete mutation report see [with-baseline](#with-baseline-flag-committish).

Set the diffing target on the command line by passing a committish with the since flag in the format `--since:<committish>`.
Set the diffing target in the config file by setting the [since target](#sincetarget-committish) option.

*\* For changes on test project files all mutants covered by tests in that file will be seen as changed.*

### `since.enabled` &lt;`flag`&gt;

Default: `null`  
Command line: `N/A`  
Config file: `"since": { "enabled": false }`

Enable or disable [since](#since-flag-committish). If the enabled property is not set but the `since` object exists in the config file it is assumed to be enabled. Use this option to (temporarily) disable `since` without having to delete the other `since` configuration.

### `since.target` &lt;`committish`&gt;

Default: `master`  
Command line: `N/A`  
Config file: `"since": { "target": 'feat-2' }`

Set the diffing target for the [since](#since-flag-committish) feature.

### `since.ignore-changes-in` &lt;`string[]`&gt;

Default: `null`  
Command line: `N/A`  
Config file: `"since": { "ignore-changes-in: ['**/*Translations.json'] }`

Allows to specify an array of files that should be ignored if present in the diff.
This feature is only recommended when you are sure these files will not affect results, or when you are prepared to sacrifice accuracy for performance.
            
Use [globbing syntax](https://en.wikipedia.org/wiki/Glob_(programming)) for wildcards. Example: ['**/*Assets.json','**/favicon.ico']

## Experimental

**The features in this section are experimental. Results can contain false positives and false negatives.**

## Baseline

### `with-baseline` &lt;`flag`&gt; [`:committish`]

Default: `false`  
Command line: `--with-baseline:feat-2`  
Config file: `"baseline": { }`

Enabling `with-baseline` saves the mutation report to a storage location such as the filesystem. The mutation report is loaded at the start of the next mutation run. Any changed source code or unit test results in a reset of the mutants affected by the change. For unchanged mutants the previous result is reused. This feature expands on the [since](#since-flag-committish) feature by providing you with a full report after a partial mutation testrun.

The report name is based on the current branch name or the [project-info.version](#project-infoversion-committish).

Set the diffing target on the command line by passing a committish with the since flag.
Set the diffing target in the config file by setting the [since target](#sincetarget-committish) option.

*\* The baseline and since features are mutually exclusive. This feature implicitly enables the [since](#since-flag-committish) feature for now.*

### `baseline.enabled` &lt;`flag`&gt;

Default: `null`  
Command line: `N/A`  
Config file: `"baseline": { "enabled": false }`

Enable or disable [with-baseline](#with-baseline-flag-committish). If the enabled property is not set but the `baseline` object exists in the config file it is assumed to be enabled. Use this option to (temporarily) disable `with-baseline` without having to delete the other baseline configuration.

### `baseline.fallback-version` &lt;`string`&gt;

Default: [since-target](#since-flag-committish)  
Command line: `N/A`  
Config file: `"baseline": { "fallback-version": 'develop' }`

When [with-baseline](#with-baseline-flag-committish) is enabled and Stryker cannot find an existing report for the current branch the fallback version is used. When Stryker is still unable to find a baseline we will do a complete instead of partial testrun. The complete testrun will then be saved as the new baseline for the next mutation testrun.

**Example**:
```json
"since-target": 'development',
"current-branch" 'feat-2'
```
```json
baseline exists for branch feat-2: false
baseline exists for branch development: false

baseline used: null (complete instead of partial testrun)
new baseline saved to: feat-2
```
```json
baseline exists for branch feat-2: false
baseline exists for branch development: true

baseline used: development
new baseline saved to: feat-2
```
```json
baseline exists for branch feat-2: true
baseline exists for branch development: true

baseline used: feat-2
new baseline saved to: feat-2
```

*\* The [since-target](#sincetarget-committish) explicit or default value is used as the fallback version unless the fallback version is explicitly set.*

### `baseline.provider` &lt;`string`&gt;

Default: `Disk`  
Command line: `N/A`  
Config file: `"baseline": { "provider": 'AzureFileStorage'}`

Sets the storage provider for the baseline used by [with-baseline](#with-baseline-flag-committish). By default this is set to disk, when the dashboard [reporter](#reporter-string) is enabled this is automatically set to Dashboard.

Supported storage providers are:

| Storage location  | Option | Description |
|------------------ |--------|-------------|
| Disk              | Disk   | Saves the baseline on disk to the `StrykerOutput` folder |
| Stryker Dashboard | Dashboard | Saves the baseline to Stryker Dashboard |
| Azure File Storage | AzureFileStorage | Saves the baseline to Azure File Storage |

For configuring the dashboard provider see [Dashboard Reporter Settings](./reporters.md#dashboard-reporter)

### `baseline.azure-fileshare-url` &lt;`url`&gt;

Default: `null`  
Command line: `N/A`  
Config file: `"baseline": { "azure-fileshare-url": 'https://stryker-net.file.core.windows.net/baselines'}`

When using the azure file storage [provider](#baselineprovider-string) you must set the file share url.
The file share url should be in the the format:

`https://<STORAGE_ACCOUNT_NAME>.file.core.windows.net/<FILE_SHARE_NAME>/<OPTIONAL_SUBFOLDER_NAME>`

The baseline are stored in a folder called `StrykerOutput/Baselines` by default. Or in `StrykerOutput/<projectName>` if a [project name](#project-infoname-string) is set.
Providing a subfolder is optional but allowed. In the case of a custom subfolder the complete url to the baselines would become `https://<FILE_SHARE_URL>/<OPTIONAL_SUBFOLDER_NAME>/StrykerOutput/Baselines`

### `azure-fileshare-sas` &lt;`string`&gt;

Default: `null`  
Command line: `--azure-fileshare-sas "se=2022-08-25T14%3A27Z&sp=rwdl&spr=https&sv=2021-06-08&sr=d&sdd=1&sig=XXXXXXXXXXXXX"`  
Config file: `N/A`

When using the azure file storage [provider](#baselineprovider-string) you must pass credentials for the fileshare to Stryker.
For authentication with the azure fileshare we support Shared Access Signatures (SAS). 

The SAS should be configured with the following properties:

Allowed services: `File`  
Allowed resource types: `Container`, `Object`  
Allowed permissions: `Read`, `Write`, `Create`

For more information on how to configure a SAS check the [Azure documentation](https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview).

## Troubleshooting

### `verbosity` &lt;`log-level`&gt;

Default: `info`  
Command line: `[-V|--verbosity] trace`  
Config file: `"verbosity": 'trace'`

Change the console `verbosity` of Stryker when you want more or less details about the mutation testrun.

All available loglevels are
* error
* warning
* info
* debug
* trace

### `log-to-file` &lt;`flag`&gt;

Default: `false`  
Command line: `[-L|--log-to-file]`  
Config file: `N/A`

When creating an issue on github you can include a logfile so the issue can be diagnosed easier. 

*\* File logging always uses loglevel `trace`.*

### `dev-mode` &lt;`flag`&gt;

Default: `false`  
Command line: `--dev-mode`  
Config file: `N/A`

Stryker will not gracefully recover from compilation errors, but instead crash immediately. Used during development to quickly diagnose errors.  
Also enables more debug logs not generally useful to normal users.

## Misc

### `dashboard-api-key` &lt;`string`&gt;

Default: `null`  
Command line: `--dashboard-api-key "afdfsgarg3wr32r3r32f3f3"`  
Config file: `N/A`
Environment variable: `STRYKER_DASHBOARD_API_KEY="afdfsgarg3wr32r3r32f3f3"`

The API key for authentication with the Stryker dashboard.  
Get your api key at the [stryker dashboard](https://dashboard.stryker-mutator.io/). To keep your api key safe, store it in an encrypted variable in your pipeline.

### `dashboard-url` &lt;`string`&gt;

Default: `https://dashboard.stryker-mutator.io`  
Command line: `N/A`  
Config file: `"dashboard-url": 'https://dev-dashboard.stryker-mutator.io'`

If you're not using the official Stryker Dashboard you can set a custom dashboard url.  
This can be used during Stryker development to not pollute the production dashboard or if you self-host a custom dashboard that adheres to the Stryker Dashboard API.

### `msbuild-path` &lt;`path`&gt;

Default: `null`  
Command line: `--msbuild-path "c://MsBuild/MsBuild.exe"`  
Config file: `N/A`  

By default, Stryker tries to auto-discover msbuild on your system. If Stryker fails to discover msbuild you may supply the path to msbuild manually with this option.

### `break-on-initial-test-failure` &lt;`flag`&gt;

Default: `false`  
Command line: `--break-on-initial-test-failure`  
Config file: `break-on-initial-test-failure`

Instruct Stryker to break execution when at least one test failed on initial test run.
