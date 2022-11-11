---
title: Operating modes
sidebar_position: 25
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/operating-modes.md
---
# Operating modes

There are currently two ways to run Stryker:

- From the test project context
- From the source project context

You will also be able to run Stryker from your solution context in the future.

## Test project context

Run `dotnet stryker` in your test project folder.

```bash
cd C:\myTestProjectDir
dotnet stryker
```

Stryker will automatically detect the source project based on the project reference.

If your test project has multiple project references, you need to specify the source project with `--project <filename>` ([docs here](https://stryker-mutator.io/docs/stryker-net/configuration/#project-file-name)). You can only specify one source project at the time.

```bash
cd C:\myTestProjectDir
dotnet stryker -p <filename>
```

## Source project context

Run `dotnet stryker --test-project "../Tests/Tests.csproj"` in your source project folder. If multiple test projects (ex unit tests, specs) target the same source project you need to specify each test project. You can include multiple test projects like this ([see docs](https://stryker-mutator.io/docs/stryker-net/configuration/#test-projects-string)):

```bash
cd C:\mySourceProjectDir
dotnet stryker -tp "../Tests/Tests.csproj" -tp "../MoreTests/MoreTests.csproj"
```

## Solution file context

When running from your project root (where your solution file is located) and passing the solution file Stryker will analyze your solution and mutate all projects it can find. [docs here](https://stryker-mutator.io/docs/stryker-net/configuration/#solution-path).

```bash
cd C:\myProjectRoot\
dotnet stryker -s "C:\myProjectRoot\mysolution.sln"
```
