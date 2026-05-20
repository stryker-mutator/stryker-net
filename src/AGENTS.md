# AGENTS — How to be productive in the Stryker.NET repo

Checklist for an AI coding agent
- Read `Stryker.Abstractions` to understand the public API (mutations, types, interfaces).
- Read `Stryker.Core` for the mutation-generation and orchestration logic (high-level behavior).
- Inspect `Stryker.TestRunner.*` and `Stryker.DataCollector` to understand how tests are executed and how coverage is collected.
- Use `Stryker.Configuration` and `Stryker.Solutions` for how inputs (patterns, solution resolution) are parsed.

Quick architecture (big picture)
- Stryker is split into clear layered components:
  - `Stryker.Abstractions` — shared types/interfaces used across all components (Mutation, Mutator enum, IProjectAndTests, ID providers).
  - `Stryker.Core` — core mutation generation, orchestration and high-level mutation test loop.
  - `Stryker.TestRunner.*` — pluggable test runner implementations (e.g. `MicrosoftTestPlatform`, `VsTest`) responsible for executing tests against mutated code.
  - `Stryker.DataCollector` — a VSTest in-proc data collector (see `CoverageCollector.cs`) used to gather coverage and map tests → mutants.
  - `Stryker.Configuration` — parsing of config, file patterns (see `FilePattern.Parse`), and ID generation (`BasicIdProvider`).
  - `Stryker.Solutions` — solution parsing / project selection logic (see `SolutionFile.cs`).
  - `Stryker.CLI` — the command-line entrypoint that wires components and exposes user-facing options.

Key, discoverable conventions and patterns (concrete examples)
- Central API surface lives in `Stryker.Abstractions`:
  - Mutations are `Mutation` objects (see `Stryker.Abstractions/Mutation.cs`) containing `OriginalNode`, `ReplacementNode`, `Type` (enum `Mutator`) and `DisplayName`.
  - Mutator enum values are decorated with `MutatorDescription` attributes (search `MutatorDescriptionAttribute.cs`) — use these strings in UI or reporting.
- File inclusion/exclusion: `FilePattern.Parse(string)` supports pattern format `(!)<glob>({<spanStart>..<spanEnd>})*` (see `Stryker.Configuration/FilePattern.cs`). When no spans are present the pattern implicitly covers the whole file.
- ID generation: `BasicIdProvider.NextId()` uses `Interlocked.Increment(ref _id)-1` — IDs start at 0 (see `Stryker.Configuration/IdProvider.cs`).
- Test-runner integration:
  - `Stryker.DataCollector/CoverageCollector.cs` is a VSTest in-proc data collector. It expects a `MutantControl` type with fields/methods named `ActiveMutant`, `CaptureCoverage`, and `GetCoverageData` — these are set through the data-collector XML produced by `CoverageCollector.GetVsTestSettings(...)`.
  - Mutant → tests mapping is passed as `<Mutant id='...' tests='guid,guid'/>` elements inside the generated VSTest settings XML (see `GetVsTestSettings`). Agents should build these XML fragments rather than hand-editing global test settings.
- Environment tweaks for test processes: `Stryker.TestRunner/ExternalEnvironmentVariables.Add(...)` adds recommended environment variables for child test processes (e.g. disables diff GUIs used by approval tests). Reuse this when spawning test runners.
- Solution resolution: `Stryker.Solutions/SolutionFile.cs` encapsulates heuristics to pick best build configuration/platform from a solution — use `GetMatching(...)` and `GetProjects(...)` rather than parsing the sln manually.

Developer workflows (commands and tips)
- Restore & build (recommended):
  - dotnet restore
  - dotnet build Stryker.slnx -c Release
  (Solution file is `Stryker.slnx` in the repository root of this workspace)
- Run CLI locally (wire-up):
  - dotnet run --project Stryker.CLI/Stryker.CLI.csproj -- [your CLI args]
- Run unit tests per project (typical naming): many tests live next to their projects using `*.UnitTest` suffix (e.g. `Stryker.Core.UnitTest`). Run with `dotnet test <proj> -c Debug` or use `--no-build` if already built.
- Debugging test runners and data collector:
  - To inspect the in-proc `CoverageCollector`: set breakpoints in `Stryker.DataCollector/CoverageCollector.cs` and run the test host (vstest) with an instrumentation config (the collector provides `GetVsTestSettings`).
  - When running test-host processes from the CLI, check `Stryker.TestRunner/ExternalEnvironmentVariables` to ensure spawned processes get consistent environment variables.

Integration & external dependencies
- Central package versioning: `Directory.Packages.props` / `Directory.Build.props` are used — changes to package versions are centralized.
- VSTest integration: `Microsoft.VisualStudio.TestPlatform.ObjectModel.*` is used extensively by `Stryker.DataCollector` and test-runner implementations. Expect tight coupling to VSTest's data-collector API and test-host process model.
- Solution parsing uses `Microsoft.VisualStudio.SolutionPersistence.*` to read `.sln`/`.slnx` files (see `Stryker.Solutions/SolutionFile.cs`).

Where an AI agent should start (short guided path)
1. Open `Stryker.Abstractions` to learn domain types (Mutation, Mutator, IProjectAndTests).
2. Skim `Stryker.Configuration/FilePattern.cs` and `IdProvider.cs` to understand how CLI options translate to targets.
3. Read `Stryker.Core`'s entrypoints (search for orchestration classes) to learn the main loop of mutation generation and test execution.
4. Inspect `Stryker.DataCollector/CoverageCollector.cs` and `Stryker.TestRunner.MicrosoftTestPlatform/*` to see how coverage and test execution interact.

Notes / gotchas for code changes
- The project uses central package/version files; updating packages often requires edits to `Directory.Packages.props` rather than individual csproj files.
- The data collector expects specific names/signatures for the Mutant control type (ActiveMutant, CaptureCoverage, GetCoverageData). Changing these names will break the VSTest integration unless the generated settings (XML) are updated accordingly.
- Many projects include a `stryker-config.json` at project roots — tests and local scripts may rely on these sample configs.

Files worth opening immediately
- `Stryker.Abstractions/Mutation.cs`
- `Stryker.Configuration/FilePattern.cs`
- `Stryker.DataCollector/CoverageCollector.cs`
- `Stryker.Solutions/SolutionFile.cs`
- `Stryker.TestRunner/ExternalEnvironmentVariables.cs`

If you need more detail
- Ask for a concise map of the core call graph (e.g., "produce call graph for mutation orchestration") or request specific examples (how a mutant id flows from generation → test-run → coverage report).

---
(Generated by an analysis of this workspace's source files. If you want a shorter or more detailed agent prompt version of this, tell me which focus: onboarding, debugging, or contributor workflow.)
