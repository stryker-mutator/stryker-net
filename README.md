[![Nuget](https://img.shields.io/nuget/v/dotnet-stryker.svg?color=blue&label=dotnet-stryker&style=flat-square)](https://www.nuget.org/packages/dotnet-stryker/)
[![Nuget](https://img.shields.io/nuget/dt/dotnet-stryker.svg?style=flat-square)](https://www.nuget.org/packages/dotnet-stryker/)
[![Azure DevOps build](https://img.shields.io/azure-devops/build/stryker-mutator/Stryker/4/master.svg?label=Azure%20Pipelines&style=flat-square)](https://dev.azure.com/stryker-mutator/Stryker/_build/latest?definitionId=4)
[![Azure DevOps tests](https://img.shields.io/azure-devops/tests/stryker-mutator/506a1f46-900e-434e-805f-ff8d36fc81af/4/master.svg?compact_message&style=flat-square)](https://dev.azure.com/stryker-mutator/Stryker/_build/latest?definitionId=4)
[![Slack](https://img.shields.io/badge/chat-on%20slack-blueviolet?style=flat-square)](https://join.slack.com/t/stryker-mutator/shared_invite/enQtOTUyMTYyNTg1NDQ0LTU4ODNmZDlmN2I3MmEyMTVhYjZlYmJkOThlNTY3NTM1M2QxYmM5YTM3ODQxYmJjY2YyYzllM2RkMmM1NjNjZjM)

⚠ You are currently looking at the V1.0 branch. We are working towards the next big release. All new features will be merged here. For currently released sources and for providing fixes see [master](https://github.com/stryker-mutator/stryker-net/tree/master)

# Stryker.NET
*Professor X: For someone who hates mutants... you certainly keep some strange company.*
*William Stryker: Oh, they serve their purpose... as long as they can be controlled.*

## Introduction
Stryker offers mutation testing for your .NET Core and .NET Framework projects. It allows you to test your tests by temporarily inserting bugs in your source code

For an introduction to mutation testing and Stryker's features, see [stryker-mutator.io](https://stryker-mutator.io/). Looking for mutation testing in [JavaScript & Typescript](https://stryker-mutator.github.io/stryker) or [Scala](https://stryker-mutator.github.io/stryker4s)?

## Getting started

Stryker is installed using [NuGet](https://www.nuget.org/packages/dotnet-stryker/) as a dotnet core tool

<details>
  <summary>Read more</summary>

### Install globally
`dotnet tool install -g dotnet-stryker`

### Install in project
Starting from dotnet core 3.0 dotnet tools can also be installed on a project level. This requires the following steps:

Create a file called dotnet-tools.json in your project folder

`dotnet new tool-manifest` 

Then install stryker without the -g flag by executing the following command in the project folder

`dotnet tool install dotnet-stryker`

Check the `dotnet-tools.json` file into source control

Now the rest of your team can install or update stryker with the following command:
`dotnet tool restore`

Now you can run stryker from your test project directory by executing:

`dotnet stryker`

### Updating stryker
Dotnet tools do not auto update so you are responsible for making sure you're up-to-date. To help with this stryker will notify you when a new version is available

To update stryker as a global tool run `dotnet tool update --global dotnet-stryker`

To update stryker as a project tool run `dotnet tool update --local dotnet-stryker` or change the version in the `dotnet-tools.json` file. Then check in the updated `dotnet-tools.json` file.

</details>

### Documentation
For the full documentation on how to use Stryker.NET, see our [configuration docs](/docs/Configuration.md)

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
