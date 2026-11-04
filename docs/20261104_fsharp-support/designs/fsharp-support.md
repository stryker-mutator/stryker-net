# fsharp-support — Design Doc

## Context

Stryker.NET supported F# only as dead plumbing: parse + a toy AST rewrite, no mutators,
no mutant IDs, no runtime switching, no coverage ([#1391](https://github.com/stryker-mutator/stryker-net/pull/1391)).
It was deleted in [#3121](https://github.com/stryker-mutator/stryker-net/pull/3121) (Dec 2024)
because the pinned `FSharp.Compiler.Service 42.7.100-preview` pulled a vulnerable
`System.Drawing.Common` that security scanners flagged for every user
([#3118](https://github.com/stryker-mutator/stryker-net/issues/3118)). The removal was a
dependency decision, not a design one.

Today an F# source project crashes with an unhandled `NotSupportedException`
(`src/Stryker.Core/Stryker.Core/Initialisation/MutableProjectTarget.cs:64-67`), even though
`.fsproj` discovery and language detection already work
(`src/Stryker.Utilities/Buildalyzer/IAnalyzerResultExtensions.cs:169`).

Key analysis findings:
- The runtime mutant-activation contract is **language-agnostic**: test runners only need a
  compiled `<ns>.MutantControl` type (static fields `ActiveMutant`, `CaptureCoverage`, method
  `GetCoverageData()`) plus environment variables. Nothing in VsTest/MTP reads C# source.
- The core blocker is `Mutation.OriginalNode/ReplacementNode` being Roslyn `SyntaxNode`
  (`src/Stryker.Abstractions/Mutation.cs:10-11`); filters, baseline, and three reporters read
  Roslyn locations from it.
- F# is expression-oriented: the C# schemata pattern
  `(if Stryker.MutantControl.IsActive n then mutated else original)` maps directly onto any
  F# expression.
- The old AST-rewrite approach is obsolete: FCS positional AST constructors changed across
  v42/v43 and would not compile today.

## Functional Requirements

### <a id="fr1"></a>FR1 — F# project runs end to end
`dotnet stryker` on an F# source project with an F# (or C#) test project produces a
mutation report with real mutants (killed/survived), instead of crashing.

### <a id="fr2"></a>FR2 — Mutant schemata (compile once)
All mutants are injected in one compilation; the active mutant is selected at run time via
the existing `MutantControl` contract, so the existing VsTest and MTP runners work unchanged.

### <a id="fr3"></a>FR3 — Compile-error rollback
A mutant whose injection breaks compilation is marked `BuildError` and removed; the run
continues. A clean build is reached within a bounded number of retries.

### <a id="fr4"></a>FR4 — Per-test coverage
Coverage analysis (`--coverage-analysis perTest`) works for F# mutants through the existing
data collector / MTP coverage contract.

### <a id="fr5"></a>FR5 — Reporting
JSON/HTML reports show F# source with correct 1-based spans and `language: "fs"`
(mutation-testing-elements supports fs highlighting).

### <a id="fr6"></a>FR6 — Initial mutator set
At least: boolean literal, arithmetic binary operators, equality/comparison operators,
string literals, and collection functions — the F# analogue of the C# LINQ mutator,
mapping module functions instead of extension methods (`l.Select(x => x)` ≡
`l |> Seq.map (fun x -> x)`), e.g. `Seq.min`↔`Seq.max`, `Seq.head`↔`Seq.last`, with
`List`/`Array` variants. The set is extensible through the same registration pattern as C#.

### <a id="fr7"></a>FR7 — Graceful degradation
C#-only features (ignored-method filter, baseline node-matching, test-to-mutant linking)
log a clear "not supported for F#" message instead of throwing. The
`ExcludeLinqExpressionFilter` concern gets a real F# analogue instead of degradation:
the orchestrator never splices inside quotations (`SynExpr.Quote`, `<@ … @>`), where an
injected `IsActive` call would surface in the expression tree and change semantics.

## Non-Functional Requirements

### <a id="nfr1"></a>NFR1 — No vulnerability-scanner regression
- **Scenario**: restore the solution with the new F# dependencies → scan transitive packages
- **Measure**: zero known vulnerabilities introduced by F# support
- **Verify**: `dotnet list src/Stryker.slnx package --vulnerable --include-transitive` exits 0 with no vulnerable entries

### <a id="nfr2"></a>NFR2 — C# pipeline unchanged
- **Scenario**: existing unit + integration test suites after the refactor
- **Measure**: all pre-existing tests pass; C# integration-test mutant counts unchanged
- **Verify**: `dotnet test src/Stryker.vstest.slnf -c Debug` and `pwsh ./integration-tests.ps1`

### <a id="nfr3"></a>NFR3 — Deterministic compile order
- **Scenario**: multi-file F# project where file order matters
- **Measure**: mutated compilation uses the `.fsproj` compile-item order (from Buildalyzer),
  never directory enumeration order
- **Verify**: integration target project with order-dependent files compiles and is mutated

## Non-goals

- **Mutating `.fsx` scripts and `.fsi` signature files** — deferred: no project context /
  no executable surface; revisit on demand.
- **Test-to-mutant source linking for F# test projects** — already tolerated (empty
  `TestFiles`, `TestProject.cs:33-37`); linking is an optimisation, not a correctness need.
- **Porting every C# mutator** — the C# set took years; FR6 defines the floor, the ADR'd
  mechanism makes additions cheap. The per-mutator verdicts (5 floor, 5 wave-2 ports,
  6 later, 10 n/a of 26) live in [mutator-portability.md](../mutator-portability.md).
- **F#-specific mutators** (pipe removal, pattern-match case deletion) — valuable later;
  excluded to bound scope.
- **Shared/multi-targeted project edge cases** — follow the C# behaviour where free.

## Rabbit holes

- **FCS untyped-AST reconstruction**: rebuilding `SynExpr` trees with positional
  constructors breaks on every FCS upgrade (proven by the deleted code). Cap: the chosen
  mechanism must not construct AST nodes (see mutation-mechanism ADR).
- **In-memory FCS compilation**: `FSharpChecker.Compile` overloads are undocumented and
  changed across versions. Cap: one spike time-boxed; fall back to `dotnet build` of a
  mutated source copy.
- **Operator semantics**: F# operators are SRTP-generic; mutating `+`→`-` on strings won't
  compile (rollback handles it, as it does for C#). Do not build a type-checker pass in v1;
  rely on FR3 rollback.
- **Trivia/formatting preservation**: only matters for report display; text-splice keeps the
  original source untouched except at splice points.

## Failure modes

| Failure | Detection | Response | Blast radius |
|---|---|---|---|
| Mutated F# source fails to compile | FCS/MSBuild diagnostics with ranges | map diagnostic range → splice id → mark `BuildError`, re-splice without it, retry (bounded, as C#: ≤50) | single mutants |
| Diagnostic has no range / maps to no splice | lookup miss | escalate: drop all mutants in that file; if still failing, fail the run with the diagnostics | one file / run |
| `.fsproj` analysis yields no compile items | empty list after Buildalyzer | `InputException` naming the project | run (clean error) |
| F# test project source parse | n/a — never parsed | keep current behaviour: empty `TestFiles`, debug log | none |
| Helper assembly missing at test run | `TypeLoadException` in runner | fail fast in build step: verify helper reference before running tests | run (clean error) |
| FCS parse error on valid file (FCS bug/version skew) | parse diagnostics on unmutated source | skip file, warn, continue | one file |

## Design

Mirror the C# pipeline shape, one F# implementation per existing seam — no new
architecture:

| Seam (exists) | C# impl | New F# impl |
|---|---|---|
| `ProjectComponentsBuilder` | `CsharpProjectComponentsBuilder` | `FsharpProjectComponentsBuilder` (compile-item order, FCS parse for ranges) |
| `IMutationProcess` | `CsharpMutationProcess` | `FsharpMutationProcess` |
| compile step | `CsharpCompilingProcess` (Roslyn, in-memory) | `FsharpCompilingProcess` (per compile-strategy ADR) |
| rollback | `CSharpRollbackProcess` (annotations) | splice-id map: diagnostic range → mutant id |
| orchestrator | `CsharpMutantOrchestrator` | `FsharpMutantOrchestrator` (walks FCS untyped AST, emits text splices) |
| mutators | `MutatorBase<T: SyntaxNode>` | `IFsharpMutator`: `SynExpr → replacement text` candidates |

Language selection happens at the two existing switch points:
`MutableProjectTarget.cs:62-67` (components builder) and the `IMutationProcess`
registration (`ServiceCollectionExtensions.cs:31` → keyed/factory by
`SourceProjectInfo` language).

Prefactoring (benefits C# too): give `Mutation` a language-neutral location
(file path + 1-based line/column span + display texts) and migrate filters, baseline,
and reporters off `OriginalNode.GetLocation()`. Roslyn nodes stay available on a
C#-specific subtype for the C# pipeline.

Mutant activation: unchanged. The mutated F# compilation references the existing C#
helper (`MutantControl`/`MutantContext`) compiled into a small helper assembly with the
per-project randomised namespace; `IsActive` calls in splices use that namespace. Runners
and data collector work unchanged (their contract is reflection on the compiled type +
env vars).

Decisions:
- [Mutation mechanism: text splice at FCS ranges](../adrs/mutation-mechanism.md)
- [Compile strategy for mutated F#](../adrs/compile-strategy.md)
- [FCS dependency and packaging](../adrs/fsharp-compiler-dependency.md)
- [Language-neutral Mutation location](../adrs/mutation-location-abstraction.md)

C4 diagram: N/A — single container (CLI tool); the table above carries the structure.
STRIDE: N/A — no new trust boundary; mutated code runs where mutated code already runs.

## Data & migration

N/A: no persistence. Report schema already supports `language: "fs"`. Baseline reports
from older versions contain no F# mutants, so no migration.

## Cross-cutting Concerns

- **Observability**: log chosen language per project at Info; splice/rollback counts at
  Debug (same places the C# pipeline logs).
- **Rollout**: F# path only activates for `.fsproj` sources — C# users cannot hit it.
  First release marked experimental in docs.
- **Rollback**: revert is a plain code revert; no stored state.
- **Upstream**: this targets an OSS repo — open a maintainer discussion before the PR
  (removal reason was dependency hygiene; NFR1 answers it).
