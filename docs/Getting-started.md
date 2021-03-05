---
title: Getting started
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/Getting-started.md
---

# 1 Install

Stryker.NET can both be installed globally and locally.

### Install globally
```bash
dotnet tool install -g dotnet-stryker
```

### Install in project
Dotnet tools can also be installed on a project level. This requires the following steps:

Create a file called dotnet-tools.json in your project folder, if this is your first local tool.

```bash
dotnet new tool-manifest
```

Then install stryker without the -g flag by executing the following command in the project folder

```bash
dotnet tool install dotnet-stryker
```

Check the `dotnet-tools.json` file into source control

Now the rest of your team can install or update stryker with the following command:

```bash
dotnet tool restore
```

# 2 Prepare

Make sure the working directory for your console is set to the *unit test* project dir.

# 3 Let's kill some mutants
For most projects no configuration is needed. Simply run stryker and it will find your source project to mutate.

```bash
dotnet stryker
```

If more configuration is needed follow the instuctions in your console.

# 4 Configure

Optionally you can add a config file to store your options so they won't have to be passed using CLI each run.

Do this by adding a `stryker-config.json` file to your run location. Now add your config to the file like this:

```
{
    "stryker-config":
    {
        "reporters": [
            "progress",
            "html"
        ]
    }
}
```

For all available configuration see [our configuration page](https://stryker-mutator.io/docs/stryker-net/Configuration).

### Troubleshooting
Have troubles running Stryker? Try running with trace logging.

```bash
dotnet stryker --log-level trace -f
```

Please [report any issues](http://github.com/stryker-mutator/stryker-net/issues) you have or let us know [via Slack](https://join.slack.com/t/stryker-mutator/shared_invite/enQtOTUyMTYyNTg1NDQ0LTU4ODNmZDlmN2I3MmEyMTVhYjZlYmJkOThlNTY3NTM1M2QxYmM5YTM3ODQxYmJjY2YyYzllM2RkMmM1NjNjZjM).
