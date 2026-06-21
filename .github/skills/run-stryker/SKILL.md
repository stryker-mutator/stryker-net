# Run Stryker from Terminal

## Purpose

This skill provides quick reference for running Stryker.NET mutation testing from the terminal. Use this skill when you need to execute Stryker tests, configure mutation parameters, or understand available command-line options.

## Basic Usage

### Minimal Command

The quickest way to get started is to use a configuration file:

```bash
cd path/to/test-project
dotnet stryker
```

This will look for a `stryker-config.json` file in the current directory and run with those settings. The integration tests in this repo are set up this way, so you can use them as examples.

### Recommended Options

```bash
dotnet stryker --verbosity "warning" --reporter "cleartext" --reporter "json" --skip-version-check
```

This runs Stryker without the startup banner and outputs a basic result in the terminal plus a JSON report in the output directory. Ideal for AI agents to use fewer tokens while still getting rich output. Parse the terminal output for quick results, and parse the JSON report for details when needed.

## Command-Line Options

### Common Options

| Option | Short | Example | Purpose |
|--------|-------|---------|---------|
| `--solution` | `-s` | `-s "../solution.sln"` | Path to solution file (required for .NET Framework projects) |
| `--project` | `-p` | `-p "MyProject.csproj"` | Project file name to mutate |
| `--test-runner` | `-t` | `-t "mtp"` | Test runner (vstest or mtp) |
| `--reporter` | `-r` | `-r "html" -r "json"` | Output reporters (repeatable) |
| `--mutation-level` | `-l` | `-l "Advanced"` | Mutation scope (Basic, Standard, Advanced, Complete) |
| `--concurrency` | `-c` | `-c "4"` | Parallel workers |
| `--threshold-high` | | `--threshold-high "90"` | High mutation score threshold |
| `--threshold-low` | | `--threshold-low "40"` | Low mutation score threshold |
| `--break-at` | `-b` | `-b "60"` | Exit with code 1 if score below this |
| `--mutate` | `-m` | `-m "**/*Services.cs"` | Files to mutate (glob pattern) |
| `--config-file` | `-f` | `-f "custom.json"` | Config file path |
| `--output` | `-O` | `-O "./results"` | Output directory |
| `--verbosity` | `-V` | `-V "warning"` | Log level (trace, debug, info, warning, error). Using `warning` or higher also suppresses the startup banner. |

## Configuration File Approach (Recommended)

For repeatable and documented Stryker runs, use a `stryker-config.json` file.

### Example stryker-config.json

```json
{
  "stryker-config": {
    "solution": "../MyProject.sln",
    "mutation-level": "Standard",
    "test-runner": "vstest",
    "reporters": ["html", "json", "progress"],
    "thresholds": {
      "high": 80,
      "low": 60,
      "break": 0
    },
    "concurrency": 4,
    "mutate": [
      "**/*.cs",
      "!**/*.Generated.cs",
      "!**/Program.cs"
    ]
  }
}
```

All integration tests in this repo use a `stryker-config.json` file for configuration.

## Configuration Validation

Always ensure your configuration follows these rules:

### File Paths
- Solution path: Must exist and be valid (required for .NET Framework)
- Project path: Use only the project file **name**, not a full path
- Mutate patterns: Use glob syntax with wildcards (`**`, `*`, `?`)

### Test Runner
Valid values:
- `vstest` - Traditional Visual Studio Test Platform (default)
- `mtp` - Microsoft Test Platform (modern, preview)

## Configuration Documentation

For comprehensive configuration reference, see the [official Stryker.NET Configuration Guide](../../../docs/configuration.md).

## Related Skills and Documentation

- [Configuration Reference](../../../docs/configuration.md) - Complete configuration options
- [Mutation Report Parsing](../stryker-json-report-parsing/SKILL.md) - Analyze JSON reports from terminal after running Stryker
