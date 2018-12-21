# Stryker.NET Command Line Interface
The Stryker CLI is currently the only implemented way to use Stryker.NET. 

## Getting started
To install Stryker.NET on your *test project* add the following lines to the root of your `.csproj` file.

``` XML
<ItemGroup>
    <DotNetCliToolReference Include="StrykerMutator.DotNetCoreCli" Version="*" />
    <PackageReference Include="StrykerMutator.DotNetCoreCli" Version="*" />
</ItemGroup>
```

After adding the references, install the packages by executing `dotnet restore` inside the project folder.

## Usage
To kick off stryker, execute the following command inside the test project folder:

`dotnet stryker`

## Configuration
While Stryker.NET wants to be a non configuration needed tool, some settings are configurable.

#### Specify your project to mutate
When Stryker finds two or more project references inside your test project, it needs to know what project should be mutated. Pass the name of this project using:

`dotnet stryker --project-file SomeProjectName.csproj`

The name will be matched to the full path. You won't have to pass the full path, as long as the name is unique for the found references.

#### Specify extra timeout time
Some mutations can create endless loops inside your code. To detect and stop these loops, Stryker generates timeouts after some time. Using this parameter you can increase or decrease the time before a timeout will be thrown.

`dotnet stryker --timeout-ms 5000`

Defaults to `30000`

#### Logging to console

`dotnet stryker --log-console <loglevel>`

All available loglevels are:
* error
* warning
* info (default)
* debug
* trace

#### Logging to a file

`dotnet stryker --log-file`

#### Number of maximum concurrent test runners  

`dotnet stryker --max-concurrent-test-runners`


#### Custom thresholds
If you want to decide on your own mutation score thresholds, you can configure this with extra parameters.

`dotnet stryker --threshold-high 90 --threshold-low 75 --threshold-break 50  `

- `mutation score > threshold-high` : Awesome! Your reporters will color this green and happy.
- `threshold-high > mutation score > threshold-low`: Warning! Your reporters will display yellow/orange colors, watch out!
- `threshold-low < mutation score > threshold-break`: Danger! Your reporters will display red colors, you're in the danger zone now.
- `threshold-break > mutation score`: Error! The application will exit with exit code 1.

The defaults are respectivly: `80` `60` `0`

#### Excluding files
If you decide to exclude files for unit testing, you can configure this with the following command:

`dotnet stryker --files-to-exclude ['./ExampleClass.cs', './ExampleDirectory', './ExampleDirectory/ExampleClass2.cs']`

We recommend to use relative paths. When someone else is running Stryker for you unit test project, paths are automatically resolved. However it is also possible to use absolute paths.

When you want to exclude multiple files (for example more then 5) it is advised to use the stryker configuration file because it is easier to handle multiple files.

#### Use a config file
There is also the option to use a config file. To use a config file all you have to do is add a file called "stryker-config.json" in the root of your test project and add a configuration section called stryker-config. Then you can add the options you want to configure to the file.

Example:
``` json
{
    "stryker-config":
    {
        "reporter": "Console",
        "reporters": [
            "ConsoleProgressBar",
            "ConsoleReport"
        ],
        "log-level": "info",
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

##### Note: The no-value options cannot be passed to the config file (like --dev-mode)

##### 

If you want to integrate these settings in your existing settings json, make sure the section is called stryker-config and run stryker with the command `--config-file-path <relativePathToFile>` or `-cp <relativePathToFile>`.
