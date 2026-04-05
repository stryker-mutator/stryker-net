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

Stryker.NET is a mutation testing framework for .NET projects. It allows you to test your tests by temporarily inserting bugs (mutations) in your source code to verify that tests catch them.

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

- **Unit tests**: Run `dotnet test` in the `/src` directory or use your IDE's test runner
- **Integration tests**: 
  - On **Windows**: Run `.\integration-tests.ps1` in the root of the repo (PowerShell 7 recommended)
  - On **macOS/Linux**: Run `pwsh ./integration-tests.ps1` in the root of the repo (requires [PowerShell 7](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell))
- Always run unit tests and integration after making a change to ensure nothing is broken

### Testing Locally

To test Stryker.NET on a project:
1. In `Stryker.CLI`, open `properties > Debug`
2. Create a new Debug profile
3. Set `Launch` as `Project`
4. Set `WorkingDirectory` to a unit test project directory
5. You can use projects in `.\integrationtest\TargetProjects` for testing
6. Run with `Stryker.CLI` as the startup project

**Note**: Running Stryker on itself doesn't work as assemblies will be in use. To run Stryker on the stryker codebase, use the official nuget release via `dotnet tool install dotnet-stryker` and then `dotnet stryker`.

