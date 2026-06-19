---
title: Migration guide
sidebar_position: 90
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/migration-guide.md
---

# Migration guide

This guide instructs how to update major versions of Stryker.NET.

## V4.x --> V5.x

### ⏭ Updated runtime

The .NET runtime requirement for Stryker.NET has been changed. Dotnet 10 runtime is now required.

Please [download and install the dotnet 10 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and update your pipeline have the dotnet 10 runtime available.

_Note it is not neccesary to target dotnet 10 in your projects. Dotnet 10 is a runtime requirement for stryker, we will still compile your project to your existing target frameworks._

## V3.x --> V4.x

### ⏭ Updated runtime

The .NET runtime requirement for Stryker.NET has been changed. Dotnet 8 runtime is now required.

Please [download and install the dotnet 8 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) and update your pipeline have the dotnet 8 runtime available.

_Note it is not neccesary to target dotnet 8 in your projects. Dotnet 8 is a runtime requirement for stryker, we will still compile your project to your existing target frameworks._

## V2.x --> V3.x

### :pushpin: Baseline

The allowed format of the [azure-fileshare-sas](./configuration.md#azure-fileshare-sas-string) option has changed. SAS must contain `sv=` and `sig=` to be valid. SAS without sv= are no longer transformed to valid SAS. If you supply stryker with an azure fileshare sas that does not include `sv=` or `sig=` you will have to update your azure-fileshare-sas.

## V1.x --> V2.x

### ⏭ Updated runtime

The .NET runtime requirement for Stryker.NET has been updated from dotnet 5.0 to dotnet 6.0 LTS.

Please [download and install the dotnet 6 runtime](https://dotnet.microsoft.com/download/dotnet/6.0) and update your pipeline have the dotnet 6 runtime available.

_Note that you do not have to update your application to target dotnet 6. Dotnet 6 is only a runtime requirement for stryker to be able to run on your system._

## V0.x --> V1.x

### ⏭ Updated runtime

The .NET runtime requirement for Stryker.NET has been updated from 3.1 to 5.0.

Please [download and install the dotnet 5 runtime](https://dotnet.microsoft.com/download/dotnet/5.0) and update your pipeline have the dotnet 5 runtime available.

_Note that you do not have to update your application to use dotnet 5. Dotnet 5 is only a runtime requirement for stryker to be able to run on your system._

### ⏭ Options rework

Almost all options have been renamed or work different. A fundamental difference on the CLI is how multi value options are passed.

#### Multi value options

The old annotation for passing multi value options was confusing and not based on any standards. For example this is how multiple reporters were passed 👎

```shell
dotnet stryker --reporters "['html', 'progress']"
```

This now looks like 👍

```shell
dotnet stryker --reporter "html" --reporter "progress"
```

#### Options migration guide

A lot of options have been renamed. We have also decided that some options either do not belong on the commandline or don't belong in the configuration file. For example an API key should not be stored in the configuration file so that possibility has been removed.

Options migration overview:

| Old cli                       | New cli                                     | Old json                      | New json                     |
| ----------------------------- | ------------------------------------------- | ----------------------------- | ---------------------------- |
| config-file-path              | f \| config-file                            | ❌                            | ❌                           |
| max-concurrent-testrunners    | c \| concurrency                            | max-concurrent-testrunners    | concurrency                  |
| dev-mode                      | dev-mode                                    | dev-mode                      | ❌                           |
| solution-path                 | s \| solution                               | solution-path                 | solution                     |
| log-file                      | L \| log-to-file                            | log-file                      | ❌                           |
| log-level                     | V \| verbosity                              | log-level                     | verbosity                    |
| mutation-level                | l \| mutation-level                         | mutation-level                | mutation-level               |
| threshold-high                | threshold-high                              | thresholds.high               | thresholds.high              |
| threshold-low                 | threshold-low                               | thresholds.low                | thresholds.low               |
| threshold-break               | b \| break-at                               | thresholds.break              | thresholds.break             |
| reporters                     | r \| reporter (flag allowed multiple times) | reporters                     | reporters                    |
| project-file                  | p \| project                                | project-file                  | project                      |
| diff                          | since                                       | diff                          | since                        |
| timeout-ms                    | ❌                                          | timeout-ms                    | additional-timeout           |
| excluded-mutations            | ❌                                          | excluded-mutations            | ignore-mutations             |
| ignore-methods                | ❌                                          | ignore-methods                | ignore-methods               |
| mutate                        | m \| mutate                                 | mutate                        | mutate                       |
| language-version              | ❌                                          | language-version              | language-version             |
| coverage-analysis             | ❌                                          | coverage-analysis             | coverage-analysis            |
| abort-test-on-fail            | ❌                                          | abort-test-on-fail            | disable-bail                 |
| disable-testing-mix-mutations | ❌                                          | disable-testing-mix-mutations | disable-mix-mutants          |
| test-projects                 | ❌                                          | test-projects                 | test-projects                |
| dashboard-url                 | ❌                                          | dashboard-url                 | dashboard-url                |
| dashboard-api-key             | dashboard-api-key                           | dashboard-api-key             | ❌                           |
| project-name                  | ❌                                          | dashboard-project             | project-info.name            |
| module-name                   | ❌                                          | dashboard-module              | project-info.module          |
| dashboard-version             | v \| version                                | dashboard-version             | project-info.version         |
| diff-ignore-files             | ❌                                          | diff-ignore-files             | since.ignore-changes-in      |
| azure-storage-url             | ❌                                          | azure-storage-url             | baseline.azure-fileshare-url |
| dashboard-fallback-version    | ❌                                          | dashboard-fallback-version    | baseline.fallback-version    |
| baseline-storage-location     | ❌                                          | baseline-storage-location     | baseline.provider            |
| dashboard-compare             | with-baseline                               | dashboard-compare             | baseline                     |
| git-diff-target               | since                                       | git-diff-target               | since.target                 |
| azure-storage-sas             | azure-fileshare-sas                         | azure-storage-sas             | ❌                           |
| files-to-exclude              | ❌                                          | files-to-exclude              | ❌                           |
| test-runner                   | ❌                                          | test-runner                   | ❌                           |
