---
title: Operating modes
sidebar_position: 25
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/operating-modes.md
---
# Operating modes

There are currently three ways to run Stryker:

- From the solution context
- From the test project context
- From the source project context

## Solution file context

When running from your project root (where your solution file is located), Stryker will try and discover a (single) solution file and mutate all projects it can find. 

```bash
cd /my-solution-dir
dotnet stryker
```

You can also pass a solution file using the solution option ([see docs](https://stryker-mutator.io/docs/stryker-net/configuration/#solution-path)).

```bash
cd /my-solution-dir
dotnet stryker --solution "/my-solution-dir/mysolution.sln"
```

> Note that Stryker prioritizes the solution context over the project context if solution and project files are located in the same folder. 

## Test project context

Run `dotnet stryker` in your test project folder.

```bash
cd /my-solution-dir/my-test-project-dir
dotnet stryker
```

Stryker will automatically detect the source project based on the project reference.

If your test project has multiple project references, you need to specify the source project with `--project <filename>` ([see docs](https://stryker-mutator.io/docs/stryker-net/configuration/#project-file-name)). You can only specify one source project at a time.

```bash
cd /my-solution-dir/my-test-project-dir
dotnet stryker --project <filename>
```

> Note that Stryker prioritizes the solution context over the project context if solution and project files are located in the same folder. 

## Source project context

Stryker can also be ran from the source project context. In this case the test project location needs to be passed with `--test-project`.

```bash
cd /my-solution-dir/my-source-project-dir
dotnet stryker --test-project "../my-test-project-dir/tests.csproj"
```

If multiple test projects (ex unit tests, specs) target the same source project you need to specify each test project. You can include multiple test projects like this ([see docs](https://stryker-mutator.io/docs/stryker-net/configuration/#test-projects-string)):

```bash
cd /my-solution-dir/my-source-project-dir
dotnet stryker --test-project "../my-test-project-dir/Tests.csproj" --test-project "../my-test-project-dir/MoreTests.csproj"
```


