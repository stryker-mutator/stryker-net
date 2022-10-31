---
title: Operating modes
sidebar_position: 25
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/operating-modes.md
---
# Operating modes

There are currently two different ways to run Stryker:

- From the test project context
- From the source project context

You will also be able to run Stryker from your solution context in the future.

## Test project context

Just run

```bash
dotnet stryker
```

in your test project folder. Stryker will automatically detect the target project based on the project reference. If your test project has multiple project references, you need to specify the target project with `--project <filename>` ([docs here](https://stryker-mutator.io/docs/stryker-net/configuration/#project-file-name)). You can only specify one target project at the time.

## Target project context

Run

```bash
dotnet stryker -tp "../Tests/Tests.csproj"
```

in your target project folder. If multiple test projects (ex unit tests, specs) target the same source project you need to specify each test project. You can include multiple test projects like this ([see docs](https://stryker-mutator.io/docs/stryker-net/configuration/#test-projects-string)):

```bash
dotnet stryker -tp "../Tests/Tests.csproj" -tp "../MoreTests/MoreTests.csproj"
```

## Solution file context

Coming soon!
