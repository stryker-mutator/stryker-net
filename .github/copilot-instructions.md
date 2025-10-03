# Copilot Instructions for Stryker.NET

This document provides guidance for GitHub Copilot when working with the Stryker.NET repository.

## Additional Copilot Instructions

Reference these external instruction files when applicable:
- **Always**: [Taming Copilot](https://github.com/github/awesome-copilot/blob/main/instructions/taming-copilot.instructions.md)
- **When writing documentation**: [Markdown Instructions](https://github.com/github/awesome-copilot/blob/main/instructions/markdown.instructions.md)
- **When writing C#**: [C# Instructions](https://github.com/github/awesome-copilot/blob/main/instructions/csharp.instructions.md)

## Project Overview

Stryker.NET is a mutation testing framework for .NET projects. It allows you to test your tests by temporarily inserting bugs (mutations) in your source code to verify that tests catch them.

## Contributing Workflow

### Creating Issues
- Create an issue first before working on new features or bug fixes
- For bugs, run `dotnet stryker --verbosity debug` and include the output in the issue

### Code Standards
- Follow the repository's `.editorconfig` and [Microsoft C# coding guidelines](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- Create or edit unit tests or integration tests for all changes
- Update documentation when adding features

### Development Setup
1. Clone the repository: `https://github.com/stryker-mutator/stryker-net.git`
2. Open `Stryker.sln` in Visual Studio or your preferred IDE
3. The solution contains multiple projects under the `src/` directory

### Running Tests
- **Unit tests**: Run `dotnet test` in the `/src` directory
- **Integration tests**: These will be run automatically in the CI/CD pipeline during pull requests
- Always run unit tests before committing changes

### Testing Locally
To test Stryker.NET on a project:
1. In `Stryker.CLI`, open `properties > Debug`
2. Create a new Debug profile
3. Set `Launch` as `Project`
4. Set `WorkingDirectory` to a unit test project directory
5. You can use projects in `.\integrationtest\TargetProjects` for testing
6. Run with `Stryker.CLI` as the startup project

**Note**: Running Stryker on itself doesn't work as assemblies will be in use by Visual Studio. You can run Stryker on a copy of Stryker instead.

## Adding a New Mutator

See the full guide in [adding_a_mutator.md](../adding_a_mutator.md).

### Key Points for Mutators
1. **Purpose**: Generate mutations that look like possible human errors, not just any mutation
2. **Performance**: Keep mutators fast - they're called on every syntax element
3. **Buildable**: Generated mutations should compile in most situations
4. **Killable**: Avoid mutations that always raise exceptions or are semantically equivalent
5. **General**: Mutators should work for all projects, not be framework-specific

### Implementation Steps
1. Create a class inheriting from `MutatorBase<T>` and implementing `IMutator`
2. Specify the expected `SyntaxNode` class you can mutate (e.g., `StatementSyntax`)
3. Override the `MutationLevel` property (typically `Complete` or `Advanced`)
4. Override `ApplyMutation<T>` to generate mutations
5. Add an entry in the `Mutator` enum
6. Create an instance in the `CsharpMutantOrchestrator` constructor
7. Add comprehensive unit tests
8. Update [docs/mutations.md](../docs/mutations.md)

### Mutator Guidelines
- Use Roslyn APIs to work with syntax trees, not text transformations
- Each mutator is called on every syntax element recursively
- Return an empty list or `yield break` if no mutation can be generated
- Mutators must not throw exceptions
- Support various C# syntax versions (expression body vs block statement)
- Invest in unit tests early - look at existing mutator tests for examples

### Helpful Resources
- [.NET Compiler Platform SDK](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/get-started/syntax-analysis) - use the Syntax Visualizer in Visual Studio
- [Roslyn Quoter](http://roslynquoter.azurewebsites.net/) - determine SyntaxFactory AST builder methods
- [Sharplab](https://sharplab.io/) - visualize compilation steps (AST, IL, etc.)

## Commit Message Format

Follow [Angular commit message style](https://docs.google.com/document/d/1rk04jEuGfk9kYzfqCuOlPTSJw3hEDZJTBN5E5f1SALo):
- Format: `<type>(<scope>): <subject>\n\n[body]`
- Types: feat, fix, docs, style, refactor, test, chore
- Use present tense (e.g., "change" not "changed", "add" not "added")
- Include motivation and contrasts with previous behavior

## Community and Support

- Ask questions via GitHub discussions or [project Slack](https://join.slack.com/t/stryker-mutator/shared_invite/enQtOTUyMTYyNTg1NDQ0LTU4ODNmZDlmN2I3MmEyMTVhYjZlYmJkOThlNTY3NTM1M2QxYmM5YTM3ODQxYmJjY2YyYzllM2RkMmM1NjNjZjM)
- Report HTML report issues at [mutation-testing-elements](https://github.com/stryker-mutator/mutation-testing-elements)
- See [CONTRIBUTING.md](../CONTRIBUTING.md) for more details
