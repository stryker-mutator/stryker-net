---
status: accepted
---
# Compile strategy for mutated F#

Addresses: [FR2](../DESIGN.md#fr2), [FR3](../DESIGN.md#fr3), [NFR3](../DESIGN.md#nfr3)

## Problem
How to compile the mutated F# sources into the assembly that overwrites the test output.
Hard to reverse: rollback (FR3) and performance depend on it. The C# pipeline compiles
in-memory with Roslyn; F# has no equally stable in-memory API.

## Options
| Option | Pros | Cons |
|---|---|---|
| A. `FSharpChecker.Compile` (argv overload) in-process | no process spawn; diagnostics as objects | API semi-documented, changed across versions; must reconstruct all compiler args (references, defines, signing) from Buildalyzer ourselves |
| B. `dotnet build` of a shadow copy: write mutated sources next to a copy of the project, inject helper reference via a dropped `Directory.Build.props`, build, copy the DLL | exact production compile (order, options, signing, resources, analyzers) for free; diagnostics parseable from MSBuild output (file+range) | process spawn + MSBuild overhead per rollback retry; shadow-copy bookkeeping |
| C. Invoke `fsc` directly with args from Buildalyzer | no MSBuild overhead | same arg-reconstruction burden as A plus process management |

## Decision
Option A, with B as the capped fallback (see Rabbit holes): reconstruct the compiler
arguments from Buildalyzer's analyzer result (it exposes the exact `fsc` command line),
pass mutated source text via in-memory file system or temp files, and read
`FSharpDiagnostic` ranges directly for rollback. One time-boxed spike validates A on the
walking skeleton; if it fails, switch to B without changing any other seam
(`FsharpCompilingProcess` encapsulates the choice).

## Consequences
Easier: fast rollback retries (no MSBuild per attempt), diagnostics as typed objects.
Harder: compiler args fidelity (signing, resources, defines) is on us — covered by an
integration target project asserting the mutated assembly loads and tests run.

Task 7 outcome: Option A landed. `FsharpCompilingProcess` rebuilds the fsc argv from Buildalyzer `CompilerArguments` (relative paths absolutised, `--refout`/`--embed` dropped, output redirected to a temp dir) and compiles via `FSharpChecker.Compile(argv)`; name, version and compile order matched in a real two-file project. Note: Buildalyzer reports `SourceFiles` and `CompilerArguments` paths relative to the project directory and includes generated `*.AssemblyInfo.fs`/`AssemblyAttributes.fs` items. Resolve against the project directory (task 6 reads them verbatim; follow-up needed there). Overridden sources compile from temp copies, so diagnostics carry temp file names (map by file name in rollback).
