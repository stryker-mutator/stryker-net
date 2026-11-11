# Stryker.NET — agent notes

See [AGENTS.md](AGENTS.md) for project instructions.

## Conventions (from retros)

- Code consuming Buildalyzer results needs at least one test against a real analyzer
  result (see the `TempFsharpProject` fixture in `src/Stryker.Fsharp.UnitTest`), not only
  mocks: real results use project-relative paths and include generated `obj/` sources.
- Run test suites with full output captured to a log file; never pipe through
  `tail`/`head` for pass-fail decisions — the pipe masks the exit code and drops
  failure names.
- On WSL use the Linux `/usr/bin/dotnet`. Windows `dotnet.exe` on an LF checkout fails
  line-ending-sensitive tests (`Environment.NewLine` is CRLF, literals compile as LF).
- Run one integration category locally with
  `GITHUB_ACTIONS=true CATEGORY=<X> RUNTIME=<y> pwsh ./integration-tests.ps1` —
  without `GITHUB_ACTIONS=true` the script ignores `CATEGORY` and runs Solution.
