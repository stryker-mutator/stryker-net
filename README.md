[![Build Status](https://dev.azure.com/stryker-mutator/Stryker/_apis/build/status/stryker-net)](https://dev.azure.com/stryker-mutator/Stryker/_build/latest?definitionId=4)
[![Gitter](https://badges.gitter.im/stryker-mutator/stryker-net.svg)](https://gitter.im/stryker-mutator/stryker-net?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![Waffle.io - Columns and their card count](https://badge.waffle.io/stryker-mutator/stryker-net.svg?columns=To%20Do,In%20Progress,Needs%20Review)](https://waffle.io/stryker-mutator/stryker-net)

# Stryker.NET
*Professor X: For someone who hates mutants... you certainly keep some strange company.*  
*William Stryker: Oh, they serve their purpose... as long as they can be controlled.*

## Introduction

For an introduction to mutation testing and Stryker's features, see [stryker-mutator.io](https://stryker-mutator.io/). Looking for [mutation testing in JavaScript](https://stryker-mutator.github.io)?

## Getting started
Stryker.NET offers you mutation testing for your .NET Core projects. It allows you to test your tests by temporarily inserting bugs. Stryker.NET is installed using [NuGet](https://www.nuget.org/packages/Stryker.Runner.DotNetCore.CLI/).

#### Install
 To install Stryker.NET on your *test project* add the following lines to the root of your `.csproj` file. on your *test* project. 

``` XML
<ItemGroup>
    <DotNetCliToolReference Include="StrykerMutator.DotNetCoreCli" Version="*" />
    <PackageReference Include="StrykerMutator.DotNetCoreCli" Version="*" />
</ItemGroup>
```

After adding the references, install the packages by executing `dotnet restore` inside the project folder.

#### Usage
Stryker.NET can be used by executing the `dotnet stryker` command inside your test project folder, using the Stryker.CLI package.

For the full documentation on how to use Stryker.NET, see the [Stryker.CLI readme](/src/Stryker.CLI/README.md).

#### Compatibility
Only compatible with .NET Core version 1.1+

## Supported Mutators
Right now, Stryker.NET supports the following mutators:
- Binary Operators
- Boolean Substitutions
- Assignment Mutator
- Unary Operators
- Update Operators
- Checked Mutator
- LINQ Mutator
- String Mutator

For the full list of all available mutators, see the [Stryker.Core readme](/src/Stryker.Core/README.md).

## Contributing
Want to help develop Stryker.NET? Check out our [contribution guide](/CONTRIBUTING.md).
