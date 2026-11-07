# Glossary

**Helper assembly**: a small assembly compiled from Stryker's C# runtime helpers and
referenced by a mutated F# compilation, so test runners find the mutant-control type
they expect. Replaces: injected helpers (for F#).

**Mutant schemata**: all mutants compiled into one assembly, each wrapped in a runtime
guard, with the active mutant selected per test run through an environment variable.
Replaces: one-compile-per-mutant.

**Rollback**: removing the mutants whose injected code fails compilation, marking them
compile errors, and retrying within a bounded number of attempts. Replaces: build-error
retry loop.

**Text splice**: a source-text replacement at a compiler-reported range that wraps an
expression in a mutant guard, applied inner-first and right-to-left so ranges stay
valid. Replaces: AST rewrite.
