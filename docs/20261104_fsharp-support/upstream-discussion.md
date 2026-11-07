# Upstream discussion draft: F# support for Stryker.NET

Draft for maintainers. Not posted. A human sends it.

## Proposal

We would like to contribute experimental F# mutation support, in a separate `Stryker.Fsharp` package. We know F# support was removed in #3121, so this draft starts with that reason.

## Why the removal reason no longer applies

The removal was dependency hygiene. The pinned FCS preview pulled in a vulnerable `System.Drawing.Common` (#3118), which flagged every Stryker install. This proposal answers it in two ways:

- **Stable FCS, isolated.** FCS 43.12.403 (stable) lives only in `Stryker.Fsharp`. `Stryker.Core` has zero FCS references. C# users never restore it, and a future FCS advisory would only affect F# users.
- **CI gate.** CI runs `dotnet list package --vulnerable --include-transitive` and fails on any hit.

## Architecture

Mutants are text splices at FCS source ranges. The mutated source is not rebuilt from the AST, which was the fragility of the old implementation. Each mutated expression becomes a mutant-switch expression, all mutants compile once in-process through FCS, and uncompilable mutants roll back using compiler diagnostics. Coverage is per test. The existing runner contract is untouched: tests run through VsTest, and reports use language `fs`.

## Breaking change in Stryker.Abstractions

`Mutation` was tied to Roslyn nodes. It now carries a neutral location and texts. Roslyn nodes moved to `CsharpMutation`, and `IMutator` returns `CsharpMutation`. Out-of-tree C# mutators and tooling that read `Mutation` will need a small update. This refactor stands alone and is the only change to existing public types.

## Evidence

- 88 F# unit tests in `Stryker.Fsharp.UnitTest`.
- Integration category `Fsharp`, with pinned counts: 11 mutants (5 killed, 2 survived, 1 no coverage, 3 compile error).
- Existing C# integration categories keep their counts.

## Supported scope

Boolean, arithmetic, equality, string literal and `String.Empty`, and `Seq`/`List`/`Array` collection function mutators. The new F# support page in the docs has the full table.

## Limitations

- Lambda bodies and `match` expressions are not walked yet.
- No `.fsx` or `.fsi` support, and no source linking for F# test files.
- Interpolated strings are skipped.
- The walker does not descend into tuples, array and list literals, records, type members, or `do` expressions.
- Extension-method LINQ is not matched.
- The injected helper assembly is unsigned. Signed F# projects are untested.
- F#-specific mutators, such as pipe removal, are future work.

## Review follow-ups

Known and deferred:

- Unattributable compile errors are fatal, with diagnostics. This is safe, but a rollback strategy could recover.
- The orchestrator does not descend into tuples, array and list literals, records, type members, or `do` expressions (documented walker gap).
- The `TrackValue` wrapper on multi-line module values may trip offside rules. It degrades safely to dropping the file's mutants.
- `BaselineMutantHelper` no longer validates that the span is a node.
- `-r:` and `-I:` arguments assume Buildalyzer reports project-relative paths.
- `DefaultCsharpBuilderFactory` duplicates some construction code.

## Options

We can open one PR, or split it. The `Mutation` refactor can go first on its own. The `Stryker.Fsharp` package and docs would follow. Tell us which you prefer.

Design record: [the design](./designs/fsharp-support.md) and the ADRs in [adrs](./adrs/).
