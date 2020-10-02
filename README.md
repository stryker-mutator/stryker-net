[![Nuget](https://img.shields.io/nuget/v/dotnet-stryker.svg?color=blue&label=dotnet-stryker&style=flat-square)](https://www.nuget.org/packages/dotnet-stryker/)
[![Nuget](https://img.shields.io/nuget/dt/dotnet-stryker.svg?style=flat-square)](https://www.nuget.org/packages/dotnet-stryker/)
[![Azure DevOps build](https://img.shields.io/azure-devops/build/stryker-mutator/Stryker/4/master.svg?label=Azure%20Pipelines&style=flat-square)](https://dev.azure.com/stryker-mutator/Stryker/_build/latest?definitionId=4)
[![Azure DevOps tests](https://img.shields.io/azure-devops/tests/stryker-mutator/506a1f46-900e-434e-805f-ff8d36fc81af/4/master.svg?compact_message&style=flat-square)](https://dev.azure.com/stryker-mutator/Stryker/_build/latest?definitionId=4)
[![Slack](https://img.shields.io/badge/chat-on%20slack-blueviolet?style=flat-square)](https://join.slack.com/t/stryker-mutator/shared_invite/enQtOTUyMTYyNTg1NDQ0LTU4ODNmZDlmN2I3MmEyMTVhYjZlYmJkOThlNTY3NTM1M2QxYmM5YTM3ODQxYmJjY2YyYzllM2RkMmM1NjNjZjM)

# Stryker.NET
*Professor X: For someone who hates mutants... you certainly keep some strange company.*
*William Stryker: Oh, they serve their purpose... as long as they can be controlled.*

## Introduction
Stryker.NET offers you mutation testing for your .NET Core and .NET Framework projects. It allows you to test your tests by temporarily inserting bugs. 

For an introduction to mutation testing and Stryker's features, see [stryker-mutator.io](https://stryker-mutator.io/). Looking for mutation testing in [JavaScript & Typescript](https://stryker-mutator.github.io/stryker) or [Scala](https://stryker-mutator.github.io/stryker4s)?

## Getting started

Stryker.NET is installed using [NuGet](https://www.nuget.org/packages/dotnet-stryker/) as a dotnet core global tool.

<details>
  <summary>Read more</summary>
 
 Stryker.NET can be installed in one of these ways:

### Global install
`dotnet tool install -g dotnet-stryker`

### Project install
Starting from dotnet core 3.0 dotnet tools can also be installed on a project level. This requires the following steps:

Create a file called dotnet-tools.json in your project folder. You can checkin to version control to make sure all team members have access to stryker

`dotnet new tool-manifest` 

Then install stryker without the -g flag while executing the following command in the project folder

`dotnet tool install dotnet-stryker`

Now you can run Stryker.NET from your test project directory by executing:

`dotnet stryker`

</details>

### Documentation
For the full documentation on how to use Stryker.NET, see our [configuration docs](/docs/Configuration.md).

### Update stryker dotnet tool
Dotnet global tools do not auto update. To update stryker as a global tool run `dotnet tool update --global dotnet-stryker`.
Stryker will notify you when a new version is available on every run.

#### Compatibility
Runs on test projects targeting:
 - netcoreapp 1.1+
 - netframework 4.5+

#### Requirements
Dotnet core runtime 3.1+ needs to be available on your system to run dotnet stryker.

For .NET Framework projects, Stryker.NET requires [nuget.exe](https://docs.microsoft.com/en-us/nuget/install-nuget-client-tools#windows) to be installed on your system. Please follow their installation instructions.

## Supported Mutators
Right now, Stryker.NET supports the following mutations:
- Arithmetic Operators
- Equality Operators
- Boolean Literals
- Assignment statements
- Collection initialization
- Unary Operators
- Update Operators
- Checked Statements
- Linq Methods
- String Literals
- Bitwise Operators

For the full list of all available mutations, see the [mutator docs](/docs/Mutators.md).

## Supported Reporters
- Html reporter
- Dashboard reporter
- Console reporter
- Progress reporter
- Console dots reporter
- Json reporter

For the full list of all available reporters, see the [reporter docs](/docs/Reporters.md).

## Contributing
Want to help develop Stryker.NET? Check out our [contribution guide](/CONTRIBUTING.md).

Issues for the HTML report should be issued at https://github.com/stryker-mutator/mutation-testing-elements
