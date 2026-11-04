---
status: accepted
---
# Language-neutral Mutation location

Addresses: [FR5](../designs/fsharp-support.md#fr5), [FR7](../designs/fsharp-support.md#fr7), [NFR2](../designs/fsharp-support.md#nfr2)

## Problem
`Mutation.OriginalNode/ReplacementNode` are Roslyn `SyntaxNode`
(`src/Stryker.Abstractions/Mutation.cs:10-11`). Mutant filters, baseline matching, and
three reporters read Roslyn spans/text from them, so no non-Roslyn language can produce a
reportable mutant. Hard to reverse: `Stryker.Abstractions` is the published contract other
components (and potential plugins) compile against.

## Options
| Option | Pros | Cons |
|---|---|---|
| A. Add neutral fields (file path, 1-based span, original/replacement display text) to `Mutation`; keep Roslyn nodes on a C#-only subtype | consumers (filters, baseline, reporters) become language-neutral; F# fills the same type | touch ~10 consumer sites; C#-specific filters must branch on node availability |
| B. Parallel `FsharpMutation` type | no C# churn | every consumer needs two code paths forever; reporters duplicated |
| C. Make `Mutation` generic over node type | type-safe | generic infection through `Mutant`, filters, reporters — huge churn for no behavioural gain |

## Decision
Option A. Neutral location+text become the canonical fields; `OriginalNode` stays (C#
subtype) for the Roslyn-walking filters (`IgnoredMethodMutantFilter`,
`ExcludeLinqExpressionFilter`, …), which skip mutants without a node (FR7). Reporters,
`FilePatternMutantFilter`, `IgnoreBlockMutantFilter`, and baseline matching move to the
neutral fields — an observable no-op for C# guarded by NFR2's test suites.

## Consequences
Easier: F# (and any future language) reaches reporting untouched; reporter unit tests
lose their Roslyn fixtures. Harder: one-time migration of the consumer sites; the
1-based/0-based conversion (`Reporters/Json/Location.cs:18-28`) is centralised once.
