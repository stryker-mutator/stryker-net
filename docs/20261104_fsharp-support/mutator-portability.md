# C# → F# mutator portability matrix

Addresses [FR6](./designs/fsharp-support.md#fr6) and bounds the "porting every C# mutator" non-goal in
[the design](./designs/fsharp-support.md#non-goals). Source of truth for the C# set:
`DefaultMutatorList()` in `CsharpMutantOrchestrator.cs:94` — 26 registered mutators.

Verdicts: **floor** = in this plan's tasks; **wave 2** = cheap port, first follow-up;
**later** = portable, needs its own analysis; **n/a** = no F# counterpart or already
covered by expression mutation.

| C# mutator | Verdict | F# mapping / reason |
|---|---|---|
| `BinaryExpressionMutator` (arith, equality, logical, bitwise, shift) | floor (task 12, task 13) + wave 2 | operator idents (`op_Addition`, …) in `SynExpr.App`; wave 2 extends to `&&`/`\|\|` and `&&& \|\|\| ^^^ <<< >>>` |
| `BooleanMutator` | floor (task 8) | `true`↔`false` literals |
| `StringMutator` | floor (task 14) | literal↔`"Stryker was here!"` |
| `StringEmptyMutator` | floor (fold into task 14) | `String.Empty` is the same .NET member |
| `LinqMutator` | floor (task 15) | `l.Select(f)` ≡ `l \|> Seq.map f`: mutate `Seq`/`List`/`Array` module functions (`min`↔`max`, `head`↔`last`, `sortBy`↔`sortByDescending`, …) |
| `ConditionalExpressionMutator` | wave 2 | force `if`/`elif`/`while` condition to `true`/`false` |
| `NegateConditionMutator` | wave 2 | wrap condition in `not (…)` |
| `PrefixUnaryMutator` | wave 2 (partial) | `-x`→`+x` (`op_UnaryNegation`), remove `not`; `++/--` have no F# form |
| `InitializerMutator` + `ArrayCreationMutator` + `CollectionExpressionMutator` | wave 2 (merged into one F# mutator) | `[1;2]`→`[]`, `[\|1;2\|]`→`[\|\|]`, `seq { … }`→`Seq.empty` |
| `MathMutator` | wave 2 | same `System.Math` calls, plus F# `min`/`max`/`abs` function idents |
| `StringMethodMutator` | later | `s.Trim()` is `SynExpr.DotGet` + `App`; same splice, broader matching |
| `StringMethodToConstantMutator` | later | with the above |
| `InterpolatedStringMutator` | later | F# `$"…"` exists; mutate the whole literal only (no partial splice inside quotes, task 14 rule) |
| `RegexMutator` | later | `Stryker.RegexMutators` package is language-agnostic; only `Regex(…)` call-site detection differs |
| `BinaryPatternMutator` | later | F# or-patterns `\|` and and-patterns `&` exist; niche, needs semantics check |
| `NullCoalescingExpressionMutator` | later | no `??`; analogue mutates `defaultArg`/`Option.defaultValue` via the task-15 table |
| `RelationalPatternMutator` | n/a — covered | F# uses `when` guards: plain expressions, already mutated by the operator mutators |
| `IsPatternExpressionMutator` | n/a — covered | `match` arms are expressions; `:? T` tests not worth v1 |
| `BlockMutator` | n/a | no statement blocks; nearest (unit body → `()`) is low value |
| `AssignmentExpressionMutator` | n/a | no compound assignment (`+=`) in F# |
| `PostfixUnaryMutator` | n/a | `x++`/`x--` don't exist |
| `CheckedMutator` | n/a | F# checked arithmetic is `open Checked`, a module swap, not syntax |
| `StatementMutator` | n/a | `return`/`break`/`continue`/`goto` don't exist |
| `ObjectCreationMutator` | n/a | records require all fields; C#-style object initializers are rare F# |

Net: 5 floor, 5 wave-2 ports, 6 later, 10 n/a. F#'s expression-oriented syntax makes most
"n/a" rows meaningless rather than missing — their mutation surface is already reached
through expression mutators.

Wave 2 is the natural next workspace after integration
(the amending rule, plan skill): one task per row, same `IFsharpMutator`
text-splice pattern, no new ADRs expected.
