---
title: Reporters
sidebar_position: 60
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/reporters.md
---

Stryker supports a variety of reporters. Enabled reporters will be activated during or after your Stryker run. 

The default reporters are [html](#html-reporter) and [progress](#progress-reporter)

# Html reporter
Our html reporter will output a html file that will visually display your project and all mutations. This is our recomended reporter for larger projects, since it displays large number of mutations in a clear way. 

```bash
dotnet stryker --reporter "html"
```

Example:

![html reporter](./images/html-report-net.png)

# Progress reporter
This reporter outputs the current status of the mutation testrun. It has a nice visual look so you can quickly see the progress. We recomend to use this reporter on large projects. It also shows an indication of the estimated time for Stryker.NET to complete.

```bash
dotnet stryker --reporter "progress"
```

Example:

![progress bar reporter](./images/progress-bar-net.png)

# Dashboard reporter
The dashboard reporter will upload your stryker result as json to the [stryker dashboard](https://dashboard.stryker-mutator.io/). To use this reporter some settings should be configured:

```bash
dotnet stryker --reporter "dashboard"
```

The following options are relevant when using the dashboard reporter:
- [Api key](./configuration.md#dashboard-api-key-<string>) - required
- [Project name](./configuration.md#project-info.name-<string>) - required
- [Project version](./configuration.md#project-info.version-<string>) - required
- [Project module](./configuration.md#project-info.module-<string>) - optional

# Cleartext reporter
It displays all files right after the mutation testrun is done. Ideal for a quick run, as it leaves no file on your system.

```bash
dotnet stryker --reporter "cleartext"
```

Example:

![console reporter](./images/console-reporter-net.png)

# Cleartext tree reporter
It displays all mutations right after the mutation testrun is done. Ideal for a quick run, as it leaves no file on your system. It is recommended to turn this reporter off on big projects.

```bash
dotnet stryker --reporter "cleartexttree"
```

Example:

![Cleartext reporter](./images/console-reporter-tree.png)

# Dots reporter
A basic reporter to display the progress of the mutationtest run. It indicates in a very simple way how many mutants have been tested and their status. This is ideal to use on build servers, as it has little/no performance loss while still giving insight.

```bash
dotnet stryker --reporter "dots"
```

Example:

![Dots reporter](./images/console-dots-reporter-net.png)

Where `"."` means killed, `"S"` means survived and `"T"` means timed out.

# Json reporter
This reporter outputs a json file with all mutation testrun info of the last run. The json is also used for the HTML reporter, but using this reporter you could use the file for your own purposes.

```bash
dotnet stryker --reporter "json"
```
