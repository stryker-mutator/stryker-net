# Copilot Instructions for Stryker.NET

This document provides guidance for GitHub Copilot when working with the Stryker.NET repository.

## Additional Copilot Instructions

Reference these instruction files when applicable:
- **Always**: [Taming Copilot](./instructions/taming-copilot.instructions.md)
- **Always**: [Conventional Commit Guidelines](./instructions/conventional-commit.instructions.md)
- **Always**: [Security Best Practices](./instructions/security-and-owasp.instructions.md)
- **Always**: [Self-Explanatory Code Commenting](./instructions/self-explanatory-code-commenting.instructions.md)
- **When writing documentation**: [Markdown Instructions](./instructions/markdown.instructions.md)
- **When writing C#**: [C# Instructions](./instructions/csharp.instructions.md)
- **When working with Azure Pipelines**: [Azure DevOps Pipelines](./instructions/azure-devops-pipelines.instructions.md)
- **When working with GitHub Actions**: [GitHub Actions](./instructions/github-actions-ci-cd-best-practices.instructions.md)

## Project Overview

Stryker.NET is a mutation testing framework for .NET projects. It allows you to test your tests by temporarily inserting bugs (mutations) in your source code to verify that tests catch them. Some of the key features of Stryker.NET include:

- Large set of built-in mutators for C# code
- Support for multiple test frameworks (xUnit, NUnit, MSTest, TUnit)
- Integration with Visual Studio Test platform
- Integration with the Microsoft Testing Platform (MTP)
- Detailed reporting of mutation testing results using [html and json report formats](https://github.com/stryker-mutator/mutation-testing-elements)
- Support for .NET Core and .NET Framework projects
- Reporting to the [Stryker Dashboard](https://dashboard.stryker-mutator.io/)
- Configurable mutation testing options and thresholds

Directory structure:
- `/src`: Main source code for Stryker.NET
  - `/src/Stryker.CLI`: Command-line interface for running Stryker.NET
  - `/src/Stryker.Core`: Core mutation testing engine. Contains the logic for analyzing projects, generating mutants, and reporting results.
  - `/src/Stryker.TestRunner`: Test runner integration for executing tests during mutation testing
  - `/src/Stryker.TestRunner.VsTest`: Test runner using the VsTest adapter for running tests with Visual Studio Test framework
  - `/src/Stryker.TestRunner.MicrosoftTestPlatform`: Test runner for Microsoft Testing Platform (MTP)
  - `/src/Stryker.Configuration`: Configuration and options management for Stryker.NET
  - `/src/Stryker.Abstractions`: Common interfaces and abstractions used across the project
  - `/src/Stryker.Utilities`: Utility functions and shared code used across the project
- `/docs`: Documentation for Stryker.NET
- `/integrationtest`: Integration tests for Stryker.NET
  - `/integrationtest/TargetProjects`: Target projects used for testing major features of stryker. For example different runtimes, test frameworks, and project types. These projects are used for testing stryker's core features.
  - `/integrationtest/Validation`: The tests validating the results of running stryker on the target projects
- `/ExampleProjects`: Example projects used only for testing F#

## Contributing Workflow

### Code Standards

- Follow the repository's `.editorconfig` and [Microsoft C# coding guidelines](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- Create or edit unit tests or integration tests for all changes
- Update documentation when adding features

### Pull Request Title Convention

When creating or updating pull requests, **always** use Angular-style conventional commit format for PR titles:
- Format: `<type>(<scope>): <subject>`
- Types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`
- Scope: The file or group of files affected (optional but recommended)
- Subject: A short, imperative description (present tense)
- Example: `feat(mutators): add string mutator support`
- Example: `fix(cli): resolve configuration parsing issue`
- Example: `docs: update contributor guidelines`

**Why**: The project uses squash merging, so the PR title becomes the commit message in the main branch history.

### Running Tests

- **Unit tests**: Use #tool:execute/runTests or when the tool is not available run `dotnet test`
- **Integration tests**:
  - On **Windows**: Run `.\integration-tests.ps1` in the root of the repo (PowerShell 7 recommended)
  - On **macOS/Linux**: Run `pwsh ./integration-tests.ps1` in the root of the repo (requires [PowerShell 7](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell))
- Always run unit tests and integration tests after making a change

### Testing Locally

To test Stryker.NET on a project in a terminal, you can build Stryker.NET and then run the resulting `Stryker.CLI.dll` on the target project. Run `dotnet <path-to-stryker.dll>` in the root of the project you want to test (adjust path as needed based on your build configuration).

For example in the `/integrationtest/TargetProjects/NetCore/TargetProject` directory, you can run `dotnet ../../../../src/Stryker.CLI/Stryker.CLI/bin/Debug/net8.0/Stryker.CLI.dll` to run the locally built Stryker.NET on the target projects.

Keep in mind the different runmodes of Stryker.NET:
- **Solution context mode**: Run Stryker.NET from the root of a project containing a solution file (`.sln`). In this mode, Stryker.NET will analyze the solution and run mutation testing on all projects in the solution.
- **Project context mode**: Run Stryker.NET from the root of a project containing a project file (`.csproj`). In this mode, Stryker.NET will analyze the project and run mutation testing on that specific project.
- **Test Context mode**: Run Stryker.NET in a directory with a test project file (`.csproj`) and specify the path to the target project using the `--project` option. In this mode, Stryker.NET will analyze the specified target project and run mutation testing on it, while using the test project for running tests.

### Running Stryker on itself

Running Stryker on itself doesn't work as assemblies will be in use. To run Stryker on the stryker codebase, use the official nuget release via `dotnet tool install dotnet-stryker` and then `dotnet stryker`. Or use the stryker-on-stryker.ps1 script in the root of the repo which will build Stryker and then run it on itself using the built dll.

## Adding a mutator

When adding a mutator see the full guide in [adding_a_mutator.md](../adding_a_mutator.md).
