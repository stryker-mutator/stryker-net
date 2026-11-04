# fsharp-support

Experimental F# mutation support for Stryker.NET, integrated 2026-11-04. F# source
projects are mutated with mutant schemata (text splices at FCS ranges), compiled
in-process with the F# compiler service, and tested through the unchanged VsTest
runner contract. User documentation: [docs/fsharp.md](../fsharp.md).

## Design

- [fsharp-support design](./designs/fsharp-support.md) — context, requirements,
  failure modes, and the seam-by-seam architecture.
- [Mutator portability matrix](./mutator-portability.md) — all 26 C# mutators triaged
  for F# (5 floor, 5 wave-2 ports, 6 later, 10 n/a).
- [Upstream discussion draft](./upstream-discussion.md) — maintainer-facing proposal,
  including the review follow-ups list.

## ADRs

- [Mutation mechanism: text splice at FCS ranges](./adrs/mutation-mechanism.md)
- [Compile strategy for mutated F#](./adrs/compile-strategy.md)
- [FCS dependency and packaging](./adrs/fsharp-compiler-dependency.md)
- [Language-neutral Mutation location](./adrs/mutation-location-abstraction.md)

## Lessons

- A constraint shared by several tasks needs its own task: "honour `MutationLevel`"
  appeared in every mutator task, and nothing implemented the filtering until it was
  made explicit mid-run.
