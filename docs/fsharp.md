---
title: F# support
sidebar_position: 45
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/fsharp.md
---

> **Experimental.** F# support is new. Mutator coverage is limited, and behaviour and output may change between releases. Please report problems in the [issue tracker](https://github.com/stryker-mutator/stryker-net/issues).

Stryker can mutate F# projects. Run it as you would for C#: see [getting started](./getting-started.md).

## What works

- **Mutant schemata.** Stryker splices a switch around each mutated expression in the original source text, so one compile holds all mutants. The active mutant is selected at run time.
- **In-process compile.** The F# compiler services compile the mutated project in-process. No extra build step is needed.
- **Rollback.** A mutant that fails to compile is reported as a compile error and removed. Stryker then recompiles the rest.
- **Per-test coverage.** Stryker records which tests reach each mutant and runs only those tests.
- **Test runner and reports.** Tests run through VsTest. Reports use the language identifier `fs`, so the HTML and JSON reports highlight F# source.

## Supported mutators

The mutator names in parentheses work with `ignore-mutations`. See [mutations](./mutations.md) for the C# equivalents.

| Mutator | Original | Mutated |
| ------- | -------- | ------- |
| Boolean (_boolean_) | `true` / `false` | `false` / `true` |
| Arithmetic (_arithmetic_) | `+` `-` `*` `/` `%` | `+` to `-`, `-` to `+`, `*` to `/`, `/` to `*`, `%` to `*` |
| Equality (_equality_) | `=` | `<>` |
| | `<>` | `=` |
| | `<` | `>` and `<=` |
| | `<=` | `>` and `<` |
| | `>` | `<` and `>=` |
| | `>=` | `<` and `>` |
| String (_string_) | `"text"` | `""` |
| | `""` | `"Stryker was here!"` |
| | `String.Empty` | replaced |
| Collection functions (_linq_) | `Seq`, `List` and `Array` functions | the counterpart in the same module (see below) |

The collection function pairs are swapped in both directions, for each of `Seq`, `List`, and `Array`:

- `min` and `max`
- `minBy` and `maxBy`
- `head` and `last`
- `tryHead` and `tryLast`
- `sortBy` and `sortByDescending`
- `sort` and `sortDescending`
- `take` and `skip`
- `takeWhile` and `skipWhile`
- `exists` and `forall`

Equality mutations belong to the `Basic` mutation level, as in C#. See [mutation levels](./configuration.md).

## Known limitations

- Lambda bodies (`fun x -> ...`) and `match` expressions are not walked yet. Stryker generates no mutants inside them.
- Only `.fs` project sources are mutated. `.fsx` scripts and `.fsi` signature files are not supported.
- Test files are not linked to source locations for F# test projects.
- Interpolated strings are skipped.
- The walker does not descend into tuples, array and list literals, records, type members, or `do` expressions. Stryker generates no mutants inside them yet.
- LINQ extension methods (`xs.Where(...)`) are not matched. Only the `Seq`, `List`, and `Array` module functions are.
- The helper assembly Stryker injects is not strong-name signed. Signed F# projects are untested.
- F#-specific mutators, such as pipe removal, are future work.
