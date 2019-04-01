[![Nuget](https://img.shields.io/nuget/v/StrykerMutator.DotNetCoreCli.svg)](https://www.nuget.org/packages/StrykerMutator.DotNetCoreCli/)
[![Nuget](https://img.shields.io/nuget/dt/StrykerMutator.DotNetCoreCli.svg)](https://www.nuget.org/packages/StrykerMutator.DotNetCoreCli/)

[![Nuget](https://img.shields.io/nuget/v/dotnet-stryker.svg)](https://www.nuget.org/packages/dotnet-stryker/)
[![Nuget](https://img.shields.io/nuget/dt/dotnet-stryker.svg)](https://www.nuget.org/packages/dotnet-stryker/)
[![Build Status](https://dev.azure.com/stryker-mutator/Stryker/_apis/build/status/stryker-net)](https://dev.azure.com/stryker-mutator/Stryker/_build/latest?definitionId=4)
[![Gitter](https://badges.gitter.im/stryker-mutator/stryker-net.svg)](https://gitter.im/stryker-mutator/stryker-net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

# Stryker.NET
*Professor X: For someone who hates mutants... you certainly keep some strange company.*  
*William Stryker: Oh, they serve their purpose... as long as they can be controlled.*

## Introduction

For an introduction to mutation testing and Stryker's features, see [stryker-mutator.io](https://stryker-mutator.io/). Looking for mutation testing in [JavaScript & Typescript](https://stryker-mutator.github.io/stryker) or [Scala](https://stryker-mutator.github.io/stryker4s)?

## Getting started
Stryker.NET offers you mutation testing for your .NET Core projects. It allows you to test your tests by temporarily inserting bugs. Stryker.NET is installed using [NuGet](https://www.nuget.org/packages/dotnet-stryker/) as a dotnet core global tool. Stryker.NET can be installed in one of these ways:

### Global install
`dotnet tool install -g dotnet-stryker`

### Project install
Starting from dotnet core 3.0 dotnet tools can also be installed on a project level. This requires the following steps:

Create a file called dotnet-tools.json in your project folder. You can checkin to version control to make sure all team members have access to stryker

`dotnet new tool-manifest` 

Then install stryker without the -g flag while executing the following command in the project folder

`dotnet tool install dotnet-stryker`

### Update stryker dotnet tool
Dotnet global tools do not auto update. To update stryker as a global tool run `dotnet tool update`

For the full documentation on how to use Stryker.NET, see our [website](http://stryker-mutator.io/stryker-net/quickstart).

#### Upgrading from csproj install?
The old way of installing stryker is deprecated and replaced by the dotnet core tool. For a guide on how to upgrade see our [blog](http://stryker-mutator.io/blog/2019-03-15/announcing-stryker-1-0).

#### Compatibility
Runs on test projects targeting:
 - netcoreapp 1.1+
 - netframework 4.5+

#### Requirements
Dotnet core runtime 2.2+ needs to be available on your system to run dotnet stryker.

For .NET Framework projects, Stryker.NET requires [nuget.exe](https://docs.microsoft.com/en-us/nuget/install-nuget-client-tools#windows) to be installed on your system. Please follow their installation instructions.

## Supported Mutators
Right now, Stryker.NET supports the following mutations:
- Arithmetic Operators
- Equality Operators
- Boolean Literals
- Assignment statements
- Unary Operators
- Update Operators
- Checked Statements
- Linq Methods
- String Literals

For the full list of all available mutations, see the [website](https://stryker-mutator.io/stryker-net/mutators).

## Contributing
Want to help develop Stryker.NET? Check out our [contribution guide](/CONTRIBUTING.md).
