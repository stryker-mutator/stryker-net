[![Build status](https://ci.appveyor.com/api/projects/status/853yby19lvrrd435/branch/master?svg=true)](https://ci.appveyor.com/project/stryker-mutator/stryker-net/branch/master)
[![Waffle.io - Columns and their card count](https://badge.waffle.io/stryker-mutator/stryker-net.svg?columns=all)](https://waffle.io/stryker-mutator/stryker-net)

# Stryker.NET
*Professor X: For someone who hates mutants... you certainly keep some strange company.*  
*William Stryker: Oh, they serve their purpose... as long as they can be controlled.*

## Introduction

For an introduction to mutation testing and Stryker's features, see [stryker-mutator.io](https://stryker-mutator.io/).

## Getting started
Stryker.NET offers you mutation testing for your .NET Core projects. It allows you to test your tests by temporarily inserting bugs.

#### Note
This project is still in its early days and is not yet available on NuGet. In the meantime, start by [mutation testing your JavaScript](https://stryker-mutator.github.io).

#### Usage
Stryker.NET can be used by executing the `dotnet stryker` command inside your test project folder, using the Stryker.CLI package.

For the full documentation on how to use Stryker.NET, see the [Stryker.CLI readme](/src/Stryker.CLI/README.md).

#### Compatibility
Only compatible with .NET Core version 1.1+

## Usage
For the full documentation about the `dotnet stryker` command, see the [Stryker.CLI readme](/src/Stryker.CLI/README.md).

## Supported Mutators
Right now, Stryker.NET supports the following mutators:
- BinaryExpressionMutator
- BooleanMutator
- PrefixUnaryStatements
- PostfixUnaryStatements
- CheckedMutator

For the full list of all available mutators, see the [Stryker.Core readme](/src/Stryker.Core/README.md).

## Contributing
Want to help develop Stryker.NET? Check out our [contribution guide](/CONTRIBUTING.md).
