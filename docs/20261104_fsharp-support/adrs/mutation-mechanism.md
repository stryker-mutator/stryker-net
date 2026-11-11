---
status: accepted
---
# Mutation mechanism: text splice at FCS ranges

Addresses: [FR2](../designs/fsharp-support.md#fr2), [FR3](../designs/fsharp-support.md#fr3), [NFR3](../designs/fsharp-support.md#nfr3)

## Problem
How to inject `(if Stryker.MutantControl.IsActive n then mutated else original)` into F#
code. Hard to reverse: the whole orchestrator, rollback, and mutator API hang off it. The
deleted implementation proved AST reconstruction breaks on FCS upgrades.

## Options
| Option | Pros | Cons |
|---|---|---|
| A. AST rewrite (build `SynExpr` nodes, compile ASTs) | mirrors C# design | FCS positional constructors change every major version (proven); trivia fields fragile; undocumented compile-from-AST API |
| B. Text splice: parse with FCS only to *find* expression ranges, then splice replacement text into the source string | survives FCS upgrades (ranges are stable API); trivial rollback (drop splice by id); exact original text preserved for reports; mutators are `range → text` (easy to write) | needs careful parenthesisation; offside rule — splices must stay on one line or preserve indentation |
| C. Fantomas round-trip (mutate AST, print with Fantomas) | clean output | adds a dependency; still builds AST nodes (same fragility as A) |

## Decision
Option B. Mutators return replacement text for a source range; the orchestrator wraps
`(if <ns>.MutantControl.IsActive <id> then <mutated> else <original>)` always fully
parenthesised, splicing right-to-left per file so ranges stay valid. A splice map
(file, final range → mutant id) drives rollback.

## Consequences
Easier: FCS upgrades, mutator authoring, rollback, report spans (original text untouched).
Harder: nested mutants inside an already-spliced expression must splice inner-first
(right-to-left, depth-first ordering handles it); multi-line expressions must be wrapped
without breaking the offside rule (parenthesised form is whitespace-insensitive inside).
